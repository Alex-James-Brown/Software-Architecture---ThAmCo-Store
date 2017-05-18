using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AyycornApiPostOrdersFacade.Models
{
    public class OrderError
    {
        public virtual string ErrorMessage { get; set; }
        public virtual List<Models.Order> UnsuccessfulOrders { get; set; }
    }
}