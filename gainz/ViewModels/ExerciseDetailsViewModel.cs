using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Input;

//using gainz.Models;
using gainz.Services;
using Microsoft.Maui.Controls;

namespace gainz.ViewModels
{
    public class ExerciseDetailsViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Exercise> _exercises;  // Same as in ExerciseDetailsPage
        private Exercise _selectedExercise; // Same as in ExerciseDetailsPage
        private string _newCategory;
        private string _selectedCategory;
        private ObservableCollection<string> _categories;

        public ObservableCollection<string> Categories
        {
            get => _categories;
            set
            {
                _categories = value;
                OnPropertyChanged(nameof(Categories));
            }
        }

        public string NewCategory
        {
            get => _newCategory;
            set
            {
                _newCategory = value;
                if (!string.IsNullOrWhiteSpace(_newCategory))
                {
                    SelectedCategory = null; // Clear selected category if new one is being typed
                }
                OnPropertyChanged(nameof(NewCategory));
            }
        }

        public string SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                if (!string.IsNullOrWhiteSpace(_selectedCategory))
                {
                    NewCategory = string.Empty; // Clear new category input if an existing category is selected
                }
                OnPropertyChanged(nameof(SelectedCategory));
            }
        }

        public Exercise Exercise
        {
            get => _selectedExercise;
            set
            {
                _selectedExercise = value;
                OnPropertyChanged(nameof(Exercise));
            }
        }
        // Command for deleting the exercise
        public Command DeleteExerciseCommand { get; }

        // Command for saving the exercise
        public Command SaveExerciseCommand { get; }

        public string Name
        {
            get => Exercise.Name;
            set
            {
                if (Exercise.Name != value)
                {
                    Exercise.Name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public string Description
        {
            get => Exercise.Description;
            set
            {
                if (Exercise.Description != value)
                {
                    Exercise.Description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }

        public string Category
        {
            get => Exercise.Category;
            set
            {
                if (Exercise.Category != value)
                {
                    Exercise.Category = value;
                    OnPropertyChanged(nameof(Category));
                }
            }
        }

        public string ImageUrl
        {
            get => Exercise.ImageUrl;
            set
            {
                if (Exercise.ImageUrl != value)
                {
                    Exercise.ImageUrl = value;
                    OnPropertyChanged(nameof(ImageUrl));
                }
            }
        }

        // Same constructor as in ExerciseDetailsPage - we need to pass the selected exercise and the list of exercises
        public ExerciseDetailsViewModel(ObservableCollection<Exercise> exercises, Exercise selectedExercise)
        {
            _exercises = exercises;
            _selectedExercise = selectedExercise;

            LoadCategories(); // Load categories for autofill
            LoadExerciseDetails(); // Load selected exercise details

            SaveExerciseCommand = new Command(OnSaveExercise);
            // Initialize the command for deleting an exercise
            DeleteExerciseCommand = new Command(OnDeleteExercise);
        }

        private void LoadCategories()
        {
            // Fetch categories from the database
            var categoriesFromDb = DatabaseService.GetAllCategories().Select(c => c.Name).ToList();
            Categories = new ObservableCollection<string>(categoriesFromDb);

            // If the current category is not in the list, set it to "Uncategorized"
            if (!string.IsNullOrWhiteSpace(_selectedExercise.Category) && !Categories.Contains(_selectedExercise.Category))
            {
                _selectedExercise.Category = "Uncategorized";
            }

            // Set the current category in the Picker if it exists
            SelectedCategory = Categories.Contains(_selectedExercise.Category) ? _selectedExercise.Category : null;
        }

        private void LoadExerciseDetails()
        {
            // Load exercise details
            OnPropertyChanged(nameof(Exercise));
        }

        private async void OnSaveExercise()
        {
            var name = Exercise.Name;
            var description = Exercise.Description;
            var selectedCategory = SelectedCategory;
            var newCategory = NewCategory;

            // Corrected if condition to check if either category is provided
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(description) || (string.IsNullOrWhiteSpace(selectedCategory) && string.IsNullOrWhiteSpace(newCategory)))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Please fill in all fields." + newCategory, "OK");
                return;
            }

            string categoryToSave = !string.IsNullOrWhiteSpace(newCategory) ? newCategory : selectedCategory;

            if (!string.IsNullOrWhiteSpace(newCategory) && !Categories.Contains(newCategory))
            {
                // Add new category to database and Picker
                DatabaseService.AddCategory(newCategory);
                Categories.Add(newCategory);
            }

            // Update the selected exercise
            _selectedExercise.Name = name;
            _selectedExercise.Description = description;
            _selectedExercise.Category = categoryToSave;

            // Debugging: Output the updated exercise details
            Console.WriteLine($"Updated Exercise: Name = {_selectedExercise.Name}, Description = {_selectedExercise.Description}, Category = {_selectedExercise.Category}");

            // Update the exercise in the SQLite database
            DatabaseService.Connection.Update(_selectedExercise);

            await Application.Current.MainPage.Navigation.PopAsync();
        }

        private async void OnDeleteExercise()
        {
            bool confirmDelete = await Application.Current.MainPage.DisplayAlert(
                "Confirm Delete",
                $"Are you sure you want to delete the exercise: {Exercise.Name}?",
                "Yes", "No");

            if (confirmDelete && _exercises.Contains(_selectedExercise))
            {
                try
                {
                    // Remove from the SQLite database
                    var db = DatabaseService.Connection;
                    db.Delete(_selectedExercise);

                    // Logic to delete the exercise (remove from the data source)
                    _exercises.Remove(_selectedExercise);

                    // Notify that the exercise has been deleted, navigate back to BankPage, etc.
                    await Application.Current.MainPage.Navigation.PopAsync();
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", $"Failed to delete exercise: {ex.Message}", "OK");
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        // INotifyPropertyChanged implementation here...
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
