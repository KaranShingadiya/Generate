using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Dyanamic_Time_Table_Generator
{
    public class Connection
    {
        public static SqlConnection con;
        public static void getConnection()
        {
            con = new SqlConnection(ConfigurationManager.ConnectionStrings["con"].ConnectionString);
        }

    }
}