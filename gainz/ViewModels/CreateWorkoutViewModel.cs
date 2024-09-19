using gainz.JoinTable;
using gainz.Models;
using gainz.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gainz.ViewModels
{
    public class CreateWorkoutViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<ExerciseGroup> ExerciseGroups { get; set; }

        private string _workoutDescription;
        public string WorkoutDescription
        {
            get => _workoutDescription;
            set
            {
                _workoutDescription = value;
                OnPropertyChanged(nameof(WorkoutDescription));
            }
        }

        public CreateWorkoutViewModel()
        {
            LoadExercisesByCategory();
        }

        private void LoadExercisesByCategory()
        {
            var db = DatabaseService.Connection;

            // Get all categories
            var categories = db.Table<Category>().ToList();

            // Group exercises by CategoryId
            var groupedExercises = categories
                .Select(c =>
                {
                    c.LoadExercises(); // Lazy load exercises
                    return new ExerciseGroup(c.Name, new ObservableCollection<SelectableExercise>(c.Exercises.Select(e => new SelectableExercise { Exercise = e })));
                })
                .ToList();

            ExerciseGroups = new ObservableCollection<ExerciseGroup>(groupedExercises);
        }


        public async Task CreateWorkout()
        {
            // Get all selected exercises
            var selectedExercises = ExerciseGroups
                .SelectMany(g => g.Exercises)
                .Where(e => e.IsSelected)
                .Select(e => e.Exercise)
                .ToList();

            if (selectedExercises.Count == 0)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "No exercises selected. Please select exercises to add to the workout.", "OK");
                return;
            }

            // Prompt for workout name
            string workoutName = await Application.Current.MainPage.DisplayPromptAsync("Workout Name", "Please enter a name for your workout:");

            if (string.IsNullOrWhiteSpace(workoutName))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Workout name cannot be empty.", "OK");
                return;
            }

            // Create new workout in the database (example)
            var workout = new Workout
            {
                Name = workoutName,
                //Exercises = selectedExercises // Assuming you have a relationship between Workout and Exercise
                Description = WorkoutDescription
            };

            DatabaseService.Connection.Insert(workout); // Insert workout into the Workout table

            // Save each selected exercise to the ExerciseWorkout join table
            foreach (var exercise in selectedExercises)
            {
                var exerciseWorkout = new ExerciseWorkout
                {
                    WorkoutId = workout.Id, // Newly created workout ID
                    ExerciseId = exercise.Id // Each selected exercise ID
                };
                DatabaseService.Connection.Insert(exerciseWorkout);
            }

            // Navigate back after saving
            await Application.Current.MainPage.Navigation.PopAsync();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // Model for a selectable exercise
    public class SelectableExercise : INotifyPropertyChanged
    {
        public Exercise Exercise { get; set; }
        private bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // Model for grouping exercises by category
    public class ExerciseGroup
    {
        public string Category { get; set; }
        public ObservableCollection<SelectableExercise> Exercises { get; set; }

        public ExerciseGroup(string category, ObservableCollection<SelectableExercise> exercises)
        {
            Category = category;
            Exercises = exercises;
        }
    }
}
