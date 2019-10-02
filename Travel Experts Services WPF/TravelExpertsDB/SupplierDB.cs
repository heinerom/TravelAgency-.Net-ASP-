using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TravelExpertsDB
{
    public class SupplierDB
    {
        // Sheila Zhao
        public static List<Supplier> GetSuppliers()
        {
            List<Supplier> Sup = new List<Supplier>();
            Supplier Sp;
            SqlConnection con = DBConnection.GetConnection();
            string query = "SELECT SupplierId, SupName " +
                             "FROM Suppliers " +
                            "ORDER BY SupplierId";
            SqlCommand cmd = new SqlCommand(query, con);
            try
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Sp = new Supplier();
                    Sp.SupplierId = (int)reader["SupplierId"];
                    Sp.SupName = (string)reader["SupName"];
                    Sup.Add(Sp);
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
            return Sup;
        }
        
        // Sheila Zhao
        // Get the products with selected supplier
        public static List<Products> GetProductsByProductSupplier(Supplier s)
        {
            if (s == null)
                return null;
            else
            {
                List<Products> products = new List<Products>();
                SqlConnection con = DBConnection.GetConnection();
                string sql =
                    "SELECT p.ProductId, p.ProdName " +
                    "FROM Products p " +
                    "inner join Products_Suppliers ps " +
                    "ON p.ProductId = ps.ProductId " +
                    "inner join Suppliers s " +
                    "ON s.SupplierId = ps.SupplierId " +
                    "WHERE s.SupplierId = @SupplierId ";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@SupplierId", s.SupplierId);
                try
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Products product = new Products();
                        product.ProductId = Convert.ToInt32(reader["ProductId"]);
                        product.ProdName = Convert.ToString(reader["ProdName"]);
                        products.Add(product);
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
                return products;
            }
        }

        // Sheila Zhao
        // Add new supplier
        public static void InsertSupplier(Supplier ns)
        {
            SqlConnection con = DBConnection.GetConnection();
            string sql =
                         "INSERT Suppliers (SupplierId, SupName) " +
                         "VALUES ((select top 1 SupplierId from Suppliers order by SupplierId desc) +1" +
                         " , @SupName)";
            SqlCommand cmdInsert = new SqlCommand(sql, con);
            cmdInsert.Parameters.AddWithValue("@SupName", ns.SupName);

            try
            {
                con.Open();
                cmdInsert.ExecuteNonQuery();
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

        // Sheila Zhao
        // edit and update the supplier
        public static void UpdateSupplier(Supplier news, Supplier olds)
        {
            SqlConnection con = DBConnection.GetConnection();
            string sql = "UPDATE Suppliers " +
                          "SET SupName = @nSupName " +
                         "WHERE SupplierId = @SupplierId " +
                         "AND SupName = @OldSupName";
            SqlCommand cmdUpdate = new SqlCommand(sql, con);
            cmdUpdate.Parameters.AddWithValue("@nSupName", news.SupName);
            cmdUpdate.Parameters.AddWithValue("@OldSupName", olds.SupName);
            cmdUpdate.Parameters.AddWithValue("@SupplierId", olds.SupplierId);
            try
            {
                con.Open();
                cmdUpdate.ExecuteNonQuery();
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

        // Sheila Zhao
        // Delete the supplier
        public static void DeleteSupplier(Supplier ds)
        {
            SqlConnection con = DBConnection.GetConnection();
            string sql =
                "DELETE FROM  Products_Suppliers " +
                "WHERE SupplierId = @SupplierId " +
                "DELETE FROM Suppliers " +
                "WHERE SupplierId = @SupplierId " +
                "AND SupName = @SupName";
            SqlCommand cmdDelete = new SqlCommand(sql, con);
            cmdDelete.Parameters.AddWithValue("@SupplierId", ds.SupplierId);
            cmdDelete.Parameters.AddWithValue("@SupName", ds.SupName);

            try
            {
                con.Open();
                cmdDelete.ExecuteNonQuery();
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

        // Sheila Zhao
        // Get the supplier not on the list with selected product
        public static List<Supplier> GetProSupNotInList(Products p)
        {
            List<Supplier> supp = new List<Supplier>();
            SqlConnection con = DBConnection.GetConnection();
            string sql = "SELECT s.SupplierId, s.SupName " +
                        "FROM Suppliers s " +
                         "WHERE s.SupplierId NOT IN " +
                        "(SELECT s.SupplierId " +
                        "FROM Suppliers " +
                        "INNER JOIN Products_Suppliers ps " +
                        "ON s.SupplierId = ps.SupplierId " +
                        "INNER JOIN Products p " +
                        "ON p.ProductId = ps.ProductId " +
                        "WHERE p.ProductId = @ProductId)";
            SqlCommand cmdselect = new SqlCommand(sql, con);
            cmdselect.Parameters.AddWithValue("@ProductId", p.ProductId);
            try
            {
                con.Open();
                SqlDataReader reader = cmdselect.ExecuteReader();
                while (reader.Read())
                {
                    Supplier suppp = new Supplier();
                    suppp.SupplierId = Convert.ToInt32(reader["SupplierId"]);
                    suppp.SupName = Convert.ToString(reader["SupName"]);                    

                    supp.Add(suppp);
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
            return supp;
        }

    }
}