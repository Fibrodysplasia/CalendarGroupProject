using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Diagnostics;

namespace CalendarProject
{
    public partial class CalendarView : Form
    {
        // Calendar variables
        private User currentUser;
        private Panel[,] dayCells;
        private int currentMonth = DateTime.Now.Month;
        private int currentYear = DateTime.Now.Year;
        private Panel selectedDayCell = null;
        private DateTime selectedDate;

        public CalendarView(User user)
        {
            currentUser = user;
            InitializeComponent();
            // manager check
            createMeetingButton.Visible = (user != null && user.IsManager);
        }

        // generate calendar when the form loads
        private void CalendarView_Load(object sender, EventArgs e)
        {
            this.Text = $"Calendar - {currentUser.FullName}";
            InitializeCalendar();
            GenerateCalendar(currentYear, currentMonth);
            DisplayMonthEvents();
        }

        private void InitializeCalendar()
        {
            // 6 weeks x 7 days
            dayCells = new Panel[6, 7];

            // add day cells to TableLayoutPanel
            for (int row = 0; row < 6; row++)
            {
                for (int col = 0; col < 7; col++)
                {
                    // panel for each day
                    Panel dayCell = new Panel();
                    dayCell.BorderStyle = BorderStyle.FixedSingle;
                    dayCell.BackColor = Color.White;
                    dayCell.Dock = DockStyle.Fill;
                    dayCell.Margin = new Padding(1);
                    dayCell.Tag = new Point(row, col); // for later reference

                    // day number
                    Label dayLabel = new Label();
                    dayLabel.Font = new Font("Arial", 10F, FontStyle.Bold);
                    dayLabel.ForeColor = Color.Black;
                    dayLabel.Location = new Point(5, 5);
                    dayLabel.AutoSize = true;
                    dayLabel.Name = "dayLabel";
                    dayCell.Controls.Add(dayLabel);

                    // event area
                    Panel eventArea = new Panel();
                    eventArea.Location = new Point(5, 25);
                    eventArea.Size = new Size(dayCell.Width - 10, dayCell.Height - 30);
                    eventArea.AutoScroll = true;
                    eventArea.Name = "eventArea";
                    dayCell.Controls.Add(eventArea);

                    // event handler for clicking days / labels
                    dayCell.Click += DayCell_Click;
                    dayLabel.Click += DayCell_Click;

                    // store dayCell in array
                    dayCells[row, col] = dayCell;

                    // adds starting from row 2 (after headers)
                    tableLayoutPanel1.Controls.Add(dayCell, col, row + 2);
                }
            }
        }

        // generate current month's calendar
        private void GenerateCalendar(int year, int month)
        {
            // update header / get info
            DateTime displayDate = new DateTime(year, month, 1);
            label8.Text = displayDate.ToString("MMMM yyyy");
            DateTime firstDayOfMonth = new DateTime(year, month, 1);
            int startDayOfWeek = (int)firstDayOfMonth.DayOfWeek;
            int daysInMonth = DateTime.DaysInMonth(year, month);

            // clear all cells
            for (int row = 0; row < 6; row++)
            {
                for (int col = 0; col < 7; col++)
                {
                    Panel dayCell = dayCells[row, col];
                    Label dayLabel = (Label)dayCell.Controls["dayLabel"];

                    // clear label and events
                    dayLabel.Text = "";
                    dayCell.BackColor = Color.White;
                    Panel eventArea = (Panel)dayCell.Controls["eventArea"];
                    eventArea.Controls.Clear();
                }
            }

            // label days
            int dayCounter = 1;

            for (int row = 0; row < 6; row++)
            {
                for (int col = 0; col < 7; col++)
                {
                    // skip to first day of the month
                    if (row == 0 && col < startDayOfWeek)
                    {
                        // grey out days from prior month (beginning of calendar)
                        dayCells[row, col].BackColor = Color.WhiteSmoke;
                        continue;
                    }

                    // grey out days from next month (end of calendar)
                    if (dayCounter > daysInMonth)
                    {
                        dayCells[row, col].BackColor = Color.WhiteSmoke;
                        continue;
                    }

                    // set numbers
                    Label dayLabel = (Label)dayCells[row, col].Controls["dayLabel"];
                    dayLabel.Text = dayCounter.ToString();

                    // date for cell
                    DateTime cellDate = new DateTime(year, month, dayCounter);
                    dayCells[row, col].Tag = cellDate;

                    // highlight weekends
                    if (col == 0 || col == 6)
                    {
                        dayCells[row, col].BackColor = Color.Azure;
                    }

                    // highlight today
                    if (cellDate.Date == DateTime.Today)
                    {
                        dayCells[row, col].BackColor = Color.LightGreen;
                    }

                    // show events for this day
                    DisplayEventsForDay(dayCells[row, col], cellDate);

                    dayCounter++;
                }
            }
        }

        // display events for a specific day
        private void DisplayEventsForDay(Panel dayCell, DateTime date)
        {
            if (currentUser == null || currentUser.Calendar == null || dayCell == null)
            {
                return;
            }

            // get eventArea
            Panel eventArea = null;
            foreach (Control control in dayCell.Controls)
            {
                if (control is Panel && control.Name == "eventArea")
                {
                    eventArea = (Panel)control;
                    break;
                }
            }
            if (eventArea == null)
            {
                // create eventArea if it doesn't exist
                eventArea = new Panel();
                eventArea.Location = new Point(5, 25);
                eventArea.Size = new Size(dayCell.Width - 10, dayCell.Height - 30);
                eventArea.AutoScroll = true;
                eventArea.Name = "eventArea";
                dayCell.Controls.Add(eventArea);
            }

            // update and display
            eventArea.Controls.Clear();
            List<Event> dayEvents = currentUser.GetEventsByDate(date);

            int yPosition = 0;
            foreach (Event evt in dayEvents)
            {
                Label eventLabel = new Label();
                eventLabel.AutoSize = false;
                eventLabel.Width = eventArea.Width - 5;
                eventLabel.Height = 20;
                eventLabel.Location = new Point(0, yPosition);
                eventLabel.Text = evt.Title;
                eventLabel.Font = new Font("Arial", 8F);
                eventLabel.BackColor = (evt is Meeting) ? Color.LightSalmon : Color.LightBlue;
                eventLabel.Tag = evt;
                eventLabel.Click += EventLabel_Click;

                // more details
                ToolTip tooltip = new ToolTip();
                tooltip.SetToolTip(eventLabel, $"{evt.Title}\n{evt.StartTime.ToShortTimeString()} - {evt.EndTime.ToShortTimeString()}");

                eventArea.Controls.Add(eventLabel);
                yPosition += 22;
            }
        }

        // Get events for specific day
        private List<Event> GetEventsForDay(DateTime date)
        {
            if (currentUser == null)
                return new List<Event>();
            return currentUser.GetEventsByDate(date);
        }

        // handle event label click
        private void EventLabel_Click(object sender, EventArgs e)
        {
            Label eventLabel = sender as Label;
            if (eventLabel != null && eventLabel.Tag is Event)
            {
                Event selectedEvent = (Event)eventLabel.Tag;
                ShowEventDetails(selectedEvent);
            }
        }
        private void ShowEventDetails(Event selectedEvent)
        {
            // event details
            using (Form detailsForm = new Form())
            {
                detailsForm.Text = "Event Details";
                detailsForm.Width = 400;
                detailsForm.Height = 300;
                detailsForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                detailsForm.MaximizeBox = false;
                detailsForm.MinimizeBox = false;
                detailsForm.StartPosition = FormStartPosition.CenterParent;

                // icon
                PictureBox iconBox = new PictureBox();
                iconBox.Image = selectedEvent is Meeting ?
                    SystemIcons.Information.ToBitmap() : SystemIcons.Application.ToBitmap();
                iconBox.SizeMode = PictureBoxSizeMode.StretchImage;
                iconBox.Size = new Size(32, 32);
                iconBox.Location = new Point(20, 20);

                Label lblDetails = new Label();
                string eventType = selectedEvent is Meeting ? "Meeting" : "Event";
                lblDetails.Text = $"{eventType}: {selectedEvent.Title}\n" +
                                 $"Date: {selectedEvent.StartTime.ToShortDateString()}\n" +
                                 $"Time: {selectedEvent.StartTime.ToShortTimeString()} - {selectedEvent.EndTime.ToShortTimeString()}";

                if (selectedEvent is Meeting meeting)
                {
                    lblDetails.Text += $"\nOrganizer: {meeting.Organizer}\n" +
                                      $"Location: {meeting.Location}\n" +
                                      $"Attendees: {string.Join(", ", meeting.Attendees)}";
                }

                lblDetails.Location = new Point(60, 20);
                lblDetails.Size = new Size(320, 180);
                lblDetails.AutoSize = false;

                // OK
                Button btnOK = new Button();
                btnOK.Text = "OK";
                btnOK.DialogResult = DialogResult.OK;
                btnOK.Size = new Size(80, 30);
                btnOK.Location = new Point(300, 220);

                // attempt to fix regular users editing/deleting meetings
                bool canEditDelete = false;

                if (selectedEvent is Meeting)
                {
                    Meeting meetingEvent = (Meeting)selectedEvent;
                    canEditDelete = (currentUser.IsManager && meetingEvent.Organizer == currentUser.Username);
                }
                else
                {
                    canEditDelete = (selectedEvent.Owner == currentUser.Username);
                }
                if (canEditDelete)
                {
                    // Edit
                    Button btnEdit = new Button();
                    btnEdit.Text = "Edit";
                    btnEdit.Size = new Size(80, 30);
                    btnEdit.Location = new Point(120, 220);
                    btnEdit.Click += (s, e) =>
                    {
                        detailsForm.DialogResult = DialogResult.Yes;
                    };

                    // Delete
                    Button btnDelete = new Button();
                    btnDelete.Text = "Delete";
                    btnDelete.Size = new Size(80, 30);
                    btnDelete.Location = new Point(210, 220);
                    btnDelete.Click += (s, e) =>
                    {
                        // get confirmation
                        if (MessageBox.Show(
                            "Are you sure you want to delete this event?",
                            "Confirm Delete",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Warning) == DialogResult.Yes)
                        {
                            detailsForm.DialogResult = DialogResult.No;
                        }
                    };
                    detailsForm.Controls.Add(btnEdit);
                    detailsForm.Controls.Add(btnDelete);
                }

                // add the other controls
                detailsForm.Controls.Add(iconBox);
                detailsForm.Controls.Add(lblDetails);
                detailsForm.Controls.Add(btnOK);

                detailsForm.AcceptButton = btnOK;

                DialogResult result = detailsForm.ShowDialog();

                if (result == DialogResult.Yes) // Edit
                {
                    EditEvent(selectedEvent);
                }
                else if (result == DialogResult.No) // Delete (I know it's stupid idk what to do)
                {
                    DeleteEvent(selectedEvent);
                }
            }
        }
        private void EditEvent(Event eventToEdit)
        {
            using (Form editForm = new Form())
            {
                editForm.Width = 300;
                editForm.Height = 400;
                editForm.Text = "Edit Event";

                // Title
                Label lblTitle = new Label() { Left = 20, Top = 20, Text = "Event Title:" };
                TextBox txtTitle = new TextBox()
                {
                    Left = 20,
                    Top = 40,
                    Width = 240,
                    Text = eventToEdit.Title
                };

                // Description
                //Label lblDescription = new Label() { Left = 20, Top = 70, Text = "Description:" };
                //TextBox txtDescription = new TextBox()
                //{
                //    Left = 20,
                //    Top = 90,
                //    Width = 240,
                //    Height = 60,
                //    Multiline = true,
                //    ScrollBars = ScrollBars.Vertical,
                //    Text = eventToEdit.Description
                //};

                // Date picker
                Label lblDate = new Label() { Left = 20, Top = 160, Text = "Date:" };
                DateTimePicker dtpDate = new DateTimePicker()
                {
                    Left = 20,
                    Top = 180,
                    Width = 240,
                    Format = DateTimePickerFormat.Short,
                    Value = eventToEdit.StartTime.Date
                };

                // Start time
                Label lblStartTime = new Label() { Left = 20, Top = 210, Text = "Start Time:" };
                DateTimePicker dtpStartTime = new DateTimePicker()
                {
                    Left = 20,
                    Top = 230,
                    Width = 240,
                    Format = DateTimePickerFormat.Time,
                    ShowUpDown = true,
                    Value = eventToEdit.StartTime
                };

                // End time
                Label lblEndTime = new Label() { Left = 20, Top = 260, Text = "End Time:" };
                DateTimePicker dtpEndTime = new DateTimePicker()
                {
                    Left = 20,
                    Top = 280,
                    Width = 240,
                    Format = DateTimePickerFormat.Time,
                    ShowUpDown = true,
                    Value = eventToEdit.EndTime
                };

                // Buttons
                Button btnSave = new Button() { Text = "Save", Left = 20, Top = 320, Width = 100 };
                Button btnCancel = new Button() { Text = "Cancel", Left = 160, Top = 320, Width = 100 };

                // Validation and update
                btnSave.Click += (s, args) =>
                {
                    if (string.IsNullOrWhiteSpace(txtTitle.Text))
                    {
                        MessageBox.Show("Event title is required", "Missing Title",
                                       MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (dtpEndTime.Value.TimeOfDay <= dtpStartTime.Value.TimeOfDay)
                    {
                        MessageBox.Show("End time must be after start time", "Invalid Times",
                                       MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // temporary event to check for conflicts
                    Event updatedEvent = new Event(
                        txtTitle.Text,
                        dtpDate.Value.Date.Add(dtpStartTime.Value.TimeOfDay),
                        dtpDate.Value.Date.Add(dtpEndTime.Value.TimeOfDay)
                    );
                    updatedEvent.Id = eventToEdit.Id;
                    updatedEvent.Owner = eventToEdit.Owner;

                    // Check for conflicts, also fix return to calendar
                    if (currentUser.HasEventConflict(updatedEvent))
                    {
                        MessageBox.Show("Failed to update event. There may be a scheduling conflict.",
                                      "Update Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    eventToEdit.Title = updatedEvent.Title;
                    eventToEdit.StartTime = updatedEvent.StartTime;
                    eventToEdit.EndTime = updatedEvent.EndTime;

                    MessageBox.Show($"Event '{eventToEdit.Title}' updated",
                                  "Event Updated", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    editForm.DialogResult = DialogResult.OK;
                };

                btnCancel.Click += (s, args) => { editForm.DialogResult = DialogResult.Cancel; };

                // Add all controls to form
                editForm.Controls.Add(lblTitle);
                editForm.Controls.Add(txtTitle);
                //editForm.Controls.Add(lblDescription);
                //editForm.Controls.Add(txtDescription);
                editForm.Controls.Add(lblDate);
                editForm.Controls.Add(dtpDate);
                editForm.Controls.Add(lblStartTime);
                editForm.Controls.Add(dtpStartTime);
                editForm.Controls.Add(lblEndTime);
                editForm.Controls.Add(dtpEndTime);
                editForm.Controls.Add(btnSave);
                editForm.Controls.Add(btnCancel);

                editForm.AcceptButton = btnSave;
                editForm.CancelButton = btnCancel;

                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    // this is duplicating events
                    // copy the event to check for conflicts
                    //Event updatedEvent = new Event(
                    //    txtTitle.Text,
                    //    dtpDate.Value.Date.Add(dtpStartTime.Value.TimeOfDay),
                    //    dtpDate.Value.Date.Add(dtpEndTime.Value.TimeOfDay)
                    //);
                    //updatedEvent.Id = eventToEdit.Id;
                    //updatedEvent.Owner = eventToEdit.Owner;
                    //bool updated = false;

                    //// remove old
                    //if (currentUser.DeleteEvent(eventToEdit))
                    //{
                    //    // add new
                    //    updated = currentUser.AddEvent(updatedEvent);
                    //}

                    //if (updated)
                    //{
                    //    MessageBox.Show($"Event '{updatedEvent.Title}' updated",
                    //                  "Event Updated", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //}
                    //else
                    //{
                    //    currentUser.AddEvent(eventToEdit);
                    //    MessageBox.Show("Failed to update event. There may be a scheduling conflict.",
                    //                  "Update Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //}

                    // Refresh calendar
                    GenerateCalendar(currentYear, currentMonth);
                    DisplayMonthEvents();
                }
            }
        }

        private void DeleteEvent(Event eventToDelete)
        {
            Debug.WriteLine($"Attempting to delete event: ID={eventToDelete.Id}, Title={eventToDelete.Title}");

            if (currentUser.RemoveEvent(eventToDelete))
            {
                // Refresh calendar
                GenerateCalendar(currentYear, currentMonth);
                DisplayMonthEvents();

                MessageBox.Show($"Event '{eventToDelete.Title}' deleted",
                              "Event Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Failed to delete the event",
                              "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // handle day cell click event
        private void DayCell_Click(object sender, EventArgs e)
        {
            Panel clickedCell = sender as Panel;
            if (clickedCell == null && sender is Label label)
            {
                clickedCell = label.Parent as Panel;
            }

            if (clickedCell != null)
            {
                // find the date
                Label dayLabel = clickedCell.Controls["dayLabel"] as Label;
                if (dayLabel != null && !string.IsNullOrEmpty(dayLabel.Text))
                {
                    int day = int.Parse(dayLabel.Text);
                    selectedDate = new DateTime(currentYear, currentMonth, day);
                    selectedDayCell = clickedCell;

                    // Show day menu
                    ShowDayMenu();

                    // highlight selected cell
                    HighlightSelectedDay(clickedCell);
                }
            }
        }

        private void ShowDayMenu()
        {
            ContextMenuStrip dayMenu = new ContextMenuStrip();
            ToolStripMenuItem addEventItem = new ToolStripMenuItem("Add Event");
            addEventItem.Click += (s, e) => AddEvent_Click();
            dayMenu.Items.Add(addEventItem);

            List<Event> dayEvents = GetEventsForDay(selectedDate);
            if (dayEvents.Count > 0)
            {
                ToolStripMenuItem viewEventsItem = new ToolStripMenuItem("View Events");
                foreach (Event evt in dayEvents)
                {
                    ToolStripMenuItem eventItem = new ToolStripMenuItem(evt.Title);
                    eventItem.Tag = evt;
                    eventItem.Click += (s, e) => ShowEventDetails((Event)((ToolStripMenuItem)s).Tag);
                    viewEventsItem.DropDownItems.Add(eventItem);
                }
                dayMenu.Items.Add(viewEventsItem);
            }

            if (dayMenu.Items.Count > 0)
            {
                dayMenu.Show(this, this.PointToClient(Cursor.Position));
            }
        }

        //event handler
        private void AddEvent_Click()
        {
            string title = "New Event";
            string description = "Event Description";

            using (Form inputForm = new Form())
            {
                inputForm.Width = 300;
                inputForm.Height = 400;
                inputForm.Text = "New Event";

                Label lblTitle = new Label()
                {
                    Left = 20,
                    Top = 20,
                    Text = "Event Title:"
                };

                TextBox txtTitle = new TextBox()
                {
                    Left = 20,
                    Top = 40,
                    Width = 240
                };

                Label lblDescription = new Label()
                {
                    Left = 20,
                    Top = 70,
                    Text = "Description:"
                };

                TextBox txtDescription = new TextBox()
                {
                    Left = 20,
                    Top = 90,
                    Width = 240,
                    Height = 60,
                    Multiline = true,
                    ScrollBars = ScrollBars.Vertical
                };

                Label lblDate = new Label()
                {
                    Left = 20,
                    Top = 160,
                    Text = "Date:"
                };

                DateTimePicker dtpDate = new DateTimePicker()
                {
                    Left = 20,
                    Top = 180,
                    Width = 240,
                    Format = DateTimePickerFormat.Short
                };

                // set default
                if (selectedDayCell != null && selectedDayCell.Tag is DateTime)
                {
                    dtpDate.Value = (DateTime)selectedDayCell.Tag;
                }

                Label lblStartTime = new Label()
                {
                    Left = 20,
                    Top = 210,
                    Text = "Start Time:"
                };

                DateTimePicker dtpStartTime = new DateTimePicker()
                {
                    Left = 20,
                    Top = 230,
                    Width = 240,
                    Format = DateTimePickerFormat.Time,
                    ShowUpDown = true
                };
                dtpStartTime.Value = DateTime.Today.AddHours(9); //default 9am start

                Label lblEndTime = new Label()
                {
                    Left = 20,
                    Top = 260,
                    Text = "End Time:"
                };
                DateTimePicker dtpEndTime = new DateTimePicker()
                {
                    Left = 20,
                    Top = 280,
                    Width = 240,
                    Format = DateTimePickerFormat.Time,
                    ShowUpDown = true
                };
                dtpEndTime.Value = DateTime.Today.AddHours(10); //default 10am end

                Button btnCreate = new Button()
                {
                    Text = "Create",
                    Left = 20,
                    Top = 320,
                    Width = 100
                };
                Button btnCancel = new Button()
                {
                    Text = "Cancel",
                    Left = 160,
                    Top = 320,
                    Width = 100
                };
                btnCreate.Click += (s, args) =>
                {
                    // validation!
                    if (string.IsNullOrWhiteSpace(txtTitle.Text))
                    {
                        MessageBox.Show("Event title is required", "Missing Title",
                                       MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (dtpEndTime.Value.TimeOfDay <= dtpStartTime.Value.TimeOfDay)
                    {
                        MessageBox.Show("End time must be after start time", "Invalid Times",
                                       MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    //create event
                    title = txtTitle.Text;
                    description = txtDescription.Text;
                    inputForm.Tag = new Tuple<DateTime, DateTime, DateTime>(
                        dtpDate.Value.Date,
                        dtpStartTime.Value,
                        dtpEndTime.Value
                    );

                    inputForm.DialogResult = DialogResult.OK;
                };
                btnCancel.Click += (s, args) => { inputForm.DialogResult = DialogResult.Cancel; };

                // add controls
                inputForm.Controls.Add(lblTitle);
                inputForm.Controls.Add(txtTitle);
                inputForm.Controls.Add(lblDescription);
                inputForm.Controls.Add(txtDescription);
                inputForm.Controls.Add(lblDate);
                inputForm.Controls.Add(dtpDate);
                inputForm.Controls.Add(lblStartTime);
                inputForm.Controls.Add(dtpStartTime);
                inputForm.Controls.Add(lblEndTime);
                inputForm.Controls.Add(dtpEndTime);
                inputForm.Controls.Add(btnCreate);
                inputForm.Controls.Add(btnCancel);

                inputForm.AcceptButton = btnCreate;
                inputForm.CancelButton = btnCancel;

                if (inputForm.ShowDialog() == DialogResult.OK)
                {
                    // get event details and make the event
                    var eventData = (Tuple<DateTime, DateTime, DateTime>)inputForm.Tag;
                    DateTime eventDate = eventData.Item1;
                    DateTime startTime = eventData.Item2;
                    DateTime endTime = eventData.Item3;

                    DateTime startDateTime = eventDate.Date.Add(startTime.TimeOfDay);
                    DateTime endDateTime = eventDate.Date.Add(endTime.TimeOfDay);

                    Event newEvent = new Event(
                        title,
                        startDateTime,
                        endDateTime
                    );
                    newEvent.Owner = currentUser.Username;
                    bool added = currentUser.AddEvent(newEvent);

                    if (added)
                    {
                        GenerateCalendar(currentYear, currentMonth);
                        DisplayMonthEvents();

                        MessageBox.Show($"Event '{title}' created for {eventDate.ToShortDateString()}",
                                      "Event Created", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Failed to add event. There may be a scheduling conflict.",
                                      "Add Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // selected cell highlight
        private void HighlightSelectedDay(Panel selectedCell)
        {
            for (int row = 0; row < 6; row++)
            {
                for (int col = 0; col < 7; col++)
                {
                    dayCells[row, col].BorderStyle = BorderStyle.FixedSingle;
                }
            }
            selectedCell.BorderStyle = BorderStyle.Fixed3D;
        }

        // navigate between months
        public void PreviousMonth()
        {
            currentMonth--;
            if (currentMonth < 1)
            {
                currentMonth = 12;
                currentYear--;
            }
            GenerateCalendar(currentYear, currentMonth);
            DisplayMonthEvents();
        }

        public void NextMonth()
        {
            currentMonth++;
            if (currentMonth > 12)
            {
                currentMonth = 1;
                currentYear++;
            }
            GenerateCalendar(currentYear, currentMonth);
            DisplayMonthEvents();
        }

        // handlers for month navigation
        private void nextMonthButton_Click(object sender, EventArgs e)
        {
            NextMonth();
        }

        private void previousMonthButton_Click(object sender, EventArgs e)
        {
            PreviousMonth();
        }

        // Create meeting button click handler
        private void createMeetingButton_Click(object sender, EventArgs e)
        {
            if (!currentUser.IsManager)
            {
                MessageBox.Show("Only managers can create meetings.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Step 1: Select Date
            using (Form dateSelectionForm = new Form())
            {
                dateSelectionForm.Width = 350;
                dateSelectionForm.Height = 200;
                dateSelectionForm.Text = "Select Meeting Date";
                dateSelectionForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                dateSelectionForm.StartPosition = FormStartPosition.CenterParent;
                dateSelectionForm.MaximizeBox = false;
                dateSelectionForm.MinimizeBox = false;

                Label lblInstructions = new Label();
                lblInstructions.Text = "Please select a date for the meeting:";
                lblInstructions.Location = new Point(20, 20);
                lblInstructions.AutoSize = true;

                // Date picker
                DateTimePicker dtpMeetingDate = new DateTimePicker();
                dtpMeetingDate.Location = new Point(20, 50);
                dtpMeetingDate.Width = 300;
                dtpMeetingDate.Format = DateTimePickerFormat.Short;
                dtpMeetingDate.Value = DateTime.Now.Date.AddDays(1); // Default to tomorrow

                // Buttons
                Button btnNext = new Button();
                Button btnCancel = new Button();
                btnNext.Text = "Next";
                btnCancel.Text = "Cancel";
                btnNext.Location = new Point(130, 100);
                btnCancel.Location = new Point(230, 100);
                btnNext.Width = 80;
                btnCancel.Width = 80;
                btnNext.DialogResult = DialogResult.OK;
                btnCancel.DialogResult = DialogResult.Cancel;

                // Add controls
                dateSelectionForm.Controls.Add(lblInstructions);
                dateSelectionForm.Controls.Add(dtpMeetingDate);
                dateSelectionForm.Controls.Add(btnNext);
                dateSelectionForm.Controls.Add(btnCancel);

                dateSelectionForm.AcceptButton = btnNext;
                dateSelectionForm.CancelButton = btnCancel;

                // date selection form
                if (dateSelectionForm.ShowDialog() != DialogResult.OK)
                    return;

                // Get date
                DateTime meetingDate = dtpMeetingDate.Value.Date;

                // Step 2: Select Users
                List<User> selectedUsers = new List<User>();

                using (Form userSelectionForm = new Form())
                {
                    userSelectionForm.Width = 400;
                    userSelectionForm.Height = 300;
                    userSelectionForm.Text = "Select Meeting Attendees";
                    userSelectionForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                    userSelectionForm.StartPosition = FormStartPosition.CenterParent;
                    userSelectionForm.MaximizeBox = false;
                    userSelectionForm.MinimizeBox = false;

                    Label lblUserInstructions = new Label();
                    lblUserInstructions.Text = "Select users to invite to the meeting:";
                    lblUserInstructions.Location = new Point(20, 20);
                    lblUserInstructions.AutoSize = true;

                    List<User> users = User.GetAllUsers();

                    // Create checklist
                    CheckedListBox clbUsers = new CheckedListBox();
                    clbUsers.Location = new Point(20, 50);
                    clbUsers.Width = 350;
                    clbUsers.Height = 150;

                    // I initialized this 'in' the for loop 
                    // and it took forever to realize what was
                    // broke
                    Dictionary<string, User> userDictionary = new Dictionary<string, User>();
                    clbUsers.Tag = userDictionary;

                    foreach (User user in users)
                    {
                        if (user.Username != currentUser.Username)
                        {
                            // pretty name instead of object displayed
                            string displayName = $"{user.FirstName} {user.LastName} ({user.Username})";
                            clbUsers.Items.Add(displayName, false);
                            ((Dictionary<string, User>)clbUsers.Tag)[displayName] = user;
                        }
                    }

                    // Buttons
                    Button btnFindSlots = new Button();
                    Button btnCancelUsers = new Button();
                    btnFindSlots.Text = "Find Available Times";
                    btnCancelUsers.Text = "Cancel";
                    btnFindSlots.Location = new Point(140, 220);
                    btnCancelUsers.Location = new Point(280, 220);
                    btnFindSlots.Width = 130;
                    btnCancelUsers.Width = 80;
                    btnFindSlots.DialogResult = DialogResult.OK;
                    btnCancelUsers.DialogResult = DialogResult.Cancel;

                    // Add controls
                    userSelectionForm.Controls.Add(lblUserInstructions);
                    userSelectionForm.Controls.Add(clbUsers);
                    userSelectionForm.Controls.Add(btnFindSlots);
                    userSelectionForm.Controls.Add(btnCancelUsers);

                    userSelectionForm.AcceptButton = btnFindSlots;
                    userSelectionForm.CancelButton = btnCancelUsers;

                    // user selection form
                    if (userSelectionForm.ShowDialog() != DialogResult.OK)
                        return;

                    // Get selected users
                    foreach (object item in clbUsers.CheckedItems)
                    {
                        string displayName = item.ToString();
                        if (clbUsers.Tag != null && ((Dictionary<string, User>)clbUsers.Tag).ContainsKey(displayName))
                        {
                            User selectedUser = ((Dictionary<string, User>)clbUsers.Tag)[displayName];
                            selectedUsers.Add(selectedUser);
                        }
                    }
                }

                // can't have a meeting with nobody
                if (selectedUsers.Count == 0)
                {
                    MessageBox.Show("No users were selected. Meeting creation cancelled.",
                                   "No Attendees", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Step 3: Find available time slots
                TimeSpan meetingDuration = TimeSpan.FromHours(1);
                List<DateTime> availableSlots = currentUser.FindAvailableMeetingSlots(meetingDate, meetingDuration, selectedUsers);

                if (availableSlots.Count == 0)
                {
                    MessageBox.Show("No available time slots were found for the selected date and attendees.",
                                   "No Available Slots", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                using (Form timeSlotForm = new Form())
                {
                    timeSlotForm.Width = 400;
                    timeSlotForm.Height = 350;
                    timeSlotForm.Text = "Select Meeting Time";
                    timeSlotForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                    timeSlotForm.StartPosition = FormStartPosition.CenterParent;
                    timeSlotForm.MaximizeBox = false;
                    timeSlotForm.MinimizeBox = false;

                    Label lblSlotInstructions = new Label();
                    lblSlotInstructions.Text = "Available time slots:";
                    lblSlotInstructions.Location = new Point(20, 20);
                    lblSlotInstructions.AutoSize = true;

                    ListBox lbTimeSlots = new ListBox();
                    lbTimeSlots.Location = new Point(20, 50);
                    lbTimeSlots.Width = 350;
                    lbTimeSlots.Height = 180;

                    foreach (DateTime slot in availableSlots)
                    {
                        lbTimeSlots.Items.Add($"{slot.ToShortTimeString()} - {slot.AddHours(1).ToShortTimeString()}");
                    }

                    // Title
                    Label lblTitle = new Label();
                    lblTitle.Text = "Meeting Title:";
                    lblTitle.Location = new Point(20, 240);
                    lblTitle.AutoSize = true;

                    TextBox txtTitle = new TextBox();
                    txtTitle.Location = new Point(110, 237);
                    txtTitle.Width = 260;

                    // Buttons
                    Button btnCreateMeeting = new Button();
                    Button btnCancelMeeting = new Button();
                    btnCreateMeeting.Text = "Create Meeting";
                    btnCancelMeeting.Text = "Cancel";
                    btnCreateMeeting.Location = new Point(170, 270);
                    btnCancelMeeting.Location = new Point(290, 270);
                    btnCreateMeeting.Width = 110;
                    btnCancelMeeting.Width = 80;
                    btnCreateMeeting.Click += (s, args) =>
                    {
                        if (lbTimeSlots.SelectedIndex < 0)
                        {
                            MessageBox.Show("Please select a time slot.", "No Time Selected",
                                          MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        if (string.IsNullOrWhiteSpace(txtTitle.Text))
                        {
                            MessageBox.Show("Please enter a meeting title.", "No Title",
                                          MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        timeSlotForm.DialogResult = DialogResult.OK;
                    };
                    btnCancelMeeting.DialogResult = DialogResult.Cancel;

                    // Add controls
                    timeSlotForm.Controls.Add(lblSlotInstructions);
                    timeSlotForm.Controls.Add(lbTimeSlots);
                    timeSlotForm.Controls.Add(lblTitle);
                    timeSlotForm.Controls.Add(txtTitle);
                    timeSlotForm.Controls.Add(btnCreateMeeting);
                    timeSlotForm.Controls.Add(btnCancelMeeting);

                    timeSlotForm.CancelButton = btnCancelMeeting;

                    // time slot selection
                    if (timeSlotForm.ShowDialog() != DialogResult.OK)
                        return;

                    // Get time slot and create the meeting
                    if (lbTimeSlots.SelectedIndex >= 0)
                    {
                        DateTime selectedSlot = availableSlots[lbTimeSlots.SelectedIndex];

                        Meeting newMeeting = new Meeting(
                            txtTitle.Text,
                            selectedSlot,
                            selectedSlot.AddHours(1),
                            currentUser.Username,
                            selectedUsers.Select(u => u.Username).ToList()
                        );

                        if (currentUser.AddEvent(newMeeting))
                        {
                            GenerateCalendar(currentYear, currentMonth);
                            DisplayMonthEvents();

                            MessageBox.Show($"Meeting '{txtTitle.Text}' created successfully for {selectedSlot.ToShortDateString()} at {selectedSlot.ToShortTimeString()}.",
                                          "Meeting Created", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Failed to create meeting. There may be a scheduling conflict.",
                                          "Meeting Creation Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }

                        // causing meetings to create duplicate events for
                        // every attendee
                        // Add to everyone's calendars and refresh
                        //currentUser.AddEvent(newMeeting);
                        //foreach (User user in selectedUsers)
                        //{
                        //    user.AddEvent(newMeeting);
                        //}
                        //GenerateCalendar(currentYear, currentMonth);
                        //DisplayMonthEvents();

                        //MessageBox.Show($"Meeting '{txtTitle.Text}' created successfully for {selectedSlot.ToShortDateString()} at {selectedSlot.ToShortTimeString()}.",
                        //              "Meeting Created", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        private void DisplayMonthEvents()
        {
            // Clear the panel
            eventPanel.Controls.Clear();

            // Get events for the current month
            List<Event> monthEvents = new List<Event>();
            foreach (Event evt in currentUser.Calendar)
            {
                if (evt.StartTime.Year == currentYear && evt.StartTime.Month == currentMonth)
                {
                    monthEvents.Add(evt);
                }
            }

            // Sort events
            monthEvents = monthEvents.OrderBy(e => e.StartTime).ToList();

            int yPosition = 5;
            const int padding = 5;
            const int eventHeight = 60;

            foreach (Event evt in monthEvents)
            {
                // Create panels
                Panel eventItemPanel = new Panel();
                eventItemPanel.Width = eventPanel.Width - 25; // Allow for scrollbar
                eventItemPanel.Height = eventHeight;
                eventItemPanel.Location = new Point(5, yPosition);
                eventItemPanel.BackColor = evt is Meeting ? Color.FromArgb(255, 128, 128) : Color.FromArgb(128, 191, 255);

                Label lblDateTime = new Label();
                lblDateTime.AutoSize = false;
                lblDateTime.Width = eventItemPanel.Width - 10;
                lblDateTime.Height = 20;
                lblDateTime.Location = new Point(5, 5);
                lblDateTime.Text = $"{evt.StartTime.ToShortDateString()} {evt.StartTime.ToShortTimeString()}";
                lblDateTime.Font = new Font("Arial", 9, FontStyle.Bold);
                lblDateTime.ForeColor = Color.Black;

                // title
                Label lblTitle = new Label();
                lblTitle.AutoSize = false;
                lblTitle.Width = eventItemPanel.Width - 10;
                lblTitle.Height = 20;
                lblTitle.Location = new Point(5, 25);
                lblTitle.Text = evt.Title;
                lblTitle.Font = new Font("Arial", 10, FontStyle.Regular);
                lblTitle.ForeColor = Color.Black;

                eventItemPanel.Tag = evt;
                eventItemPanel.Click += EventItem_Click;
                lblTitle.Click += EventItem_Click;
                lblDateTime.Click += EventItem_Click;

                eventItemPanel.Controls.Add(lblDateTime);
                eventItemPanel.Controls.Add(lblTitle);

                eventPanel.Controls.Add(eventItemPanel);

                // update position for next event or overlaps
                yPosition += eventHeight + padding;
            }

            Label headerLabel = eventPanel.Parent.Controls["eventsHeaderLabel"] as Label;
            if (headerLabel != null)
            {
                headerLabel.Text = "Events";
            }
        }

        // handle event item click
        private void EventItem_Click(object sender, EventArgs e)
        {
            // Get clicked panel
            Panel eventPanel = sender as Panel;
            if (eventPanel == null && sender is Label label)
            {
                eventPanel = label.Parent as Panel;
            }

            if (eventPanel != null && eventPanel.Tag is Event selectedEvent)
            {
                ShowEventDetails(selectedEvent);
            }
        }
    }
}