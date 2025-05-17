using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Diagnostics;

namespace CalendarProject
{
    public class User
    {
        // Properties
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Username { get; protected set; }
        public string Password { get; protected set; }
        public bool IsManager { get; set; }
        public List<Event> Calendar { get; set; }

        // list of users for authentication
        private static List<User> users = new List<User>();

        // Constructor for user creation
        public User(string firstName, string lastName, string password)
        {
            FirstName = firstName;
            LastName = lastName;
            // first letter first name + last name, i.e John Doe is jdoe
            Username = (FirstName.Substring(0, 1) + LastName).ToLower();
            Password = password;
            IsManager = false;
            FullName = firstName + " " + lastName;
            Calendar = new List<Event>();
        }

        // Constructor for loading User from db
        private User(string username, string firstName, string lastName, string password, bool isManager)
        {
            FirstName = firstName;
            LastName = lastName;
            Username = username;
            Password = password;
            IsManager = isManager;
            FullName = firstName + " " + lastName;
            Calendar = new List<Event>();
        }

        // methods
        public static bool AddUser(User user)
        {
            MySqlConnection conn = new MySqlConnection(Database.CONNECTION_STRING);
            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(Database.SQL_INSERT_USER, conn);
                cmd.Parameters.AddWithValue("@username", user.Username);
                cmd.Parameters.AddWithValue("@firstName", user.FirstName);
                cmd.Parameters.AddWithValue("@lastName", user.LastName);
                cmd.Parameters.AddWithValue("@password", user.Password);
                cmd.Parameters.AddWithValue("@isManager", user.IsManager ? 1 : 0);

                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error saving user: " + ex.ToString());
                return false;
            }
            finally
            {
                conn.Close();
            }
        }

        public static List<User> GetAllUsers()
        {
            List<User> users = new List<User>();
            MySqlConnection conn = new MySqlConnection(Database.CONNECTION_STRING);

            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(Database.SQL_GET_ALL_USERS, conn);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        User user = new User(
                            reader["username"].ToString(),
                            reader["firstName"].ToString(),
                            reader["lastName"].ToString(),
                            reader["password"].ToString(),
                            Convert.ToBoolean(reader["isManager"])
                        );
                        users.Add(user);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error retrieving users: " + ex.ToString());
            }
            finally
            {
                conn.Close();
            }

            return users;
        }

        public static User Login(string username, string password)
        {
            Debug.WriteLine($"Attempting login as {username}");

            MySqlConnection conn = new MySqlConnection(Database.CONNECTION_STRING);
            User authenticatedUser = null;

            try
            {
                conn.Open();

                // debugging
                using (MySqlCommand describeCmd = new MySqlCommand($"DESCRIBE 834_sp25_t6_users;", conn))
                {
                    using (MySqlDataReader descReader = describeCmd.ExecuteReader())
                    {
                        Debug.WriteLine("User table structure:");
                        while (descReader.Read())
                        {
                            Debug.WriteLine($"- {descReader["Field"]} ({descReader["Type"]})");
                        }
                    }
                }

                // more debugging
                using (MySqlCommand sampleCmd = new MySqlCommand($"SELECT * FROM 834_sp25_t6_users LIMIT 1;", conn))
                {
                    using (MySqlDataReader sampleReader = sampleCmd.ExecuteReader())
                    {
                        if (sampleReader.Read())
                        {
                            Debug.WriteLine("Sample user data format:");
                            for (int i = 0; i < sampleReader.FieldCount; i++)
                            {
                                Debug.WriteLine($"- {sampleReader.GetName(i)}: {sampleReader[i]}");
                            }
                        }
                    }
                }

                // attempt login
                MySqlCommand cmd = new MySqlCommand(Database.SQL_GET_USER_BY_USERNAME, conn);
                cmd.Parameters.AddWithValue("@username", username);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string storedPassword = reader["password"].ToString();
                        if (storedPassword == password)
                        {
                            Debug.WriteLine("Login successful");
                            authenticatedUser = new User(
                                reader["username"].ToString(),
                                reader["firstName"].ToString(),
                                reader["lastName"].ToString(),
                                reader["password"].ToString(),
                                Convert.ToBoolean(reader["isManager"])
                            );
                        }
                        else
                        {
                            Debug.WriteLine("Login Failed (password)");
                        }
                    }
                    else
                    {
                        Debug.WriteLine("Login Failed (username)");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Login error: {ex.Message}");
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }

            return authenticatedUser;
        }

        public void Logout()
        {
            // clear controls etc
            Debug.WriteLine($"User {Username} logged out");
        }

        public void LoadEvents()
        {
            MySqlConnection conn = new MySqlConnection(Database.CONNECTION_STRING);
            try
            {
                conn.Open();
                Calendar.Clear();

                // user's events
                LoadOwnedEvents(conn);

                // user's meetings to attend
                LoadAttendingEvents(conn);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error loading events: " + ex.ToString());
            }
            finally
            {
                conn.Close();
            }
        }

        private void LoadOwnedEvents(MySqlConnection conn)
        {
            MySqlCommand cmd = new MySqlCommand(Database.SQL_GET_EVENTS_BY_OWNER, conn);
            cmd.Parameters.AddWithValue("@owner", Username);

            DataTable eventsTable = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            adapter.Fill(eventsTable);

            foreach (DataRow row in eventsTable.Rows)
            {
                int eventId = Convert.ToInt32(row["eventId"]);
                bool isMeeting = Convert.ToBoolean(row["isMeeting"]);

                if (!isMeeting)
                {
                    // Regular events
                    Event newEvent = new Event(
                        row["name"].ToString(),
                        Convert.ToDateTime(row["startDateTime"]),
                        Convert.ToDateTime(row["endDateTime"])
                    )
                    {
                        Id = eventId,
                        Owner = row["owner"].ToString()
                    };

                    Calendar.Add(newEvent);
                }
                else
                {
                    // Meeting - get attendees from eventroster
                    List<string> attendees = GetEventAttendees(conn, eventId);

                    Meeting meeting = new Meeting(
                        row["name"].ToString(),
                        Convert.ToDateTime(row["startDateTime"]),
                        Convert.ToDateTime(row["endDateTime"]),
                        row["owner"].ToString(),
                        attendees
                    )
                    {
                        Id = eventId,
                        Owner = row["owner"].ToString()
                    };

                    Calendar.Add(meeting);
                }
            }
        }
        private void LoadAttendingEvents(MySqlConnection conn)
        {
            MySqlCommand cmd = new MySqlCommand(Database.SQL_GET_EVENTS_FOR_ATTENDEE, conn);
            cmd.Parameters.AddWithValue("@username", Username);

            DataTable eventsTable = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            adapter.Fill(eventsTable);

            foreach (DataRow row in eventsTable.Rows)
            {
                int eventId = Convert.ToInt32(row["eventId"]);

                // Skip loaded events
                if (Calendar.Exists(e => e.Id == eventId))
                    continue;

                // this is from eventroster, so it must be a meeting
                List<string> attendees = GetEventAttendees(conn, eventId);

                Meeting meeting = new Meeting(
                    row["name"].ToString(),
                    Convert.ToDateTime(row["startDateTime"]),
                    Convert.ToDateTime(row["endDateTime"]),
                    row["owner"].ToString(),
                    attendees
                )
                {
                    Id = eventId,
                    Owner = row["owner"].ToString()
                };

                Calendar.Add(meeting);
            }
        }

        private List<string> GetEventAttendees(MySqlConnection conn, int eventId)
        {
            List<string> attendees = new List<string>();

            MySqlCommand cmd = new MySqlCommand(Database.SQL_GET_ROSTER_BY_EVENT, conn);
            cmd.Parameters.AddWithValue("@eventId", eventId);

            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    attendees.Add(reader["username"].ToString());
                }
            }

            return attendees;
        }

        public bool AddEvent(Event newEvent)
        {
            // make sure owner is set
            if (string.IsNullOrEmpty(newEvent.Owner))
            {
                newEvent.Owner = this.Username;
            }

            // check conflicts
            if (HasEventConflict(newEvent))
            {
                return false;
            }

            // Save
            if (SaveEventToDatabase(newEvent))
            {
                Calendar.Add(newEvent);
                return true;
            }

            return false;
        }

        private bool SaveEventToDatabase(Event eventToSave)
        {
            MySqlConnection conn = new MySqlConnection(Database.CONNECTION_STRING);
            try
            {
                conn.Open();
                MySqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    bool isMeeting = eventToSave is Meeting;
                    Debug.WriteLine($"Event Created: {eventToSave.Title}");

                    MySqlCommand cmd = new MySqlCommand(Database.SQL_INSERT_EVENT, conn);
                    cmd.Transaction = transaction;
                    cmd.Parameters.AddWithValue("@owner", eventToSave.Owner);
                    cmd.Parameters.AddWithValue("@name", eventToSave.Title);
                    cmd.Parameters.AddWithValue("@startDateTime", eventToSave.StartTime);
                    cmd.Parameters.AddWithValue("@endDateTime", eventToSave.EndTime);
                    cmd.Parameters.AddWithValue("@isMeeting", isMeeting ? 1 : 0);

                    cmd.ExecuteNonQuery();

                    // Get ID
                    cmd = new MySqlCommand(Database.SQL_GET_LAST_INSERT_ID, conn);
                    cmd.Transaction = transaction;
                    int eventId = Convert.ToInt32(cmd.ExecuteScalar());
                    eventToSave.Id = eventId;

                    if (isMeeting)
                    {
                        Meeting meeting = (Meeting)eventToSave;
                        Console.WriteLine($"Adding {meeting.Attendees.Count} attendees to meeting roster");

                        foreach (string attendee in meeting.Attendees)
                        {
                            cmd = new MySqlCommand(Database.SQL_INSERT_ROSTER_ENTRY, conn);
                            cmd.Transaction = transaction;
                            cmd.Parameters.AddWithValue("@eventId", eventId);
                            cmd.Parameters.AddWithValue("@username", attendee);
                            cmd.ExecuteNonQuery();
                            Console.WriteLine($"Added attendee {attendee} to meeting roster");
                        }

                        // organizer wasn't being added
                        if (!meeting.Attendees.Contains(meeting.Organizer))
                        {
                            cmd = new MySqlCommand(Database.SQL_INSERT_ROSTER_ENTRY, conn);
                            cmd.Transaction = transaction;
                            cmd.Parameters.AddWithValue("@eventId", eventId);
                            cmd.Parameters.AddWithValue("@username", meeting.Organizer);
                            cmd.ExecuteNonQuery();
                            Console.WriteLine($"Added organizer {meeting.Organizer} to meeting roster");
                        }
                    }

                    transaction.Commit();
                    Console.WriteLine($"Event saved successfully with ID: {eventId}");
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error in transaction, rolling back: " + ex.ToString());
                    transaction.Rollback();
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving event: " + ex.ToString());
                return false;
            }
            finally
            {
                conn.Close();
            }
        }

        public bool RemoveEvent(Event eventToRemove)
        {
            // Check permissions
            if (eventToRemove is Meeting meeting)
            {
                if (!IsManager || meeting.Organizer != this.Username)
                {
                    Console.WriteLine($"Cannot delete meeting: User {Username} is not the manager organizer");
                    return false;
                }
            }
            else
            {
                if (eventToRemove.Owner != this.Username)
                {
                    Console.WriteLine($"Cannot delete event: User {Username} is not the owner");
                    return false;
                }
            }

            Console.WriteLine($"User {Username} is removing event ID {eventToRemove.Id}, title: {eventToRemove.Title}");

            bool deletedFromDb = DeleteEventFromDatabase(eventToRemove);

            if (deletedFromDb)
            {
                // Verify and remove
                VerifyEventDeletion(eventToRemove.Id);
                return Calendar.Remove(eventToRemove);
            }

            return false;
        }
        // struggling with delete cascade, more debugging here
        public static void VerifyEventDeletion(int eventId)
        {
            MySqlConnection conn = new MySqlConnection(Database.CONNECTION_STRING);
            try
            {
                conn.Open();

                MySqlCommand checkEventCmd = new MySqlCommand(
                    "SELECT COUNT(*) FROM 834_sp25_t6_events WHERE eventId = @eventId", conn);
                checkEventCmd.Parameters.AddWithValue("@eventId", eventId);
                int eventCount = Convert.ToInt32(checkEventCmd.ExecuteScalar());

                MySqlCommand checkRosterCmd = new MySqlCommand(
                    "SELECT COUNT(*) FROM 834_sp25_t6_eventroster WHERE eventId = @eventId", conn);
                checkRosterCmd.Parameters.AddWithValue("@eventId", eventId);
                int rosterCount = Convert.ToInt32(checkRosterCmd.ExecuteScalar());

                if (eventCount > 0)
                {
                    Debug.WriteLine($"WARNING: Event ID {eventId} still exists in events table after deletion");
                }

                if (rosterCount > 0)
                {
                    Debug.WriteLine($"WARNING: Event ID {eventId} still has {rosterCount} roster entries after deletion");
                }

                if (eventCount == 0 && rosterCount == 0)
                {
                    Debug.WriteLine($"SUCCESS: Event ID {eventId} and all roster entries successfully deleted");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error verifying deletion: {ex.Message}");
            }
            finally
            {
                conn.Close();
            }
        }
        private bool DeleteEventFromDatabase(Event eventToDelete)
        {
            MySqlConnection conn = new MySqlConnection(Database.CONNECTION_STRING);
            try
            {
                conn.Open();
                MySqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    // Delete all associated roster entries manually since CASCADE failing
                    Debug.WriteLine($"Manually deleting roster entries for event ID {eventToDelete.Id}");

                    MySqlCommand deleteRosterCmd = new MySqlCommand(
                        $"DELETE FROM 834_sp25_t6_eventroster WHERE eventId = @eventId", conn);
                    deleteRosterCmd.Transaction = transaction;
                    deleteRosterCmd.Parameters.AddWithValue("@eventId", eventToDelete.Id);
                    int rosterRowsDeleted = deleteRosterCmd.ExecuteNonQuery();

                    Console.WriteLine($"Deleted {rosterRowsDeleted} roster entries");

                    MySqlCommand deleteEventCmd = new MySqlCommand(Database.SQL_DELETE_EVENT, conn);
                    deleteEventCmd.Transaction = transaction;
                    deleteEventCmd.Parameters.AddWithValue("@eventId", eventToDelete.Id);
                    deleteEventCmd.Parameters.AddWithValue("@owner", eventToDelete.Owner);
                    int eventRowsDeleted = deleteEventCmd.ExecuteNonQuery();

                    Console.WriteLine($"Deleted {eventRowsDeleted} event records for ID {eventToDelete.Id}");

                    // Commit tsx
                    if (rosterRowsDeleted > 0 || eventRowsDeleted > 0)
                    {
                        transaction.Commit();
                        return true;
                    }
                    else
                    {
                        // roll back if nothing deletes
                        transaction.Rollback();
                        Console.WriteLine("Nothing to delete - rolled back transaction");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in delete transaction: {ex.Message}");
                    transaction.Rollback();
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting event: {ex.Message}");
                return false;
            }
            finally
            {
                conn.Close();
            }
        }
        public bool HasEventConflict(Event newEvent)
        {
            foreach (var existingEvent in Calendar)
            {
                if (existingEvent.Id == newEvent.Id)
                    continue;

                // Check for overlap
                if (newEvent.StartTime < existingEvent.EndTime &&
                    newEvent.EndTime > existingEvent.StartTime)
                {
                    return true;
                }
            }
            return false;
        }

        public bool DeleteEvent(Event eventToRemove)
        {
            // ensure only owners remove/edit
            // edit is just copy/check conflict/remove and add
            if (eventToRemove is Meeting meeting)
            {
                if (!IsManager || meeting.Organizer != this.Username)
                {
                    return false;
                }
            }
            else
            {
                if (eventToRemove.Owner != this.Username)
                {
                    return false;
                }
            }
            return Calendar.Remove(eventToRemove);
        }

        public List<Event> GetEventsByDate(DateTime date)
        {
            return Calendar.FindAll(e =>
                e.StartTime.Date <= date.Date && e.EndTime.Date >= date.Date
            );
        }

        public List<DateTime> FindAvailableMeetingSlots(DateTime date, TimeSpan duration, List<User> attendees)
        {
            if (!IsManager)
            {
                return new List<DateTime>();
            }

            List<DateTime> availableSlots = new List<DateTime>();
            // 9-5 working hours
            DateTime dayStart = date.Date.AddHours(9);
            DateTime dayEnd = date.Date.AddHours(17);

            // Check every hour
            for (DateTime slot = dayStart; slot.Add(duration) <= dayEnd; slot = slot.AddMinutes(60))
            {
                DateTime slotEnd = slot.Add(duration);
                bool isAvailable = true;

                // Check manager's calendar
                foreach (var evt in Calendar)
                {
                    if (slot < evt.EndTime && slotEnd > evt.StartTime)
                    {
                        isAvailable = false;
                        break;
                    }
                }

                if (!isAvailable)
                    continue;

                // Check attendees' calendars
                foreach (var attendee in attendees)
                {
                    foreach (var evt in attendee.Calendar)
                    {
                        if (slot < evt.EndTime && slotEnd > evt.StartTime)
                        {
                            isAvailable = false;
                            break;
                        }
                    }

                    if (!isAvailable)
                        break;
                }

                if (isAvailable)
                {
                    availableSlots.Add(slot);
                }
            }

            return availableSlots;
        }
    }
}