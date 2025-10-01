using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Participants
{
    public static class DatabaseConnection
    {
        private static readonly string connectionString = "Server=192.168.0.89,1436;Database=LMS;User Id=LMSUser;Password=1234;Encrypt=False";
        
        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }

        public static bool TestDatabaseConnection()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    MessageBox.Show("Kết nối thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi kết nối: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
        }
    }
}
