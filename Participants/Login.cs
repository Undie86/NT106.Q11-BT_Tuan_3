using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Participants
{
    public partial class Login : Form
    {
        private bool passwordVisible = false;

        public Login()
        {
            InitializeComponent();
            CustomizeComponents();
            SetupPlaceholderText();
            ApplyRoundedCorners();
            showPasswordButton.Click += ShowPasswordButton_Click;
            rightPanel.Resize += rightPanel_Resize;
            rightPanel_Resize(this, EventArgs.Empty);
        }

        private void rightPanel_Resize(object sender, EventArgs e)
        {
            mainPanel.Left = (rightPanel.Width - mainPanel.Width) / 2;
            mainPanel.Top = (rightPanel.Height - mainPanel.Height) / 2;
        }

        private void ShowPasswordButton_Click(object sender, EventArgs e)
        {
            passwordVisible = !passwordVisible;
            if (passwordVisible)
            {
                if (passwordTextBox.Text == "Enter your Password")
                {
                    passwordTextBox.PasswordChar = '\0';
                }
                else
                {
                    passwordTextBox.PasswordChar = '\0';
                }
                showPasswordButton.Text = "🙈";
            }
            else
            {
                if (passwordTextBox.Text == "Enter your Password")
                {
                    passwordTextBox.PasswordChar = '\0';
                }
                else
                {
                    passwordTextBox.PasswordChar = '●';
                }
                showPasswordButton.Text = "👁";
            }
        }

        private void CustomizeComponents()
        {
            // Change these settings to allow resizing
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            
            emailTextBox.BorderStyle = BorderStyle.None;
            passwordTextBox.BorderStyle = BorderStyle.None;
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
            showPasswordButton.FlatStyle = FlatStyle.Flat;
            showPasswordButton.FlatAppearance.BorderSize = 0;
            showPasswordButton.Text = "👁";
            showPasswordButton.Font = new Font("Segoe UI", 10F);
        }

        private void SetupPlaceholderText()
        {
            // Email placeholder
            emailTextBox.Text = "Enter your email";
            emailTextBox.ForeColor = Color.Gray;
            emailTextBox.GotFocus += (s, e) =>
            {
                if (emailTextBox.Text == "Enter your email")
                {
                    emailTextBox.Text = "";
                    emailTextBox.ForeColor = Color.Black;
                }
            };
            emailTextBox.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(emailTextBox.Text))
                {
                    emailTextBox.Text = "Enter your email";
                    emailTextBox.ForeColor = Color.Gray;
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
    }
}
