using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows.Forms;
using System.Diagnostics;

namespace CalendarProject
{
    public partial class WelcomeScreen : Form
    {
        public WelcomeScreen()
        {
            InitializeComponent();
        }

        // form designer code here instead of designer.cs auto generated stuff
        // might break because I moved it but it was irritating
        private void InitializeComponent()
        {
            this.lblUsername = new System.Windows.Forms.Label();
            this.lblPassword = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.btnLogin = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.SuspendLayout();

            // lblTitle
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Arial", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(100, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(200, 26);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Calendar System";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.TopCenter;

            // lblUsername
            this.lblUsername.AutoSize = true;
            this.lblUsername.Location = new System.Drawing.Point(50, 80);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(58, 13);
            this.lblUsername.TabIndex = 1;
            this.lblUsername.Text = "Username:";

            // lblPassword
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(50, 120);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(56, 13);
            this.lblPassword.TabIndex = 2;
            this.lblPassword.Text = "Password:";

            // txtUsername
            this.txtUsername.Location = new System.Drawing.Point(140, 77);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(180, 20);
            this.txtUsername.TabIndex = 3;

            // txtPassword
            this.txtPassword.Location = new System.Drawing.Point(140, 117);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(180, 20);
            this.txtPassword.TabIndex = 4;

            // btnLogin
            this.btnLogin.Location = new System.Drawing.Point(140, 170);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(100, 30);
            this.btnLogin.TabIndex = 5;
            this.btnLogin.Text = "Login";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);

            // WelcomeScreen
            this.AcceptButton = this.btnLogin;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(380, 230);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.txtUsername);
            this.Controls.Add(this.lblPassword);
            this.Controls.Add(this.lblUsername);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "WelcomeScreen";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Calendar Login";
            this.Load += new System.EventHandler(this.WelcomeScreen_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Label lblTitle;


        private void WelcomeScreen_Load(object sender, EventArgs e)
        {
            try
            {
                // Initialize database (just connects, doesn't create tables)
                Debug.WriteLine("Initializing database connection...");
                Database.InitializeDatabase();

                // Check existing users
                List<User> users = User.GetAllUsers();
                Debug.WriteLine($"Found {users.Count} existing users in database");

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in WelcomeScreen_Load: {ex.Message}");
                MessageBox.Show($"Error connecting to database: {ex.Message}",
                               "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // old code
            //try
            //{
            //    // test first
            //    if (!Database.TestConnection())
            //    {
            //        MessageBox.Show("Could not connect to the database. Please check your settings.",
            //                       "Database Connection Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        Application.Exit();
            //        return;
            //    }
            //    Debug.WriteLine("Connection Successful. Initializing Database...");
            //    Database.InitializeDatabase();
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show($"Error initializing database: {ex.Message}\n\nThe application will now close.",
            //   "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    Application.Exit();
            //}

            //// check if db empty
            //List<User> users = User.GetAllUsers();
            //Debug.WriteLine($"Found {users.Count} users.");

            //if (users.Count == 0)
            //{
            //    Debug.WriteLine("No users found, creating test users...");
            //    CreateTestUsers();
            //    users = User.GetAllUsers();
            //    foreach (var user in users)
            //    {
            //        Debug.WriteLine($"Created user: {user.FirstName} {user.LastName}\n" +
            //            $"Username: {user.Username}\n" +
            //            $"Password: {user.Password}"); 
            //    } 
                    

            //}
        }

        // test users/events
        //private void CreateTestUsers()
        //{
        //    User john = new User("John", "Doe", "password1");
        //    User jane = new User("Jane", "Smith", "password2");
        //    User manager = new User("Joe", "Shmoe", "manager3");
        //    manager.IsManager = true;
        //    User.AddUser(john);
        //    User.AddUser(jane);
        //    User.AddUser(manager);

        //    // John's events
        //    Event johnEvent1 = new Event(
        //        "Dentist Appointment",
        //        DateTime.Now.Date.AddDays(2).AddHours(10),
        //        DateTime.Now.Date.AddDays(2).AddHours(11),
        //        "Wisdom Teefs"
        //    );
        //    johnEvent1.Owner = john.Username;

        //    Event johnEvent2 = new Event(
        //        "PT",
        //        DateTime.Now.Date.AddDays(1).AddHours(17),
        //        DateTime.Now.Date.AddDays(1).AddHours(18.5)
        //    );
        //    johnEvent2.Owner = john.Username;

        //    // Jane's event
        //    Event janeEvent = new Event(
        //        "Project Deadline",
        //        DateTime.Now.Date.AddDays(5).AddHours(9),
        //        DateTime.Now.Date.AddDays(5).AddHours(17)
        //    );
        //    janeEvent.Owner = jane.Username;

        //    // Joe's event
        //    Event managerEvent = new Event(
        //        "Budget Review",
        //        DateTime.Now.Date.AddDays(3).AddHours(13),
        //        DateTime.Now.Date.AddDays(3).AddHours(15),
        //        "Q2 budget review"
        //    );
        //    managerEvent.Owner = manager.Username;

        //    // Team Meeting
        //    Meeting teamMeeting = new Meeting(
        //        "Team Status Update",
        //        DateTime.Now.Date.AddDays(2).AddHours(14),
        //        DateTime.Now.Date.AddDays(2).AddHours(15),
        //        manager.Username,
        //        new List<string> { john.Username, jane.Username },
        //        "Conference Room",
        //        "Weekly status meeting"
        //    );

        //    // Add all events
        //    john.AddEvent(johnEvent1);
        //    john.AddEvent(johnEvent2);
        //    john.AddEvent(teamMeeting);

        //    jane.AddEvent(janeEvent);
        //    jane.AddEvent(teamMeeting);

        //    manager.AddEvent(managerEvent);
        //    manager.AddEvent(teamMeeting);

        //    // debugging
        //    Debug.WriteLine("Created test users/events.");
        //}

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            User authenticatedUser = User.Login(username, password);

            if (authenticatedUser != null)
            {
                // more debugging
                Debug.WriteLine($"User {authenticatedUser.Username} logged in successfully");
                Debug.WriteLine($"Is manager: {authenticatedUser.IsManager}");
                authenticatedUser.LoadEvents();
                Debug.WriteLine($"Loaded {authenticatedUser.Calendar.Count} events for user.");

                // hide form and show calendar
                this.Hide();
                CalendarView calendarView = new CalendarView(authenticatedUser);

                calendarView.FormClosed += (s, args) => {
                    // log out when the form is closed
                    authenticatedUser.Logout();
                    this.Close();
                };

                calendarView.Show();
            }
            else
            {
                // failed
                MessageBox.Show("Login failed. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPassword.Clear();
                txtPassword.Focus();
            }
        }
    }
}