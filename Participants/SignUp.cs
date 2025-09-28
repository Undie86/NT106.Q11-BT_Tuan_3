using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Participants
{
    public partial class SignUp : Form
    {
        private bool passwordVisible = false;
        private bool rePasswordVisible = false;

        public SignUp()
        {
            InitializeComponent();
            CustomizeComponents();
            SetupPlaceholderText();
            ApplyRoundedCorners();
            signInButton.Click += SignInButton_Click;
            showPasswordButton.Click += ShowPasswordButton_Click;
            showRePasswordButton.Click += ShowRePasswordButton_Click;
            rightPanel.Resize += RightPanel_Resize;
            RightPanel_Resize(this, EventArgs.Empty);
        }

        private void RightPanel_Resize(object sender, EventArgs e)
        {
            mainPanel.Left = (rightPanel.Width - mainPanel.Width) / 2;
            mainPanel.Top = (rightPanel.Height - mainPanel.Height) / 2;
        }

        private void ShowPasswordButton_Click(object sender, EventArgs e)
        {
            passwordVisible = !passwordVisible;
            if (passwordVisible)
            {
                if (passwordTextBox.Text == "Enter your password")
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
                if (passwordTextBox.Text == "Enter your password")
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

        private void ShowRePasswordButton_Click(object sender, EventArgs e)
        {
            rePasswordVisible = !rePasswordVisible;
            if (rePasswordVisible)
            {
                if (rePasswordTextBox.Text == "Re-enter your password")
                {
                    rePasswordTextBox.PasswordChar = '\0';
                }
                else
                {
                    rePasswordTextBox.PasswordChar = '\0';
                }
                showRePasswordButton.Text = "🙈";
            }
            else
            {
                if (rePasswordTextBox.Text == "Re-enter your password")
                {
                    rePasswordTextBox.PasswordChar = '\0';
                }
                else
                {
                    rePasswordTextBox.PasswordChar = '●';
                }
                showRePasswordButton.Text = "👁";
            }
        }

        private void CustomizeComponents()
        {
            // Change these settings to allow resizing
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;

            // Apply rounded corners to text boxes
            ApplyRoundedCornersToTextBoxes();

            // Customize buttons
            signUpButton.FlatStyle = FlatStyle.Flat;
            signUpButton.FlatAppearance.BorderSize = 0;
            signUpButton.Cursor = Cursors.Hand;
            signInButton.FlatStyle = FlatStyle.Flat;
            signInButton.FlatAppearance.BorderSize = 0;
            signInButton.Cursor = Cursors.Hand;
            signInButton.BackColor = Color.RoyalBlue;
            signInButton.ForeColor = Color.White;
            signInButton.Font = new Font("Segoe UI", 10F);

            // Customize show password buttons
            showPasswordButton.FlatStyle = FlatStyle.Flat;
            showPasswordButton.FlatAppearance.BorderSize = 0;
            showPasswordButton.Text = "👁";
            showPasswordButton.Font = new Font("Segoe UI", 10F);
            showRePasswordButton.FlatStyle = FlatStyle.Flat;
            showRePasswordButton.FlatAppearance.BorderSize = 0;
            showRePasswordButton.Text = "👁";
            showRePasswordButton.Font = new Font("Segoe UI", 10F);
        }

        private void ApplyRoundedCornersToTextBoxes()
        {
            int radius = 10;
            
            // Apply rounded corners to all text boxes
            TextBox[] textBoxes = { fullnameTextBox, usernameTextBox, emailTextBox, passwordTextBox, rePasswordTextBox, phoneTextBox };
            
            foreach (TextBox textBox in textBoxes)
            {
                textBox.Paint += (s, e) => DrawRoundedTextBox(textBox, e, radius);
                
                // Set initial region for rounded appearance
                using (var path = GetRoundedRectPath(new Rectangle(0, 0, textBox.Width, textBox.Height), radius))
                {
                    textBox.Region = new Region(path);
                }
                
                // Handle resize events to maintain rounded corners
                textBox.Resize += (s, e) =>
                {
                    using (var path = GetRoundedRectPath(new Rectangle(0, 0, textBox.Width, textBox.Height), radius))
                    {
                        textBox.Region = new Region(path);
                    }
                };
            }
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
            // Fullname placeholder
            fullnameTextBox.Text = "Enter your full name";
            fullnameTextBox.ForeColor = Color.Gray;
            fullnameTextBox.GotFocus += (s, e) =>
            {
                if (fullnameTextBox.Text == "Enter your full name")
                {
                    fullnameTextBox.Text = "";
                    fullnameTextBox.ForeColor = Color.Black;
                }
            };
            fullnameTextBox.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(fullnameTextBox.Text))
                {
                    fullnameTextBox.Text = "Enter your full name";
                    fullnameTextBox.ForeColor = Color.Gray;
                }
            };

            // Username placeholder
            usernameTextBox.Text = "Enter your username";
            usernameTextBox.ForeColor = Color.Gray;
            usernameTextBox.GotFocus += (s, e) =>
            {
                if (usernameTextBox.Text == "Enter your username")
                {
                    usernameTextBox.Text = "";
                    usernameTextBox.ForeColor = Color.Black;
                }
            };
            usernameTextBox.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(usernameTextBox.Text))
                {
                    usernameTextBox.Text = "Enter your username";
                    usernameTextBox.ForeColor = Color.Gray;
                }
            };

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
            passwordTextBox.Text = "Enter your password";
            passwordTextBox.ForeColor = Color.Gray;
            passwordTextBox.PasswordChar = '\0';
            passwordTextBox.GotFocus += (s, e) =>
            {
                if (passwordTextBox.Text == "Enter your password")
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
                    passwordTextBox.Text = "Enter your password";
                    passwordTextBox.ForeColor = Color.Gray;
                    passwordTextBox.PasswordChar = '\0';
                }
            };

            // Re-enter password placeholder
            rePasswordTextBox.Text = "Re-enter your password";
            rePasswordTextBox.ForeColor = Color.Gray;
            rePasswordTextBox.PasswordChar = '\0';
            rePasswordTextBox.GotFocus += (s, e) =>
            {
                if (rePasswordTextBox.Text == "Re-enter your password")
                {
                    rePasswordTextBox.Text = "";
                    rePasswordTextBox.ForeColor = Color.Black;
                    rePasswordTextBox.PasswordChar = rePasswordVisible ? '\0' : '●';
                }
            };
            rePasswordTextBox.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(rePasswordTextBox.Text))
                {
                    rePasswordTextBox.Text = "Re-enter your password";
                    rePasswordTextBox.ForeColor = Color.Gray;
                    rePasswordTextBox.PasswordChar = '\0';
                }
            };

            // Phone number placeholder
            phoneTextBox.Text = "Enter your phone number";
            phoneTextBox.ForeColor = Color.Gray;
            phoneTextBox.GotFocus += (s, e) =>
            {
                if (phoneTextBox.Text == "Enter your phone number")
                {
                    phoneTextBox.Text = "";
                    phoneTextBox.ForeColor = Color.Black;
                }
            };
            phoneTextBox.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(phoneTextBox.Text))
                {
                    phoneTextBox.Text = "Enter your phone number";
                    phoneTextBox.ForeColor = Color.Gray;
                }
            };
        }

        private void ApplyRoundedCorners()
        {
            // Only apply rounded corners for buttons
            signUpButton.Paint += (s, e) => DrawRoundedButton(signUpButton, e);
            signInButton.Paint += (s, e) => DrawRoundedButton(signInButton, e);
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

        private void SignInButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            SignIn signIn = new SignIn();
            signIn.FormClosed += (s, args) => this.Close();
            signIn.Show();
        }
    }
}
