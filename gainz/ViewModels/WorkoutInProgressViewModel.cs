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
using System.Timers;
using System.Windows.Input;
using static gainz.App;

/*
 * The ViewModel will handle the stopwatch, track sets and allow users to add new sets. 
 * It will also manage the list of exercises in the workout and the volume lifted
 */

namespace gainz.ViewModels
{
    public class WorkoutInProgressViewModel : INotifyPropertyChanged
    {
        public string WorkoutName { get; set; }
        public string ElapsedTime { get; set; }
        public string TotalWeight { get; set; }
        public string TotalSets { get; set; }
        public ObservableCollection<ExerciseInProgressViewModel> Exercises { get; set; }

        // Already importing system.timers so not sure why I need to be doing this
        private System.Timers.Timer _timer;
        private DateTime _startTime;

        public ICommand ExitWorkoutCommand { get; set; }
        public ICommand FinishWorkoutCommand { get; set; }

        public WorkoutInProgressViewModel(int workoutId)
        {
            // Load workout data from the database
            var workout = DatabaseService.GetWorkoutWithExercises(workoutId);
            WorkoutName = workout.Name;
            Exercises = new ObservableCollection<ExerciseInProgressViewModel>(
                workout.Exercises.Select(e => new ExerciseInProgressViewModel(e))
            );

            // Initialize commands
            ExitWorkoutCommand = new Command(ExitWorkout);
            FinishWorkoutCommand = new Command(FinishWorkout);

            // Subscribe to changes in each exercise
            foreach (var exercise in Exercises)
            {
                exercise.OnSetChanged += RecalculateTotals;
            }

            // Start the timer
            _startTime = DateTime.Now;
            _timer = new System.Timers.Timer(1000);
            _timer.Elapsed += OnTimedEvent;
            _timer.Start();
        }

        private void RecalculateTotals()
        {
            int totalWeight = 0;
            int totalSets = 0;

            foreach (var exercise in Exercises)
            {
                totalWeight += exercise.Sets.Sum(set => set.Weight * set.Reps);
                totalSets += exercise.Sets.Count;
            }

            TotalWeight = $"{totalWeight} KG";
            TotalSets = $"{totalSets} Sets";

            OnPropertyChanged(nameof(TotalWeight));
            OnPropertyChanged(nameof(TotalSets));
        }

        // Update elapsed time every second
        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            TimeSpan elapsed = DateTime.Now - _startTime;
            ElapsedTime = $"{elapsed.Minutes:D2}:{elapsed.Seconds:D2}";
            OnPropertyChanged(nameof(ElapsedTime));
        }

        private async void ExitWorkout()
        {
            bool answer = await Application.Current.MainPage.DisplayAlert("Exit Workout", "Are you sure you want to exit without saving?", "Yes", "No");
            if (answer)
            {
                // Exit without saving
                Application.Current.MainPage.Navigation.PopAsync();
            }
        }

        private async void FinishWorkout()
        {
            bool answer = await Application.Current.MainPage.DisplayAlert("Finish Workout", "Are you sure you want to finish this workout?", "Yes", "No");
            if (answer)
            {
                // Save workout details and finish
                _timer.Stop();
                SaveWorkoutDetails();
                Application.Current.MainPage.Navigation.PopAsync();
            }
        }

        private void SaveWorkoutDetails()
        {
            var completedWorkout = new CompletedWorkout
            {
                WorkoutName = this.WorkoutName,
                WorkoutDate = DateTime.Now,
                TotalVolume = CalculateTotalVolume(),
                TotalSets = CalculateTotalSets(),
                Sets = new List<CompletedSet>()
            };

            // Save the workout first
            DatabaseService.SaveCompletedWorkout(completedWorkout);

            foreach (var exerciseViewModel in Exercises)
            {
                foreach (var set in exerciseViewModel.Sets)
                {
                    var completedSet = new CompletedSet
                    {
                        Weight = set.Weight,
                        Reps = set.Reps,
                        CompletedWorkoutId = completedWorkout.Id,
                        ExerciseId = exerciseViewModel.Id  // Link to the existing Exercise model
                    };

                    // Save the set
                    DatabaseService.SaveCompletedSet(completedSet);

                    completedWorkout.Sets.Add(completedSet);
                }
            }
        }


        // Method to calculate the total volume (weight * reps for all exercises)
        private int CalculateTotalVolume()
        {
            int totalVolume = 0;

            foreach (var exercise in Exercises)
            {
                foreach (var set in exercise.Sets)
                {
                    totalVolume += set.Weight * set.Reps;
                }
            }

            return totalVolume;
        }

        // Method to calculate the total number of sets across all exercises
        private int CalculateTotalSets()
        {
            int totalSets = 0;

            foreach (var exercise in Exercises)
            {
                totalSets += exercise.Sets.Count;
            }

            return totalSets;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    //public class ExerciseInProgress : INotifyPropertyChanged
    //{
    //    public string Name { get; set; }
    //    public ObservableCollection<Set> Sets { get; set; }
    //    public ICommand AddSetCommand { get; set; }
    //    public bool IsExpanded { get; set; }

    //    public ExerciseInProgress(Exercise exercise)
    //    {
    //        Name = exercise.Name;
    //        Sets = new ObservableCollection<Set>();
    //        AddSetCommand = new Command(AddSet);
    //    }

    //    private void AddSet()
    //    {
    //        // Logic to add a set
    //        Sets.Add(new Set { Weight = 0, Reps = 0 });  // Placeholder for user input
    //        OnPropertyChanged(nameof(Sets));
    //    }

    //    public event PropertyChangedEventHandler PropertyChanged;
    //    protected virtual void OnPropertyChanged(string propertyName)
    //    {
    //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    //    }
    //}
}
