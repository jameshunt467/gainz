using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gainz.Services;
using SQLite;

namespace gainz
{
    public class Exercise : INotifyPropertyChanged
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        //[MaxLength(100)]
        private string name;
        public string Name
        {
            get => name;
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        private string description;
        public string Description
        {
            get => description;
            set
            {
                if (description != value)
                {
                    description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }

        private string category;
        public string Category
        {
            get => category;
            set
            {
                if (category != value)
                {
                    category = value;
                    OnPropertyChanged(nameof(Category));
                }
            }
        }

        private string imageUrl;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string ImageUrl
        {
            get => imageUrl;
            set
            {
                if (imageUrl != value)
                {
                    imageUrl = value;
                    OnPropertyChanged(nameof(ImageUrl));
                }
            }
        }
    }

    public class BankViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Exercise> Exercises { get; set; }

        public BankViewModel()
        {
            // Load exercises from the database
            LoadExercisesFromDatabase();
        }

        public void LoadExercisesFromDatabase()
        {
            try
            {
                var db = DatabaseService.Connection;

                // Clear the collection first to avoid duplicates
                if (Exercises == null)
                {
                    Exercises = new ObservableCollection<Exercise>();
                }
                Exercises.Clear();

                // Read all exercises from the SQLite database
                var exercisesFromDb = db.Table<Exercise>().ToList();

                if (exercisesFromDb == null)
                {
                    Console.WriteLine("Error: No exercises found in the database.");
                    return; // Exit early if the database query failed
                }

                // Get all categories from the database to check if they still exist
                var existingCategories = DatabaseService.GetAllCategories().Select(c => c.Name).ToHashSet();

                if (existingCategories == null)
                {
                    Console.WriteLine("Error: No categories found in the database.");
                    return; // Exit early if the category query failed
                }

                // Add each exercise to the ObservableCollection
                foreach (var exercise in exercisesFromDb)
                {
                    if (exercise == null)
                    {
                        Console.WriteLine("Error: Encountered a null exercise in the database.");
                        continue; // Skip any null exercises
                    }

                    // If the category does not exist, set it to "Uncategorized"
                    if (!string.IsNullOrWhiteSpace(exercise.Category) && !existingCategories.Contains(exercise.Category))
                    {
                        exercise.Category = "Uncategorized";
                        db.Update(exercise); // Update the exercise in the database to save this change
                    }

                    Exercises.Add(exercise);
                }
            }
            catch (Exception ex)
            {
                // Log the exception message and stack trace for debugging
                Console.WriteLine($"Error loading exercises: {ex.Message}");
                Console.WriteLine(ex.StackTrace);

                // Display error message to the user (optional)
                Application.Current.MainPage.DisplayAlert("Error", "Failed to load exercises.", "OK");
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        // INotifyPropertyChanged implementation here...
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
