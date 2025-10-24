using System;
using System.Data.SqlClient;

namespace LMS_Server
{
    public class DatabaseManager
    {
        private readonly string connectionString;

        public DatabaseManager(string connStr)
        {
            connectionString = connStr;
        }

        // ✅ Đăng nhập — server tự verify password
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

                        // ✅ So sánh mật khẩu gốc (client gửi) với hash trong DB
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

        // ✅ Đăng ký — hash trước khi lưu
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

                    // ✅ Hash mật khẩu ở phía server
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

        // ✅ Quên mật khẩu — reset password, hash tại server
        public string ForgotPassword(string username, string email)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = "SELECT Username FROM LMSData WHERE Username = @Username AND Email = @Email";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Email", email);
                        object result = cmd.ExecuteScalar();

                        if (result == null || result == DBNull.Value)
                            return "NOT_FOUND";

                        // ✅ Tạo mật khẩu mới (8 ký tự ngẫu nhiên)
                        string newPass = Guid.NewGuid().ToString("N").Substring(0, 8);

                        // ✅ Hash lại mật khẩu mới
                        string newHash = PasswordHashing.HashPassword(newPass);

                        // ✅ Cập nhật DB
                        string update = "UPDATE LMSData SET Password = @Password WHERE Username = @Username";
                        using (SqlCommand updateCmd = new SqlCommand(update, conn))
                        {
                            updateCmd.Parameters.AddWithValue("@Password", newHash);
                            updateCmd.Parameters.AddWithValue("@Username", username);
                            updateCmd.ExecuteNonQuery();
                        }

                        // ✅ Trả lại mật khẩu mới dạng plain text để gửi cho client (hoặc email)
                        return newPass;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi ForgotPassword: " + ex.Message);
                return "ERROR";
            }
        }
    }
}
