using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using SQLite;
using Microsoft.Maui.Storage;
using gainz.Models;
using gainz.JoinTable;

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
                    _connection.CreateTable<Workout>();
                    _connection.CreateTable<ExerciseWorkout>();

                    // Enable foreign key support (if supported by the SQLite version)
                    _connection.Execute("PRAGMA foreign_keys = ON;");
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

        public static Category GetCategoryByName(string name)
        {
            return Connection.Table<Category>().FirstOrDefault(c => c.Name == name);
        }
        public static Category GetCategoryById(int id)
        {
            return Connection.Table<Category>().FirstOrDefault(c => c.Id == id);
        }

        // end of CATEGORY METHODS

        // start of ExerciseWorkout

        public static Workout GetWorkoutWithExercises(int workoutId)
        {
            var workout = Connection.Table<Workout>().FirstOrDefault(w => w.Id == workoutId);

            if (workout != null)
            {
                // Get the exercise IDs associated with this workout
                var exerciseIds = Connection.Table<ExerciseWorkout>()
                    .Where(ew => ew.WorkoutId == workoutId)
                    .Select(ew => ew.ExerciseId)
                    .ToList();

                // Retrieve the actual exercise objects
                var exercises = Connection.Table<Exercise>()
                    .Where(e => exerciseIds.Contains(e.Id))
                    .ToList();

                workout.Exercises = exercises;
            }

            return workout;
        }

        // end of ExerciseWorkout

        // start of EXERCISES

        // Method to get all exercises
        public static List<Exercise> GetAllExercises()
        {
            return Connection.Table<Exercise>().ToList();
        }

        public static Exercise GetExerciseWithId(int id)
        {
            return Connection.Table<Exercise>().FirstOrDefault(e => e.Id == id);
        }

        // end of EXERCISES
    }
}
