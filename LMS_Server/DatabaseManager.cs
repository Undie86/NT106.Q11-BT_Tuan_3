using System;
using System.Data.SqlClient;

namespace LMS_Server
{
    public class DatabaseManager
    {
        private readonly string connectionString;
        public string ConnectionString => connectionString;

        public DatabaseManager(string connStr)
        {
            connectionString = connStr;
        }

        // Đăng nhập — server tự verify password
        public bool CheckLogin(string username, string password)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = "SELECT Password FROM LMSData WHERE Username = @Username";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        object result = cmd.ExecuteScalar();

                        if (result == null || result == DBNull.Value)
                            return false; // user không tồn tại

                        string storedHash = result.ToString();

                        // So sánh mật khẩu gốc (client gửi) với hash trong DB
                        return PasswordHashing.VerifyPassword(password, storedHash);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi CheckLogin: " + ex.Message);
                return false;
            }
        }

        // Đăng ký — hash trước khi lưu
        public bool RegisterUser(string username, string password, string email)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Kiểm tra user đã tồn tại chưa
                    string checkQuery = "SELECT COUNT(*) FROM LMSData WHERE Username = @Username";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@Username", username);
                        int count = (int)checkCmd.ExecuteScalar();

                        if (count > 0)
                            return false; // user đã tồn tại
                    }

                    // Hash mật khẩu ở phía server
                    string hashedPassword = PasswordHashing.HashPassword(password);

                    string insertQuery = "INSERT INTO LMSData (Username, Password, Email) VALUES (@Username, @Password, @Email)";
                    using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
                    {
                        insertCmd.Parameters.AddWithValue("@Username", username);
                        insertCmd.Parameters.AddWithValue("@Password", hashedPassword);
                        insertCmd.Parameters.AddWithValue("@Email", email);
                        insertCmd.ExecuteNonQuery();
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi RegisterUser: " + ex.Message);
                return false;
            }
        }

        public bool UpdatePassword(string email, string hashedPassword)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "UPDATE Users SET Password = @Password WHERE Email = @Email";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Password", hashedPassword);
                        cmd.Parameters.AddWithValue("@Email", email);
                        int rows = cmd.ExecuteNonQuery();
                        return rows > 0;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }


    }
}
