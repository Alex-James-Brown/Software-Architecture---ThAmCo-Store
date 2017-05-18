using Newtonsoft.Json.Linq;

namespace AyycornApiPostOrdersFacade.Models
{
    public class JReturnModel
    {
        public virtual bool Success { get; set; }
        public virtual string ErrorMessage { get; set; }
        public virtual JArray Json { get; set; }
    }
}