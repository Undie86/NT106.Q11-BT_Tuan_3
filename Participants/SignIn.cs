using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Participants
{
    public partial class SignIn : Form
    {
        private int failedAttempts = 0;              // number of failed attemps
        private int maxAttempts = 5;                 // max try
        private DateTime lockoutEndTime = DateTime.MinValue;
        private int lockoutSeconds = 60;             // lock time
        private bool passwordVisible = false;

        public SignIn()
        {
            InitializeComponent();
            CustomizeComponents();
            SetupPlaceholderText();
            ApplyRoundedCorners();
            rightPanel.Resize += rightPanel_Resize;
            rightPanel_Resize(this, EventArgs.Empty);
            signUpButton.Click += SignUpButton_Click;
        }

        private void SignUpButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            SignUp signUp = new SignUp();
            signUp.FormClosed += (s, args) => this.Close();
            signUp.Show();
        }

        private void rightPanel_Resize(object sender, EventArgs e)
        {
            mainPanel.Left = (rightPanel.Width - mainPanel.Width) / 2;
            mainPanel.Top = (rightPanel.Height - mainPanel.Height) / 2;
        }
        private void CustomizeComponents()
        {
            // Change these settings to allow resizing
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            
            // Apply rounded corners to text boxes
            ApplyRoundedCornersToTextBoxes();
            
            signInButton.FlatStyle = FlatStyle.Flat;
            signInButton.FlatAppearance.BorderSize = 0;
            signInButton.Cursor = Cursors.Hand;
            signUpButton.FlatStyle = FlatStyle.Flat;
            signUpButton.FlatAppearance.BorderSize = 0;
            signUpButton.Cursor = Cursors.Hand;
            signUpButton.BackColor = Color.RoyalBlue;
            signUpButton.ForeColor = Color.White;
            signUpButton.Font = new Font("Segoe UI", 10F);
            forgotPasswordLink.Cursor = Cursors.Hand;
        }

        private void ApplyRoundedCornersToTextBoxes()
        {
            int radius = 10;
            
            // Apply rounded corners to text boxes
            usernameTextBox.Paint += (s, e) => DrawRoundedTextBox(usernameTextBox, e, radius);
            passwordTextBox.Paint += (s, e) => DrawRoundedTextBox(passwordTextBox, e, radius);
            
            // Set regions for rounded appearance
            using (var path = GetRoundedRectPath(new Rectangle(0, 0, usernameTextBox.Width, usernameTextBox.Height), radius))
            {
                usernameTextBox.Region = new Region(path);
            }
            using (var path = GetRoundedRectPath(new Rectangle(0, 0, passwordTextBox.Width, passwordTextBox.Height), radius))
            {
                passwordTextBox.Region = new Region(path);
            }
            
            // Handle resize events to maintain rounded corners
            usernameTextBox.Resize += (s, e) =>
            {
                using (var path = GetRoundedRectPath(new Rectangle(0, 0, usernameTextBox.Width, usernameTextBox.Height), radius))
                {
                    usernameTextBox.Region = new Region(path);
                }
            };
            passwordTextBox.Resize += (s, e) =>
            {
                using (var path = GetRoundedRectPath(new Rectangle(0, 0, passwordTextBox.Width, passwordTextBox.Height), radius))
                {
                    passwordTextBox.Region = new Region(path);
                }
            };
        }

        private void DrawRoundedTextBox(TextBox textBox, PaintEventArgs e, int radius)
        {
            using (var path = GetRoundedRectPath(new Rectangle(0, 0, textBox.Width, textBox.Height), radius))
            {
                textBox.Region = new Region(path);
            }
        }

        private void SetupPlaceholderText()
        {
            // Email placeholder
            usernameTextBox.Text = "Enter your email";
            usernameTextBox.ForeColor = Color.Gray;
            usernameTextBox.GotFocus += (s, e) =>
            {
                if (usernameTextBox.Text == "Enter your email")
                {
                    usernameTextBox.Text = "";
                    usernameTextBox.ForeColor = Color.Black;
                }
            };
            usernameTextBox.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(usernameTextBox.Text))
                {
                    usernameTextBox.Text = "Enter your email";
                    usernameTextBox.ForeColor = Color.Gray;
                }
            };
            // Password placeholder
            passwordTextBox.Text = "Enter your Password";
            passwordTextBox.ForeColor = Color.Gray;
            passwordTextBox.PasswordChar = '\0';
            passwordTextBox.GotFocus += (s, e) =>
            {
                if (passwordTextBox.Text == "Enter your Password")
                {
                    passwordTextBox.Text = "";
                    passwordTextBox.ForeColor = Color.Black;
                    passwordTextBox.PasswordChar = passwordVisible ? '\0' : '●';
                }
            };
            passwordTextBox.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(passwordTextBox.Text))
                {
                    passwordTextBox.Text = "Enter your Password";
                    passwordTextBox.ForeColor = Color.Gray;
                    passwordTextBox.PasswordChar = '\0';
                }
            };
        }

        private void ApplyRoundedCorners()
        {
            // Only apply rounded corners for buttons
            signInButton.Paint += (s, e) => DrawRoundedButton(signInButton, e);
            signUpButton.Paint += (s, e) => DrawRoundedButton(signUpButton, e);
        }

        private void DrawRoundedButton(Button btn, PaintEventArgs e)
        {
            int radius = 20;
            using (var path = GetRoundedRectPath(new Rectangle(0, 0, btn.Width, btn.Height), radius))
            {
                btn.Region = new Region(path);
            }
        }

        private GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }

        private void signInButton_Click(object sender, EventArgs e)
        {

            string username = usernameTextBox.Text.Trim();
            string password = passwordTextBox.Text.Trim();

            // Kiểm tra khóa tạm thời
            if (DateTime.Now < lockoutEndTime)
            {
                double secondsLeft = (lockoutEndTime - DateTime.Now).TotalSeconds;
                MessageBox.Show($"Your account has been locked for {Math.Ceiling(secondsLeft)} second(s).");
                return;
            }

            //  Tạo request gửi đến server
            string encryptedPassword = AESEncryption.Encrypt(password);
            string request = $"LOGIN|{username}|{encryptedPassword}";
           
            ServerConnection conn = new ServerConnection(); 
            string response = conn.SendRequest(request);

            // Phân tích phản hồi từ server
            string[] parts = response.Split('|');
            string status = parts[0];
            string message = parts.Length > 1 ? parts[1] : "Unknown response";

            if (status == "SUCCESS")
            {
                MessageBox.Show(message, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                failedAttempts = 0; // reset
            }
            else
            {
                failedAttempts++;

                if (failedAttempts >= maxAttempts)
                {
                    lockoutEndTime = DateTime.Now.AddSeconds(lockoutSeconds);
                    failedAttempts = 0;
                    MessageBox.Show($"Wrong password {maxAttempts} time(s). Your account has been locked for {lockoutSeconds} second(s).");
                }
                else
                {
                    int attemptsLeft = maxAttempts - failedAttempts;
                    MessageBox.Show($"Invalid login credentials. You have {attemptsLeft} more try.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

    }
}

