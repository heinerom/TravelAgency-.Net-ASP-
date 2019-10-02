using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace TravelExpertsDB
{
    public class PackageProductSuppliersDB
    {
        // Sheila Zhao
        // display the package details 
        public static List<PackageProductSuppliers> GetProductSuppliersByPackage(Packages pkg)
        {
            List<PackageProductSuppliers> ppss = new List<PackageProductSuppliers>();
            SqlConnection con = DBConnection.GetConnection();
            string sql =
                "SELECT pk.PackageId, pk.PkgName, pps.ProductSupplierId, p.ProductId, p.ProdName, s.SupplierId, s.SupName " +
                "FROM Packages pk " +
                "inner join Packages_Products_Suppliers pps " +
                "ON pps.PackageId = pk.PackageId " +
                "inner join Products_Suppliers ps " +
                "ON pps.ProductSupplierId = ps.ProductSupplierId " +
                "inner join Products p " +
                "ON p.ProductId = ps.ProductId " +
                "inner join Suppliers s " +
                "ON s.SupplierId = ps.SupplierId " +
                "WHERE pk.PackageId = @Package " +
                "ORDER BY p.ProdName";
            SqlCommand cmdSelect = new SqlCommand(sql, con);
            cmdSelect.Parameters.AddWithValue("@Package", pkg.PackageID);
            try
            {
                con.Open();
                SqlDataReader Reader = cmdSelect.ExecuteReader();
                while (Reader.Read())
                {
                    PackageProductSuppliers pps = new PackageProductSuppliers();
                    pps.PackageId = Convert.ToInt32(Reader["PackageId"]);
                    pps.ProductSupplierId = Convert.ToInt32(Reader["ProductSupplierId"]);
                    pps.ProductId = Convert.ToInt32(Reader["ProductId"]);
                    pps.SupplierId = Convert.ToInt32(Reader["SupplierId"]);
                    pps.PkgName = Convert.ToString(Reader["PkgName"]);
                    pps.ProdName = Convert.ToString(Reader["ProdName"]);
                    pps.SupName = Convert.ToString(Reader["SupName"]);
                    ppss.Add(pps);
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
            return ppss;
        }
        // Ethan SHipley
        // Inserts Product_SupplierID with the packageID based on inputs of packageID and ProductSupplierId to link them together
        public static void InsertProductSupplierIdPpkg(int PackageId, int ProductSupplierId)
        {
            SqlConnection con = DBConnection.GetConnection();
            string sql =
                "Insert Packages_Products_Suppliers " +
                    "(PackageId, ProductSupplierId)" +
                "Values " +
                    "(@PackageId, @ProductSupplierId); ";
            SqlCommand cmdinsert = new SqlCommand(sql, con);
            cmdinsert.Parameters.AddWithValue("@PackageId", PackageId);
            cmdinsert.Parameters.AddWithValue("@ProductSupplierId", ProductSupplierId);
            try
            {
                con.Open();
                cmdinsert.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("The error is " + ex.Message, ex.GetType().ToString());
            }
            finally
            {
                con.Close();
            }
        }
        // Ethan SHipley
        // Deletes Product_SupplierID with the packageID based on inputs of packageID and ProductSupplierId
        public static void DeleteProductSupplierIdPpkg(int PackageId, int ProductSupplierId)
        {
            SqlConnection con = DBConnection.GetConnection();
            string sql =
                "Delete From Packages_Products_Suppliers " +
                "where PackageId=@PackageId AND ProductSupplierId=@ProductSupplierId";
            SqlCommand cmddelete = new SqlCommand(sql, con);
            cmddelete.Parameters.AddWithValue("@PackageId", PackageId);
            cmddelete.Parameters.AddWithValue("@ProductSupplierId", ProductSupplierId);
            try
            {
                con.Open();
                cmddelete.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("The error is " + ex.Message, ex.GetType().ToString());
            }
            finally
            {
                con.Close();
            }
        }
    }
}