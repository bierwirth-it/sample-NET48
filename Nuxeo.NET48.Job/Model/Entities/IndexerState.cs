using System;
using System.ComponentModel.DataAnnotations;

namespace Nuxeo.NET48.Job.Model
{
    public class IndexerState
    {
        public IndexerState()
        {
        }

        public IndexerState(string key, DateTime lastIndexed)
        {
            Key = key;
            LastIndexed = lastIndexed;
        }

        [StringLength(255)]
        public string Key { get; set; }
        public DateTime LastIndexed { get; set; }
    }
}