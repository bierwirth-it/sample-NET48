using System;

namespace Nuxeo.NET48.Job.Model
{
    public class Order
    {
        public Order()
        {
        }

        public Order(Guid orderId, DateTime? lastAttachmentChange)
        {
            OrderId = orderId;
            LastAttachmentChange = lastAttachmentChange;
        }

        public Guid OrderId { get; set; }
        public DateTime? LastAttachmentChange { get; set; }
    }
}