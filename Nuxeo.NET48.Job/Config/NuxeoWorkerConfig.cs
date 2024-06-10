using System.Configuration;

namespace Nuxeo.NET48.Job
{
    public class NuxeoWorkerConfig
    {
        // This can come from app.config but i want it fixed and compiled in
        public static readonly NuxeoWorkerConfig Default = new NuxeoWorkerConfig
        {
            Client = (
                Url: "http://nuxeo/nuxeo/",
                Username: "Username",
                Password: "Password"
            ),
            Attachment = (
                Table: "file",
                Schema: "order_document"
            ),
            Parent = (
                Table: "order",
                Schema: "order",
                FieldName: "or:order_id"
            )
        };

        public static readonly NuxeoWorkerConfig AppConfig = new NuxeoWorkerConfig
        {
            Client = (
                Url: ConfigurationManager.AppSettings["Client.Url"] ?? Default.Client.Url,
                Username: ConfigurationManager.AppSettings["Client.Username"] ?? Default.Client.Username,
                Password: ConfigurationManager.AppSettings["Client.Password"] ?? Default.Client.Password
            ),
            Attachment = (
                Table: ConfigurationManager.AppSettings["Attachment.Table"] ?? Default.Attachment.Table,
                Schema: ConfigurationManager.AppSettings["Attachment.Schema"] ?? Default.Attachment.Schema
            ),
            Parent = (
                Table: ConfigurationManager.AppSettings["Parent.Table"] ?? Default.Parent.Table,
                Schema: ConfigurationManager.AppSettings["Parent.Schema"] ?? Default.Parent.Schema,
                FieldName: ConfigurationManager.AppSettings["Parent.FieldName"] ?? Default.Parent.FieldName
            )
        };

        public (string Url, string Username, string Password) Client;
        public (string Table, string Schema) Attachment;
        public (string Table, string Schema, string FieldName) Parent;
    }

}
