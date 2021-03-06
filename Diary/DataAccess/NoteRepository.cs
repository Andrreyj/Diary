﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Diary.Model;
using System.Threading;
using System.Threading.Tasks;

namespace Diary.DataAccess
{
    public class NoteRepository
    {
        #region Fields

        string connectionString;

        #endregion // Fields

        #region Constructor

        public NoteRepository(string connectionString)
        {
            if(!CheckConnect(connectionString))
            {
                throw new  Exception();

            }

            this.connectionString = connectionString;
        }

        #endregion // Constructor

        #region Public methods

        public List<Note> GetNotesOfDay(DateTime date)
        {
            if (date != null)
            {
                string query = $"SELECT * from dbo.Note WHERE Note_date=@Note_date";

                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@Note_date", date.ToShortDateString());

                return LoadData(query, connection, command);
            }
            else
            {
                throw new ArgumentNullException("date is null");
            }
        }

        public List<Note> GetNotesOfDays(DateTime dateBegin, DateTime dateEnd)
        {
            if (dateBegin != null && dateEnd != null)
            {
                string query = $"SELECT * from dbo.Note WHERE Note_date >= @Note_dateBegin" +
                    $" and Note_date <= @Note_dateEnd";

                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@Note_dateBegin", dateBegin.ToShortDateString());
                command.Parameters.AddWithValue("@Note_dateEnd", dateEnd.ToShortDateString());

                return LoadData(query, connection, command);
            }
            else
            {
                throw new ArgumentNullException("dateBegin/dateEnd is null");
            }

        }

        public List<Note> GetAllNotes()
        {
            string query = $"SELECT * from dbo.Note";

            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = new SqlCommand(query, connection);

            return LoadData(query, connection, command);
        }

        public async void AddNoteAsync(Note note)
        {
            if (note != null)
            {
                note.IdNote = GetCountNotes() + 1;

                string query = $"Insert Into dbo.Note (Id_note, Note_date, Id_type_job, Id_relevance, Id_progress, Time_start, Time_finish)" +
                    $" values (@Id_note, @Note_date, @Id_type_job, @Id_relevance, @Id_progress, @Time_start, @Time_finish)";

                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = GetAddUpdateNoteSqlCommand(query, connection, note);

                await Task.Run(() => DumpData(query, connection, command));
            }
            else
            {
                throw new ArgumentNullException("note is null");
            }
        }
        public void AddNote(Note note)
        {
            if (note != null)
            {
                note.IdNote = GetCountNotes() + 1;

                string query = $"Insert Into dbo.Note (Id_note, Note_date, Id_type_job, Id_relevance, Id_progress, Time_start, Time_finish)" +
                    $" values (@Id_note, @Note_date, @Id_type_job, @Id_relevance, @Id_progress, @Time_start, @Time_finish)";

                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = GetAddUpdateNoteSqlCommand(query, connection, note);

                DumpData(query, connection, command);
            }
            else
            {
                throw new ArgumentNullException("note is null");
            }
        }
        public void UpdateNote(Note note)
        {
            if (note != null)
            {
                string query = $"Update dbo.Note " +
                    $" SET Note_date=@Note_date, " +
                    $" Id_type_job=@Id_type_job, Id_relevance=@Id_relevance, " +
                    $" Id_progress=@Id_progress, Time_start=@Time_start, Time_finish=@Time_finish" +
                    $" WHERE Id_note=@Id_note";

                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = GetAddUpdateNoteSqlCommand(query, connection, note);

                DumpData(query, connection, command);
            }
            else
            {
                throw new ArgumentNullException("note is null");  
            }
        }

        public void RemoveNote(Note note)
        {
            if (note != null)
            {
                string query = $"DELETE from dbo.Note WHERE Id_note=@Id_note";

                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@Id_note", note.IdNote);

                DumpData(query, connection, command);
            }
            else
            {
                throw new ArgumentNullException("note is null");
            }
        }
        public void RemoveNotes(DateTime date)
        {
            if (date != null)
            {
                string query = $"DELETE from dbo.Note WHERE Note_date=@Note_date";

                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@Note_date", date.ToShortDateString());

                DumpData(query, connection, command);
            }
            else
            {
                throw new ArgumentNullException("date is null");
            }
        }

        #endregion // Public methods

        #region Private methods

        SqlCommand GetAddUpdateNoteSqlCommand(string query, SqlConnection connection, Note note)
        {
            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Id_note", note.IdNote);
            command.Parameters.AddWithValue("@Note_date", note.NoteDate.ToShortDateString());
            command.Parameters.AddWithValue("@Id_type_job", note.TypeJob.IdTypeJob);
            command.Parameters.AddWithValue("@Id_relevance", note.Relevance.IdRelevance);
            command.Parameters.AddWithValue("@Id_progress", note.Progress.IdProgress);
            command.Parameters.AddWithValue("@Time_start", note.TimeStart);
            command.Parameters.AddWithValue("@Time_finish", note.TimeFinish);

            return command;
        }

        bool CheckConnect(string connectionString)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        int GetCountNotes()
        {
            int count = 0;

            using (SqlConnection connection =
                           new SqlConnection(connectionString))
            {
                string query = $"SELECT max(Id_note) from dbo.Note";

                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    count = (int)reader[0];
                }
                reader.Close();
            }

            return count;
        }

        List<Note> LoadData(string query, SqlConnection connection, SqlCommand command)
        {
            List<Note> loadList = new List<Note>();

            using ( connection )
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    loadList.Add(
                        new Note(
                                idNote: (int)reader[0],
                                noteData: (DateTime)reader[1],
                                typeJob: new TypeJobRepository(connectionString).GetTypeJob((int)reader[2]),
                                relevance: new RelevanceRepository(connectionString).GetdRelevance((int)reader[3]),
                                progress: new ProgressRepository(connectionString).GetProgress((int)reader[4]),
                                timeStart: (TimeSpan)reader[5],
                                timeFinish: (TimeSpan)reader[6]
                            )
                        );
                }
                reader.Close(); ;
            }

            return loadList;
        }
     
        void DumpData(string query, SqlConnection connection, SqlCommand command)
        {
            using (connection)
            {
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        #endregion // Private methods
    }
}
