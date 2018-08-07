using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Entregavel
{
    public class SqlConn
    {
        public static SqlConnection Abrir()
        {
            string str = System.Configuration.ConfigurationManager.ConnectionStrings["conexao"].ConnectionString;
            SqlConnection conn = new SqlConnection(str);
            conn.Open();
            return conn;
        }
    }
}