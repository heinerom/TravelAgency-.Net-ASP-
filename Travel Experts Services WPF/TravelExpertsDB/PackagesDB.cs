using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace TravelExpertsDB
{
    public static class PackagesDB
    {
        public static List<Packages> GetPackages()
        {
            List<Packages> Packages = new List<Packages>();
            Packages pkg;
            string query = "SELECT * FROM Packages";
            SqlConnection Connection = DBConnection.GetConnection(/*ServerName, DatabaseName*/);
            SqlCommand cmd = new SqlCommand(query, Connection);
            try
            {
                Connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();//
                while (reader.Read())
                {
                    pkg = new Packages();
                    pkg.PackageID = Convert.ToInt32(reader["PackageID"]);
                    pkg.PkgName = reader["PkgName"].ToString();// getting Package name
                    if (reader["PkgStartDate"] == DBNull.Value)
                    {
                        pkg.PkgStartDate = null;
                    }
                    else
                    {
                        pkg.PkgStartDate = Convert.ToDateTime(reader["PkgStartDate"]);
                    }
                    if (reader["PkgEndDate"] == DBNull.Value)
                    {
                        pkg.PkgEndDate = null;
                    }
                    else
                    {
                        pkg.PkgEndDate = Convert.ToDateTime(reader["PkgEndDate"]);
                    }
                    pkg.PkgDesc = reader["PkgDesc"].ToString();// getting Package name
                    pkg.PkgBasePrice = Convert.ToDecimal(reader["PkgBasePrice"]);// getting Package name
                    pkg.PkgAgencyCommission = Convert.ToDecimal(reader["PkgAgencyCommission"]);
                    if (reader["PkgAgencyCommission"] == DBNull.Value)
                    {
                        pkg.PkgAgencyCommission = 0;
                    }
                    else
                    {
                        pkg.PkgAgencyCommission = Convert.ToDecimal(reader["PkgAgencyCommission"]);
                    }
                    Packages.Add(pkg);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("The error is " + ex.Message, ex.GetType().ToString());
            }
            finally
            {
                Connection.Close();
            }
            return Packages; // returns the gathered packagaes information
        }
        public static void UpdatePackages(Packages packages, int pkgid)
        {
            //query to update the table
            string query = "UPDATE Packages " +
                           "SET PkgName = " + "'" + packages.PkgName + "', " +
                           "PkgBasePrice = " + "'" + packages.PkgBasePrice + "', " +
                           "PkgDesc = " + "'" + packages.PkgDesc + "', " +
                           "PkgEndDate = " + "'" + packages.PkgEndDate + "', " +
                           "PkgStartDate = " + "'" + packages.PkgStartDate + "', " +
                           "PkgAgencyCommission = " + "'" + packages.PkgAgencyCommission + "' " +
                           "WHERE PackageId = " + pkgid;
            //connection to the DB
            SqlConnection connection = DBConnection.GetConnection();
            //sending query and connection to DB
            SqlCommand cmd = new SqlCommand(query, connection);
            try
            {
                // open a connection
                connection.Open();
                //execute query
                cmd.ExecuteNonQuery();
                MessageBox.Show("Update successful");
            }
            catch (Exception ex)
            {
                MessageBox.Show("The error is " + ex.Message, ex.GetType().ToString());
            }
            finally
            {
                // close the connection
                connection.Close();
            }
        }
        public static void InsertPackages(Packages packages)
        {
            SqlConnection con = DBConnection.GetConnection();
            string sql = "INSERT Packages (" +
                               "PkgName," +
                               "PkgDesc," +
                               "PkgBasePrice," +
                               "PkgAgencyCommission," +
                               "PkgStartDate," +
                               "PkgEndDate)" +
                        "VALUES (" +
                               "@PkgName," +
                               "@PkgDesc," +
                               "@PkgBasePrice," +
                               "@PkgAgencyCommission," +
                               "@PkgStartDate," +
                               "@PkgEndDate);";
            SqlCommand cmdInsert = new SqlCommand(sql, con);
            cmdInsert.Parameters.AddWithValue("@PkgName", packages.PkgName);
            cmdInsert.Parameters.AddWithValue("@PkgDesc", packages.PkgDesc);
            cmdInsert.Parameters.AddWithValue("@PkgBasePrice", packages.PkgBasePrice);
            cmdInsert.Parameters.AddWithValue("@PkgAgencyCommission", packages.PkgAgencyCommission);
            cmdInsert.Parameters.AddWithValue("@PkgStartDate", packages.PkgStartDate);
            cmdInsert.Parameters.AddWithValue("@PkgEndDate", packages.PkgEndDate);
            try
            {
                con.Open();
                cmdInsert.ExecuteNonQuery();
                //MessageBox.Show("Add package successful");
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
        public static void DeletePackage(int PackageID)
        {
            SqlConnection con = DBConnection.GetConnection();
            string sql =
                "DELETE FROM Packages_Products_Suppliers " +
                "WHERE PackageID = @PackageID " +
                "DELETE FROM Packages " +
                "WHERE PackageID = @PackageID;";
            SqlCommand cmdDelete = new SqlCommand(sql, con);
            cmdDelete.Parameters.AddWithValue("@PackageID", PackageID);
            try
            {
                con.Open();
                cmdDelete.ExecuteNonQuery();
                MessageBox.Show("Package Delete successful");
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