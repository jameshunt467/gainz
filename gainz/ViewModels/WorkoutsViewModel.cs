using gainz.Models;
using gainz.Pages;
using gainz.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static gainz.App;

namespace gainz.ViewModels
{
    public class WorkoutsViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Workout> Workouts { get; set; } = new ObservableCollection<Workout>();
        public ICommand WorkoutTappedCommand { get; set; } // Command for handling workout taps

        public WorkoutsViewModel()
        {
            LoadWorkouts();
            WorkoutTappedCommand = new Command<Workout>(OnWorkoutTapped); // Initialize command
        }

        private async void OnWorkoutTapped(Workout workout)
        {
            if (workout != null)
            {
                // Navigate to the WorkoutDetailsPage using Shell routing and pass the workout ID as a parameter
                try
                {
                    //await Shell.Current.GoToAsync($"//workoutdetails?WorkoutId={workout.Id}");
                    await Application.Current.MainPage.Navigation.PushAsync(new WorkoutDetailsPage(workout.Id));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Navigation Error: {ex.Message}");
                    await Application.Current.MainPage.DisplayAlert("Navigation Error", ex.Message, "OK");
                }

                //await Navigation.PushAsync(new WorkoutDetailsPage(workout.Id));
            }
        }

        private void LoadWorkouts()
        {
            // Fetch saved workouts from the database
            var savedWorkouts = DatabaseService.Connection.Table<Workout>().ToList();

            // Add them to the ObservableCollection
            Workouts.Clear();
            foreach (var workout in savedWorkouts)
            {
                Workouts.Add(workout);
                // Debug code to verify that the exercises are being loaded
                System.Diagnostics.Debug.WriteLine($"[{Constants.LogTag}] Loaded Workout: {workout.Name}, Description: {workout.Description}");
            }
            // Output the number of fetched workouts
            System.Diagnostics.Debug.WriteLine($"[{Constants.LogTag}] Finished loading workouts. Total count: {savedWorkouts.Count}");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
