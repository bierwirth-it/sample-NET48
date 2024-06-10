using CommandLine;
using NLog;
using System;

namespace Nuxeo.NET48.Job
{
    /// <summary>   A job to do incremental processing of document attachments, 
    ///             to build an index with the newest attachment date per document</summary>
    ///
    /// <remarks>   Andre Bierwirth, 07.06.2024. </remarks>

    internal class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            try
            {
                Environment.Exit(ParseAndExecute(args));
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.Message);
            }
        }

        private static int ParseAndExecute(string[] args)
        {
            var parseResult = Parser.Default.ParseArguments<CheckParams, SyncParams>(args);
            var exitCode = parseResult.MapResult(
                (CheckParams opts) => new CheckMain(opts).Main(),
                (SyncParams opts) => new SyncMain(opts).Main().Result,
                errs => 1
            );
            return exitCode;
        }
    }
}
