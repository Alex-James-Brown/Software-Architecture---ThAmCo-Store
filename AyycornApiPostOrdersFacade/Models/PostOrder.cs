using System;

namespace AyycornApiPostOrdersFacade.Models
{
    public class PostOrder
    {
        public virtual string AccountName { get; set; }
        public virtual string CardNumber { get; set; }
        public virtual int ProductId { get; set; }
        public virtual int Quantity { get; set; }
        public virtual string StoreName { get; set; }
    }
}