using NLog;
using Nuxeo.NET48.Job.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nuxeo.NET48.Job
{
    internal class SyncMain
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly NuxeoRepository _repo;
        private readonly NuxeoWorker _worker;
        private readonly SyncParams _opts;

        public SyncMain(SyncParams opts)
        {
            this._opts = opts;
            _worker = new NuxeoWorker(NuxeoWorkerConfig.Default);
            _repo = new NuxeoRepository();
        }

        private (int added, int updated) ProcessAttachmentData(IEnumerable<AttachmentData> attachmentData)
        {
            int added = 0, updated = 0;

            foreach (var ad in attachmentData)
            {
                if (!ad.ParentId.HasValue) continue;

                _repo.FindOrAddOrder(
                    new Order(ad.ParentId.Value, ad.Modified),
                    (order) => added++,
                    (order) =>
                    {
                        updated++;
                        order.LastAttachmentChange = ad.Modified;
                    });
            }
            return (added, updated);
        }

        public async Task<int> Main()
        {
            do
            {
                IndexerState state = _repo.GetOrCreateIndexerState(_opts.Reverse);

                var attachments = await _worker.GetAttachments(state.LastIndexed, _opts.Reverse);
                var attachmentData = await _worker.EnrichParentField(attachments.Result);

                var result = ProcessAttachmentData(attachmentData);

                _repo.UpdateAndSaveIndexerState(state, attachments.LastIndexed);
                Logger.Trace($"added {result.added} updated {result.updated} to: {attachments.LastIndexed}");
            } while (_opts.Reverse);

            return 0;
        }
    }
}