using System;
using System.Collections.Generic;
using System.Linq;

namespace Nuxeo.NET48.Job
{
    public class GetAttachmentsResult
    {
        public readonly IDictionary<string, AttachmentData> Result;
        public readonly long TotalCount;
        public readonly DateTime LastIndexed;

        public GetAttachmentsResult(IDictionary<string, AttachmentData> result, long totalCount, DateTime lastIndexed)
        {
            Result = result;
            TotalCount = totalCount;
            LastIndexed = lastIndexed;
        }
    }
}
