using NuxeoClient;
using NuxeoClient.Wrappers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Nuxeo.NET48.Job
{
    public class NuxeoWorker
    {
        private readonly NuxeoWorkerConfig _cfg;
        private readonly Client _client;

        public NuxeoWorker(NuxeoWorkerConfig cfg)
        {
            this._cfg = cfg;
            this._client = new Client(cfg.Client.Url, new Authorization(cfg.Client.Username, cfg.Client.Password)).AddDefaultSchema("*");
        }

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private async Task<T> ExecuteQuery<T>(string nxql, string schema, int page = 0, int pageSize = 1000) where T : Entity
        {
            return (T)await _client.Operation("Document.Query")
                .SetParameter("query", nxql)
                .SetParameter("language", "nxsql")
                .SetParameter("pageSize", pageSize)
                .SetParameter("currentPageIndex", page)

                .AddSchema(schema)

                .Execute();
        }

        private string GetParentsBatchNxql(IEnumerable<string> batch)
        {
            var list = String.Join(",", batch.Select(f => $"'{f}'").ToArray());
            return
                $"SELECT * " +
                $"FROM {_cfg.Parent.Table} " +
                $"WHERE " +
                $"  ecm:primaryType = '{_cfg.Parent.Schema}' AND " +
                $"  ecm:uuid in ({list})";
        }

        private string GetAttachmentsNxql(DateTime lastDate, bool reverse)
        {
            DateTime newDate;
            if (reverse)
                newDate = lastDate.AddDays(-1);
            else
            {
                var isMoreThanDay = lastDate.AddDays(+1) < DateTime.UtcNow;
                newDate = isMoreThanDay ? lastDate.AddDays(+1) : DateTime.UtcNow;
            }

            var op = reverse ? 
                new String[] { ">=", "<=" } : 
                new String[] { "<=", ">=" };

            var order = reverse ? "DESC" : string.Empty;

            return
                $"SELECT * " +
                $"FROM {_cfg.Attachment.Table} " +
                $"WHERE " +
                $"  (ecm:versionLabel is null OR ecm:isLatestVersion = 1) AND " +
                $"  ecm:currentLifeCycleState != 'deleted' AND " +
                $"  ecm:primaryType = '{_cfg.Attachment.Schema}' AND " +
                $"  ( " +
                $"      dc:modified {op[0]} TIMESTAMP '{newDate:yyyy-MM-dd HH:mm:ss}' AND " +
                $"      dc:modified {op[1]} TIMESTAMP '{lastDate:yyyy-MM-dd HH:mm:ss}' " +
                $"  ) " +
                $"ORDER BY " +
                $"  dc:modified {order}";
        }

        public async Task<GetAttachmentsResult> GetAttachments(DateTime lastDate, bool reverse = false)
        {
            var nxql = GetAttachmentsNxql(lastDate, reverse);

            Pageable result;
            DateTime jobDate = lastDate;
            var page = 0;
            var list = new ConcurrentDictionary<string, AttachmentData>();
            do
            {
                result = await ExecuteQuery<Pageable>(nxql, _cfg.Attachment.Schema, page);

                result.Entries.ForEach(entry =>
                {
                    if (entry.LastModified.HasValue && ((!reverse && entry.LastModified > jobDate) || (reverse && entry.LastModified < jobDate)))
                        jobDate = entry.LastModified.Value;

                    if (list.TryGetValue(entry.ParentRef, out var item))
                    {
                        if (entry.LastModified > item.Modified)
                            item.Modified = entry.LastModified;
                    } else
                        list.TryAdd(entry.ParentRef, new AttachmentData { Modified = entry.LastModified });
                });

                page++;
            } while (result.IsNextPageAvailable);

            return new GetAttachmentsResult(list, result.TotalSize, jobDate);
        }

        public async Task<IEnumerable<AttachmentData>> EnrichParentField(IDictionary<string, AttachmentData> attachments)
        {
            const int batchSize = 1000;

            foreach (var batch in attachments.Keys.Batch(batchSize))
            {
                var nxql = GetParentsBatchNxql(batch);

                var result = await ExecuteQuery<Documents>(nxql, _cfg.Parent.Schema, 0, batchSize);
                result.Entries.ForEach(parent =>
                {
                    if (attachments.TryGetValue(parent.Uid, out var attachment))
                        if (parent.Properties.TryGetValue(_cfg.Parent.FieldName, out var parentId))
                            attachment.ParentId = Guid.Parse(parentId.ToString());
                });
            }
            return attachments.Values;
        }
    }
}
