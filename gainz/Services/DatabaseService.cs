using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using SQLite;
using Microsoft.Maui.Storage;
using gainz.Models;

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
                    _connection.CreateTable<Category>();
                }
                return _connection;
            }
        }

        // start of CATEGORY METHODS

        // Method to get all categories
        public static List<Category> GetAllCategories()
        {
            return Connection.Table<Category>().ToList();
        }

        // Method to add a new category
        public static void AddCategory(string categoryName)
        {
            Connection.Insert(new Category { Name = categoryName });
        }

        // Method to delete a category
        public static void DeleteCategory(Category category)
        {
            Connection.Delete(category);
        }

        // end of CATEGORY METHODS
    }
}
