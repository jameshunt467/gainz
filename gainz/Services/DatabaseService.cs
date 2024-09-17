using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using SQLite;
using Microsoft.Maui.Storage;

namespace gainz.Services
{
    internal static class DatabaseService
    {
        private static string _databaseFile;
        private static string DatabaseFile
        {
            get
            {
                if (_databaseFile == null)
                {
                    string databaseDir = Path.Combine(FileSystem.Current.AppDataDirectory, "data");
                    Directory.CreateDirectory(databaseDir);

                    _databaseFile = Path.Combine(databaseDir, "exercise_data.sqlite");
                }
                return _databaseFile;
            }
        }

        private static SQLiteConnection _connection;
        public static SQLiteConnection Connection
        {
            get
            {
                if (_connection == null)
                {
                    _connection = new SQLiteConnection(DatabaseFile);
                    _connection.CreateTable<Exercise>(); // Use the Exercise model
                }
                return _connection;
            }
        }
    }
}
