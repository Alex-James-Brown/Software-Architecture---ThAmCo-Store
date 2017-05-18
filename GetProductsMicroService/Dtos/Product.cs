using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GetProductsMicroService.Dtos
{
    public class Product
    {
        public virtual int Id { get; set; }
        public virtual string Ean { get; set; }
        public virtual int CategoryId { get; set; }
        public virtual string CategoryName { get; set; }
        public virtual int BrandId { get; set; }
        public virtual string BrandName { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual double Price { get; set; }
        public virtual bool InStock { get; set; }
        public virtual DateTime? ExpectedRestock { get; set; }
        public virtual string StoreName { get; set; }
    }
}