using CommandLine;
using CommandLine.Text;
using System.Collections.Generic;

namespace Nuxeo.NET48.Job
{
    [Verb("sync", HelpText = "Sync and process modified attachments")]
    class SyncParams
    {
        [Option('v', "verbose", HelpText = "Display more detailed messages.")]
        public bool Verbose { get; set; }

        [Option('q', "quiet", HelpText = "Supresses summary messages.")]
        public bool Quiet { get; set; }

        [Option('r', "reverse", HelpText = "Try to process attachments backwards")]
        public bool Reverse { get; set; }


        [Usage(ApplicationAlias = "Nuxeo.NET48.Job.exe")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Sync attachments", UnParserSettings.WithGroupSwitchesOnly(), new SyncParams { Quiet = true });
            }
        }

    }
}
