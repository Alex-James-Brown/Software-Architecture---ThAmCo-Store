using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetProductsMicroService
{
    public interface IProductRepo
    {
        Task<IEnumerable<Dtos.Product>> GetWcfProducts();
        Task<IEnumerable<Dtos.Product>> GetHttpProducts();
    }
}
