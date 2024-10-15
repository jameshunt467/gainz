using gainz.JoinTable;
using gainz.Models;
using gainz.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static gainz.App;

namespace gainz.ViewModels
{
    public class WorkoutDetailsViewModel : INotifyPropertyChanged
    {
        public int WorkoutId { get; set; } // Property to hold the workoutId
        public string WorkoutName { get; set; }
        public string WorkoutDescription { get; set; }
        public ObservableCollection<Exercise> Exercises { get; set; }

        public ICommand StartWorkoutCommand { get; set; }
        public ICommand DeleteWorkoutCommand { get; set; }
        public ICommand DeleteExerciseCommand { get; set; }
        public ICommand AddExerciseCommand { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public WorkoutDetailsViewModel()
        {
            // Initialize the commands
            StartWorkoutCommand = new Command(StartWorkout);
            DeleteWorkoutCommand = new Command(DeleteWorkout);
            DeleteExerciseCommand = new Command<Exercise>(DeleteExercise);
            AddExerciseCommand = new Command(AddExercise);
        }

        // Method to load workout details using the WorkoutId
        public void LoadWorkoutDetails()
        {
            // Fetch workout details using the WorkoutId
            var workout = DatabaseService.GetWorkoutWithExercises(WorkoutId);

            if (workout != null)
            {
                WorkoutName = workout.Name;
                WorkoutDescription = workout.Description;
                Exercises = new ObservableCollection<Exercise>(workout.Exercises);

                // Notify the view that properties have changed
                OnPropertyChanged(nameof(WorkoutName));
                OnPropertyChanged(nameof(WorkoutDescription));
                OnPropertyChanged(nameof(Exercises));
            }
            Debug.WriteLine($"[{Constants.LogTag}] LoadWorkoutDetails complete.  Fetched Workout({workout.Name}).");
        }

        private async void StartWorkout()
        {
            // Logic to start the workout
            await Shell.Current.GoToAsync($"workoutinprogress?workoutId={WorkoutId}");
        }

        private async void DeleteWorkout()
        {
            bool confirmDelete = await Application.Current.MainPage.DisplayAlert(
                "Confirm Delete",
                $"Are you sure you want to delete the workout: {WorkoutName}?",
                "Yes", "No");

            if (confirmDelete)
            {
                // Delete the workout from the database
                var workout = DatabaseService.Connection.Table<Workout>().FirstOrDefault(w => w.Name == WorkoutName);
                if (workout != null)
                {
                    DatabaseService.Connection.Delete(workout);
                }

                // Navigate back
                await Application.Current.MainPage.Navigation.PopAsync();
            }
        }

        private async void DeleteExercise(Exercise exercise)
        {
            bool confirmDelete = await Application.Current.MainPage.DisplayAlert(
                "Confirm Delete",
                $"Are you sure you want to delete the exercise: {exercise.Name}?",
                "Yes", "No");

            if (confirmDelete)
            {
                Exercises.Remove(exercise);

                // Update the join table and remove the exercise from the workout
                var exerciseWorkout = DatabaseService.Connection.Table<ExerciseWorkout>()
                    .FirstOrDefault(ew => ew.WorkoutId == WorkoutId && ew.ExerciseId == exercise.Id);
                if (exerciseWorkout != null)
                {
                    DatabaseService.Connection.Delete(exerciseWorkout);
                }
            }
        }

        private async void AddExercise()
        {
            // Navigate to ExerciseSelectionPage and pass the workout ID
            await Shell.Current.GoToAsync($"exerciseselection?workoutId={WorkoutId}");
        }

        // Implement the property changed notification and command logic here
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
