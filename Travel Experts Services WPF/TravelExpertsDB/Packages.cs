using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelExpertsDB
{
    public class Packages
    {
        public int PackageID { get; set; }
        public string PkgName { get; set; }
        public DateTime? PkgStartDate { get; set; }
        public DateTime? PkgEndDate { get; set; }
        public string PkgDesc { get; set; }
        public decimal PkgBasePrice { get; set; }
        public decimal PkgAgencyCommission { get; set; }


        public Packages(int id, string pn, DateTime? sd, DateTime? ed, string pd, decimal pb, decimal pc)
        {
            PackageID = id;
            PkgName = pn;
            PkgStartDate = sd;
            PkgEndDate = ed;
            PkgDesc = pd;
            PkgBasePrice = pb;
            PkgAgencyCommission = pc;
        }

        public Packages()
        {
            PackageID = -1;
            PkgName = null;
            PkgStartDate = null;
            PkgEndDate = null;
            PkgDesc = null;
            PkgBasePrice = 0;
            PkgAgencyCommission = 0;
        }

    }
}
