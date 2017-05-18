using Newtonsoft.Json.Linq;

namespace AyycornMVCUsersFacade.Models
{
    public class JReturnModel
    {
        public virtual bool Success { get; set; }
        public virtual JObject Json { get; set; }
    }
}