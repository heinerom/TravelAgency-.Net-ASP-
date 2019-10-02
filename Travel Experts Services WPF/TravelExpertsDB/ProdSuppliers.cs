using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelExpertsDB
{
    public class ProdSuppliers
    {
        public int ProductSupplierId { get; set; }   
        public int ProdId { get; set; }
        public int SupplierId { get; set; }

        public ProdSuppliers()
        {
            ProductSupplierId = -1;
            ProdId = -1;
            SupplierId = -1;
        }

        public ProdSuppliers(int psid, int pid, int sid)
        {
            ProductSupplierId = psid;
            ProdId = pid;
            SupplierId = sid;
        }

        public ProdSuppliers(int pid, int sid)
        {
            ProdId = pid;
            SupplierId = sid;
        }
    }
}
