namespace CalendarProject
{
    partial class CalendarView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tableLayoutPanel1 = new TableLayoutPanel();
            createMeetingButton = new Button();
            addEventButton = new Button();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            label7 = new Label();
            label8 = new Label();
            nextMonthButton = new Button();
            previousMonthButton = new Button();
            label9 = new Label();
            eventPanel = new Panel();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 10;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 11F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 11F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 11F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 11F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 11F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 11F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 11F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 13F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 5F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 5F));
            tableLayoutPanel1.Controls.Add(createMeetingButton, 8, 0);
            tableLayoutPanel1.Controls.Add(addEventButton, 9, 0);
            tableLayoutPanel1.Controls.Add(label1, 0, 1);
            tableLayoutPanel1.Controls.Add(label2, 1, 1);
            tableLayoutPanel1.Controls.Add(label3, 2, 1);
            tableLayoutPanel1.Controls.Add(label4, 3, 1);
            tableLayoutPanel1.Controls.Add(label5, 4, 1);
            tableLayoutPanel1.Controls.Add(label6, 5, 1);
            tableLayoutPanel1.Controls.Add(label7, 6, 1);
            tableLayoutPanel1.Controls.Add(label8, 1, 0);
            tableLayoutPanel1.Controls.Add(nextMonthButton, 6, 0);
            tableLayoutPanel1.Controls.Add(previousMonthButton, 0, 0);
            tableLayoutPanel1.Controls.Add(label9, 7, 0);
            tableLayoutPanel1.Controls.Add(eventPanel, 7, 1);
            tableLayoutPanel1.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
            tableLayoutPanel1.Location = new Point(12, 12);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 8;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 7.4850297F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 7.4850297F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 14.1716576F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 14.1716576F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 14.1716576F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 14.1716576F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 14.1716576F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 14.1716576F));
            tableLayoutPanel1.Size = new Size(776, 504);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // createMeetingButton
            // 
            createMeetingButton.BackColor = Color.Azure;
            createMeetingButton.Dock = DockStyle.Right;
            createMeetingButton.Font = new Font("Arial Rounded MT Bold", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            createMeetingButton.Location = new Point(698, 3);
            createMeetingButton.Name = "createMeetingButton";
            createMeetingButton.Size = new Size(32, 31);
            createMeetingButton.TabIndex = 14;
            createMeetingButton.Text = "+M";
            createMeetingButton.UseVisualStyleBackColor = false;
            createMeetingButton.Click += createMeetingButton_Click;
            // 
            // addEventButton
            // 
            addEventButton.BackColor = Color.Azure;
            addEventButton.Dock = DockStyle.Right;
            addEventButton.Font = new Font("Arial Rounded MT Bold", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            addEventButton.Location = new Point(736, 3);
            addEventButton.Name = "addEventButton";
            addEventButton.Size = new Size(37, 31);
            addEventButton.TabIndex = 13;
            addEventButton.Text = "+E";
            addEventButton.UseVisualStyleBackColor = false;
            addEventButton.Click += AddEventButton_Click;
            // 
            // label1
            // 
            label1.BackColor = Color.AliceBlue;
            label1.Dock = DockStyle.Fill;
            label1.Font = new Font("Arial Rounded MT Bold", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(3, 37);
            label1.Name = "label1";
            label1.Size = new Size(79, 37);
            label1.TabIndex = 0;
            label1.Text = "Sunday";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            label2.BackColor = Color.AliceBlue;
            label2.Dock = DockStyle.Fill;
            label2.Font = new Font("Arial Rounded MT Bold", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.Location = new Point(88, 37);
            label2.Name = "label2";
            label2.Size = new Size(79, 37);
            label2.TabIndex = 1;
            label2.Text = "Monday";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            label3.BackColor = Color.AliceBlue;
            label3.Dock = DockStyle.Fill;
            label3.Font = new Font("Arial Rounded MT Bold", 9F);
            label3.Location = new Point(173, 37);
            label3.Name = "label3";
            label3.Size = new Size(79, 37);
            label3.TabIndex = 2;
            label3.Text = "Tuesday";
            label3.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            label4.BackColor = Color.AliceBlue;
            label4.Dock = DockStyle.Fill;
            label4.Font = new Font("Arial Rounded MT Bold", 9F);
            label4.Location = new Point(258, 37);
            label4.Name = "label4";
            label4.Size = new Size(79, 37);
            label4.TabIndex = 3;
            label4.Text = "Wednesday";
            label4.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            label5.BackColor = Color.AliceBlue;
            label5.Dock = DockStyle.Fill;
            label5.Font = new Font("Arial Rounded MT Bold", 9F);
            label5.Location = new Point(343, 37);
            label5.Name = "label5";
            label5.Size = new Size(79, 37);
            label5.TabIndex = 4;
            label5.Text = "Thursday";
            label5.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label6
            // 
            label6.BackColor = Color.AliceBlue;
            label6.Dock = DockStyle.Fill;
            label6.Font = new Font("Arial Rounded MT Bold", 9F);
            label6.Location = new Point(428, 37);
            label6.Name = "label6";
            label6.Size = new Size(79, 37);
            label6.TabIndex = 5;
            label6.Text = "Friday";
            label6.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label7
            // 
            label7.BackColor = Color.AliceBlue;
            label7.Dock = DockStyle.Fill;
            label7.Font = new Font("Arial Rounded MT Bold", 9F);
            label7.Location = new Point(513, 37);
            label7.Name = "label7";
            label7.Size = new Size(79, 37);
            label7.TabIndex = 6;
            label7.Text = "Saturday";
            label7.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label8
            // 
            label8.BackColor = Color.AliceBlue;
            tableLayoutPanel1.SetColumnSpan(label8, 5);
            label8.Dock = DockStyle.Fill;
            label8.Font = new Font("Arial Rounded MT Bold", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label8.Location = new Point(88, 0);
            label8.Name = "label8";
            label8.Size = new Size(419, 37);
            label8.TabIndex = 7;
            label8.Text = "Month 1889";
            label8.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // nextMonthButton
            // 
            nextMonthButton.Anchor = AnchorStyles.Right;
            nextMonthButton.BackColor = Color.Azure;
            nextMonthButton.Font = new Font("Arial Rounded MT Bold", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            nextMonthButton.Location = new Point(549, 3);
            nextMonthButton.Name = "nextMonthButton";
            nextMonthButton.Size = new Size(43, 30);
            nextMonthButton.TabIndex = 8;
            nextMonthButton.Text = ">";
            nextMonthButton.UseVisualStyleBackColor = false;
            nextMonthButton.Click += nextMonthButton_Click;
            // 
            // previousMonthButton
            // 
            previousMonthButton.Anchor = AnchorStyles.Left;
            previousMonthButton.BackColor = Color.Azure;
            previousMonthButton.Font = new Font("Arial Rounded MT Bold", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            previousMonthButton.Location = new Point(3, 3);
            previousMonthButton.Name = "previousMonthButton";
            previousMonthButton.Size = new Size(43, 30);
            previousMonthButton.TabIndex = 9;
            previousMonthButton.Text = "<";
            previousMonthButton.UseVisualStyleBackColor = false;
            previousMonthButton.Click += previousMonthButton_Click;
            // 
            // label9
            // 
            label9.BackColor = Color.AliceBlue;
            label9.Dock = DockStyle.Left;
            label9.Font = new Font("Arial Rounded MT Bold", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label9.Location = new Point(620, 0);
            label9.Margin = new Padding(25, 0, 3, 0);
            label9.Name = "label9";
            label9.Size = new Size(64, 37);
            label9.TabIndex = 11;
            label9.Text = "Events";
            label9.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // eventPanel
            // 
            tableLayoutPanel1.SetColumnSpan(eventPanel, 3);
            eventPanel.Location = new Point(620, 40);
            eventPanel.Margin = new Padding(25, 3, 3, 3);
            eventPanel.Name = "eventPanel";
            tableLayoutPanel1.SetRowSpan(eventPanel, 7);
            eventPanel.Size = new Size(153, 461);
            eventPanel.TabIndex = 10;
            // 
            // CalendarView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.AliceBlue;
            ClientSize = new Size(800, 528);
            Controls.Add(tableLayoutPanel1);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            Name = "CalendarView";
            Text = "Calendar";
            Load += CalendarView_Load;
            tableLayoutPanel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        private void AddEventButton_Click(object sender, EventArgs e)
        {
            // I'm bad at sticking to naming conventions
            AddEvent_Click();
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label label7;
        private Label label8;
        private Button nextMonthButton;
        private Button previousMonthButton;
        private Panel EventPanel;
        private Label label9;
        private Button addEventButton;
        private Panel eventPanel;
        private Button createMeetingButton;
    }
}