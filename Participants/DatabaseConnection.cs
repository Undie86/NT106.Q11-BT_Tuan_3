using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace Participants
{
    public static class DatabaseConnection
    {
        private static readonly string connectionString = "Data Source=NGUYENDUCKHIEM\\SQLEXPRESS;Initial Catalog=LMS;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";
        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}
