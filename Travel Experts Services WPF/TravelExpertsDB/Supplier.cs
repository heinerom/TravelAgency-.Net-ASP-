using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace TravelExpertsDB
{
    public class Supplier
    {
        public int SupplierId { get; set; }
        public string SupName { get; set; }
        public Supplier()
        {
            SupplierId = -1;
            SupName = null;
        }
        public Supplier(int id, string sn)
        {
            SupplierId = id;
            SupName = sn;
        }

        public Supplier(string sn)
        {
            SupName = sn;
        }

        public override string ToString()
        {
            return SupName;
        }
    }
}