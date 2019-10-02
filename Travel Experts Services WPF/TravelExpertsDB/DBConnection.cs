using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelExpertsDB
{
    public static class DBConnection
    {
        public static SqlConnection GetConnection(/*string ServerName, string DatabaseName*/)
        {
            string ConnectionString = @"Data Source=oosd.database.windows.net;Initial Catalog=TravelExperts;User ID=ethan;Password=Travel2$19";

            SqlConnection connection = new SqlConnection(ConnectionString);

            return connection;
        }
    }
}
