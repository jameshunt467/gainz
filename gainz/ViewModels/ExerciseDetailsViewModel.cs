using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Input;
using AndroidX.Emoji2.Text.FlatBuffer;
using gainz.Models;


//using gainz.Models;
//using static gainz.App;
using gainz.Services;
using Microcharts;
using Microsoft.Maui.Controls;
using SkiaSharp;
using Constants = gainz.App.Constants;

namespace gainz.ViewModels
{
    public class ExerciseDetailsViewModel : INotifyPropertyChanged
    {
        //private ObservableCollection<Exercise> _exercises;  // Same as in ExerciseDetailsPage
        private Exercise _selectedExercise; // Same as in ExerciseDetailsPage
        private string _newCategory;
        private string _selectedCategory;
        private ObservableCollection<string> _categories;

        // For graphing functionality
        public bool IsVolumeSelected { get; set; }          // TODO: Delete this (we're not using this binding)
        public bool IsHighestWeightSelected { get; set; }   // TODO: Delete this (we're not using this binding)
        public ObservableCollection<CompletedWorkout> Workouts { get; set; }
        //public Chart Chart { get; private set; }
        private Chart _chart;
        private double _chartWidth = 1000; // Default width
        private const double WidthStep = 250;
        private const double MinWidth = 210; // minimum width
        private const double MaxWidth = 1000; // maximum width
        public double ChartWidth
        {
            get => _chartWidth;
            set
            {
                if (_chartWidth != value)
                {
                    _chartWidth = value;
                    OnPropertyChanged(nameof(ChartWidth));
                }
            }
        }

        public Chart Chart
        {
            get => _chart;
            set
            {
                _chart = value;
                OnPropertyChanged(nameof(Chart));
            }
        }
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

        public int CategoryId
        {
            get => Exercise.CategoryId;
            set
            {
                if (Exercise.CategoryId != value)
                {
                    Exercise.CategoryId = value;
                    OnPropertyChanged(nameof(CategoryId));
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
        public ExerciseDetailsViewModel(int id)
        {
            // Fetch exercises from the database
            //var exercisesFromDb = DatabaseService.GetAllExercises();
            //_exercises = new ObservableCollection<Exercise>(exercisesFromDb);

            Debug.WriteLine($"[{Constants.LogTag}] ExerciseDetailsViewModel initializing with id {id}");
            _selectedExercise = DatabaseService.GetExerciseWithId(id);

            LoadCategories(); // Load categories for autofill
            LoadExerciseDetails(); // Load selected exercise details

            SaveExerciseCommand = new Command(OnSaveExercise);
            // Initialize the command for deleting an exercise
            DeleteExerciseCommand = new Command(OnDeleteExercise);

            // Graphing functionality initialization
            IsVolumeSelected = true; // Default to Volume
            IsHighestWeightSelected = false;

            // Update Chart when selection changes
            UpdateChart(IsVolumeSelected);
        }


        public void DecreaseWidth()
        {
            //Debug.WriteLine($"[{Constants.LogTag}] ChartWidth: {ChartWidth}");
            if (ChartWidth - WidthStep >= MinWidth)
            {
                ChartWidth -= WidthStep;
            }
        }

        public void IncreaseWidth()
        {
            //Debug.WriteLine($"[{Constants.LogTag}] ChartWidth: {ChartWidth}");
            if (ChartWidth + WidthStep <= MaxWidth)
            {
                ChartWidth += WidthStep;
            }
        }

        public void UpdateChart(bool isVolume)
        {
            // Pull all completed workouts again 
            Workouts = new ObservableCollection<CompletedWorkout>(DatabaseService.GetCompletedWorkouts());
            Debug.WriteLine($"[{Constants.LogTag}] Updating chart ..");
            // Logic to switch between Volume and Highest Weight
            Chart = isVolume ? CreateVolumeChart() : CreateHighestWeightChart();

            String selectionString = isVolume ? "Volume Chart" : "Highest Weight Chart";
            Debug.WriteLine($"[{Constants.LogTag}] Chart updated.  Drawing {selectionString}");

            OnPropertyChanged(nameof(Chart));
        }

        private Chart CreateVolumeChart()
        {
            try
            {
                // Filter sets for the selected exercise and sum their volumes per workout
                var entries = Workouts.Select(workout =>
                {
                    int exerciseVolume = workout.Sets
                        .Where(set => set.ExerciseId == _selectedExercise.Id)
                        .Sum(set => set.Weight * set.Reps);

                    return new ChartEntry(exerciseVolume)
                    {
                        Label = workout.WorkoutDate.ToString("MMM dd"),
                        ValueLabel = exerciseVolume.ToString(),
                        Color = SKColors.Cyan
                    };
                }).ToArray();

                Debug.WriteLine($"[{Constants.LogTag}] Number of VolumeChart data points: {entries.Length}");

                return new LineChart
                {
                    Entries = entries,
                    LineMode = LineMode.Straight,
                    LineSize = 4,
                    PointMode = PointMode.Circle,
                    PointSize = 10,
                    BackgroundColor = SKColors.White, // Explicitly set the background color
                    LabelTextSize = 30, // Increase label text size for better readability
                    LabelOrientation = Orientation.Horizontal, // Ensure x-axis labels are horizontal
                    ValueLabelOrientation = Orientation.Horizontal, // Ensure value labels are horizontal
                    // EnableYLabels = true, // Enable y-axis labels for clarity
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[{Constants.LogTag}] Error creating volume chart: {ex.Message}");
                Application.Current.MainPage.DisplayAlert("Error", $"Failed to create volume chart: {ex.Message}", "OK");
                return null;
            }
        }

        private Chart CreateHighestWeightChart()
        {
            try
            {
                // Determine the heaviest set for the selected exercise per workout
                var entries = Workouts.Select(workout =>
                {
                    int heaviestWeight = workout.Sets
                        .Where(set => set.ExerciseId == _selectedExercise.Id)
                        .OrderByDescending(set => set.Weight)
                        .Select(set => set.Weight)
                        .FirstOrDefault();

                    return new ChartEntry(heaviestWeight)
                    {
                        Label = workout.WorkoutDate.ToString("MMM dd"),
                        ValueLabel = heaviestWeight.ToString(),
                        Color = SKColor.Parse("#FF6347")
                    };
                }).ToArray();

                Debug.WriteLine($"Number of HighestWeight data points: {entries.Length}");
                return new LineChart
                {
                    Entries = entries,
                    LineMode = LineMode.Straight,
                    LineSize = 4,
                    PointMode = PointMode.Circle,
                    PointSize = 10,
                    BackgroundColor = SKColors.White, // Explicitly set the background color
                    LabelTextSize = 30, // Increase label text size for better readability
                    LabelOrientation = Orientation.Horizontal, // Ensure x-axis labels are horizontal
                    ValueLabelOrientation = Orientation.Horizontal, // Ensure value labels are horizontal
                    // EnableYLabels = true, // Enable y-axis labels for clarity
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[{Constants.LogTag}] Error creating highest weight chart: {ex.Message}");
                Application.Current.MainPage.DisplayAlert("Error", $"Failed to create highest weight chart: {ex.Message}", "OK");
                return null;
            }
        }

        private void LoadCategories()
        {
            // Fetch categories from the database
            var categoriesFromDb = DatabaseService.GetAllCategories().Select(c => c.Name).ToList();
            Categories = new ObservableCollection<string>(categoriesFromDb);

            // If the current category is not in the list, set it to "Uncategorized"
            var currentCategory = DatabaseService.GetCategoryById(_selectedExercise.CategoryId)?.Name;
            if (!string.IsNullOrWhiteSpace(currentCategory) && !Categories.Contains(currentCategory))
            {
                currentCategory = "Uncategorized";
            }

            // Set the current category in the Picker if it exists
            SelectedCategory = Categories.Contains(currentCategory) ? currentCategory : null;
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

            int categoryIdToSave;

            if (!string.IsNullOrWhiteSpace(newCategory))
            {
                // Add new category to database if it doesn't exist
                if (!Categories.Contains(newCategory))
                {
                    var newCategoryObj = new Category { Name = newCategory };
                    DatabaseService.Connection.Insert(newCategoryObj);
                    Categories.Add(newCategory);
                    categoryIdToSave = newCategoryObj.Id; // Get the ID of the newly inserted category
                }
                else
                {
                    // Retrieve the ID of the existing category
                    categoryIdToSave = DatabaseService.GetCategoryByName(newCategory).Id;
                }
            }
            else
            {
                // Retrieve the ID of the selected category
                categoryIdToSave = DatabaseService.GetCategoryByName(selectedCategory).Id;
            }

            // Update the selected exercise
            _selectedExercise.Name = name;
            _selectedExercise.Description = description;
            _selectedExercise.CategoryId = categoryIdToSave;

            // Debugging: Output the updated exercise details
            Console.WriteLine($"Updated Exercise: Name = {_selectedExercise.Name}, Description = {_selectedExercise.Description}, Category = {_selectedExercise.CategoryId}");

            // Update the exercise in the SQLite database
            DatabaseService.Connection.Update(_selectedExercise);

            await Shell.Current.GoToAsync("..");
        }

        private async void OnDeleteExercise()
        {
            Debug.WriteLine($"Attempting to delete {Exercise.Name}");
            bool confirmDelete = await Application.Current.MainPage.DisplayAlert(
                "Confirm Delete",
                $"Are you sure you want to delete the exercise: {Exercise.Name}?",
                "Yes", "No");

            if (confirmDelete)
            {
                try
                {
                    // Remove from the SQLite database
                    var db = DatabaseService.Connection;
                    db.Delete(_selectedExercise);

                    // Logic to delete the exercise (remove from the data source)
                    //_exercises.Remove(_selectedExercise);

                    Debug.WriteLine($"Going back to BankPage now");

                    // Notify that the exercise has been deleted, navigate back to BankPage, etc.vigate back to BankPage
                    await Shell.Current.GoToAsync("..");
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
