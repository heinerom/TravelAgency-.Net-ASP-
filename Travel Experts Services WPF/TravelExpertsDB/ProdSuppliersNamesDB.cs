using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace TravelExpertsDB
{
    //Ethan Shipley 
    // gets the names and ids for productsuppliers
    public class ProdSuppliersNamesDB
    {
        public static List<ProdSuppliersNames> GetProdSupAll(List<PackageProductSuppliers> ppss)
        {
            List<ProdSuppliersNames> psn_all = new List<ProdSuppliersNames>();

            SqlConnection con = DBConnection.GetConnection();
            string sql =
                "SELECT ps.ProductSupplierId, p.ProductID, s.SupplierId, s.SupName, p.ProdName " +
                "FROM Products_Suppliers ps " +
                "INNER JOIN Suppliers s " +
                "ON ps.SupplierId = s.SupplierId " +
                "INNER JOIN Products p " +
                "ON ps.ProductId = p.ProductId " +
                "where ProductSupplierId not in (select ProductSupplierId from Packages_Products_Suppliers)" +
                "ORDER BY p.ProdName, s.SupName; ";
            SqlCommand cmdselect = new SqlCommand(sql, con);
            try
            {
                con.Open();
                SqlDataReader reader = cmdselect.ExecuteReader();
                while (reader.Read())
                {
                    ProdSuppliersNames psn = new ProdSuppliersNames();
                    psn.ProductSupplierId = Convert.ToInt32(reader["ProductSupplierId"]);
                    psn.ProductID = Convert.ToInt32(reader["ProductID"]);
                    psn.SupplierId = Convert.ToInt32(reader["SupplierId"]);
                    psn.SupName = Convert.ToString(reader["SupName"]);
                    psn.ProdName = Convert.ToString(reader["ProdName"]);

                    psn_all.Add(psn);
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("The error is " + ex.Message, ex.GetType().ToString());
            }
            finally
            {
                con.Close();
            }
            return psn_all;
        }
    }
}