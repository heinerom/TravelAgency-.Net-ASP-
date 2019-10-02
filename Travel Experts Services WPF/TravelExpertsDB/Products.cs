using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelExpertsDB
{
    public class Products
    {
        public int ProductId { get; set; }
        public string ProdName { get; set; }

        public Products()
        {
            ProductId = -1;
            ProdName = null;
        }

        public Products(int id, string pn)
        {
            ProductId = id;
            ProdName = pn;
        }

        public Products(string pn)
        {
            ProdName = pn;
        }

        public override string ToString()
        {
            return ProdName;
        }
    }
}
