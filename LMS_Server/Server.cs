using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Data.SqlClient;

namespace LMS_Server
{
    public class Server
    {
        private TcpListener listener;
        private const int PORT = 8080;
        private DatabaseManager db;

        public Server()
        {
            string connStr = @"Data Source=NGUYENDUCKHIEM\SQLEXPRESS;Initial Catalog=LMS;Integrated Security=True;TrustServerCertificate=True";
            db = new DatabaseManager(connStr);
        }

        public void Start()
        {
            listener = new TcpListener(IPAddress.Any, PORT);
            listener.Start();
            Console.WriteLine("Server is running on port " + PORT);

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                Console.WriteLine("Client connected: " + client.Client.RemoteEndPoint);

                Thread t = new Thread(HandleClient);
                t.Start(client);
            }
        }

        private void HandleClient(object obj)
        {
            TcpClient client = (TcpClient)obj;
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[2048];

            try
            {
                while (true)
                {
                    int count = stream.Read(buffer, 0, buffer.Length);
                    if (count == 0) break;

                    string request = Encoding.UTF8.GetString(buffer, 0, count);
                    Console.WriteLine("Request from client: " + request);

                    string response = ProcessRequest(request);
                    byte[] respBytes = Encoding.UTF8.GetBytes(response);
                    stream.Write(respBytes, 0, respBytes.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                client.Close();
            }
        }

        private string ProcessRequest(string request)
        {
            string[] parts = request.Split('|');
            string cmd = parts[0].ToUpper();

            switch (cmd)
            {
                case "LOGIN":
                    return HandleLogin(parts[1], parts[2]); // username, password gốc

                case "REGISTER":
                    return HandleRegister(parts[1], parts[2], parts[3]); // username, password gốc, email

                case "FORGOT":
                    return HandleForgotPassword(parts[1], parts[2]); // username, email

                default:
                    return "FAIL|Lệnh không hợp lệ";
            }
        }

        // ✅ Đăng nhập: server tự verify hash
        private string HandleLogin(string username, string password)
        {
            try
            {
                bool isValid = db.CheckLogin(username, password);
                return isValid ? "SUCCESS|Đăng nhập thành công" : "FAIL|Sai tên đăng nhập hoặc mật khẩu";
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi đăng nhập: " + ex.Message);
                return "FAIL|Server error: " + ex.Message;
            }
        }

        // ✅ Đăng ký: server hash password trước khi lưu
        private string HandleRegister(string username, string password, string email)
        {
            try
            {
                string hashedPassword = PasswordHashing.HashPassword(password);

                using (SqlConnection conn = new SqlConnection(@"Data Source=NGUYENDUCKHIEM\SQLEXPRESS;Initial Catalog=LMS;Integrated Security=True;TrustServerCertificate=True"))
                {
                    conn.Open();

                    // Kiểm tra username tồn tại
                    string checkQuery = "SELECT COUNT(*) FROM LMSData WHERE Username = @Username";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@Username", username);
                        int exists = (int)checkCmd.ExecuteScalar();
                        if (exists > 0)
                            return "FAIL|Tên đăng nhập đã tồn tại";
                    }

                    // Lưu tài khoản mới
                    string insertQuery = "INSERT INTO LMSData (Username, Password, Email) VALUES (@Username, @Password, @Email)";
                    using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
                    {
                        insertCmd.Parameters.AddWithValue("@Username", username);
                        insertCmd.Parameters.AddWithValue("@Password", hashedPassword);
                        insertCmd.Parameters.AddWithValue("@Email", email);
                        insertCmd.ExecuteNonQuery();
                    }

                    return "SUCCESS|Đăng ký thành công";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi đăng ký: " + ex.Message);
                return "FAIL|Server error: " + ex.Message;
            }
        }

        // ✅ Quên mật khẩu: server tự tạo mật khẩu mới, hash trước khi lưu
        private string HandleForgotPassword(string username, string email)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(@"Data Source=NGUYENDUCKHIEM\SQLEXPRESS;Initial Catalog=LMS;Integrated Security=True;TrustServerCertificate=True"))
                {
                    conn.Open();

                    string checkQuery = "SELECT COUNT(*) FROM LMSData WHERE Username = @Username AND Email = @Email";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@Username", username);
                        checkCmd.Parameters.AddWithValue("@Email", email);

                        int exists = (int)checkCmd.ExecuteScalar();
                        if (exists == 0)
                            return "FAIL|Không tìm thấy tài khoản phù hợp";
                    }

                    //  Tạo mật khẩu ngẫu nhiên mới
                    string newPasswordPlain = GenerateRandomPassword();
                    string newPasswordHash = PasswordHashing.HashPassword(newPasswordPlain);

                    //  Cập nhật DB
                    string updateQuery = "UPDATE LMSData SET Password = @Password WHERE Username = @Username";
                    using (SqlCommand updateCmd = new SqlCommand(updateQuery, conn))
                    {
                        updateCmd.Parameters.AddWithValue("@Password", newPasswordHash);
                        updateCmd.Parameters.AddWithValue("@Username", username);
                        updateCmd.ExecuteNonQuery();
                    }

                    // Trong thực tế: gửi email newPasswordPlain
                    return $"SUCCESS|Mật khẩu mới của bạn là: {newPasswordPlain}";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi quên mật khẩu: " + ex.Message);
                return "FAIL|Server error: " + ex.Message;
            }
        }

        //  Hàm tạo mật khẩu ngẫu nhiên
        private string GenerateRandomPassword(int length = 8)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random rnd = new Random();
            char[] buffer = new char[length];
            for (int i = 0; i < length; i++)
                buffer[i] = chars[rnd.Next(chars.Length)];
            return new string(buffer);
        }
    }
}
