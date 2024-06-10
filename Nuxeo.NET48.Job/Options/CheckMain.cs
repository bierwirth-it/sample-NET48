using NLog;
using Nuxeo.NET48.Job.Model;
using System.Linq;

namespace Nuxeo.NET48.Job
{
    internal class CheckMain
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly CheckParams _options;

        public CheckMain(CheckParams options)
        {
            this._options = options;
        }

        internal int Main()
        {
            Logger.Trace($"Open DB");
            var db = new NuxeoEntities();

            var cnt = db.Order.Count();
            Logger.Info($"Check successfull can open db and read table count: {cnt}");

            Logger.Trace("Finished!");
            return 0;
        }
    }
}