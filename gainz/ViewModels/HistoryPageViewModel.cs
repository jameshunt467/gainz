using gainz.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static gainz.App;

namespace gainz.ViewModels
{
    public class HistoryPageViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<CompletedWorkoutViewModel> CompletedWorkouts { get; set; }

        public HistoryPageViewModel()
        {
            try
            {
                // Fetch completed workouts from the database
                var completedWorkouts = DatabaseService.GetCompletedWorkouts();
                Debug.WriteLine($"[{Constants.LogTag}] Loaded {completedWorkouts.Count} completed workouts from the database.");

                // Log the data count to check if it's being retrieved properly
                Console.WriteLine($"Loaded {completedWorkouts.Count} workouts.");

                // Convert them to ViewModels for binding
                CompletedWorkouts = new ObservableCollection<CompletedWorkoutViewModel>(
                    completedWorkouts.Select(workout => new CompletedWorkoutViewModel(workout))
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading completed workouts: {ex.Message}");
            }
        }

        public void LoadCompletedWorkouts()
        {
            // Alternatively just use CompletedWorkouts?.Clear(); instead of this block
            if (CompletedWorkouts == null)
            {
                CompletedWorkouts = new ObservableCollection<CompletedWorkoutViewModel>();
            }

            // Clear the existing data
            CompletedWorkouts.Clear();

            // Fetch completed workouts from the database
            var completedWorkouts = DatabaseService.GetCompletedWorkouts();

            // Convert them to ViewModels for binding and add to the collection
            foreach (var workout in completedWorkouts)
            {
                CompletedWorkouts.Add(new CompletedWorkoutViewModel(workout));
            }

            // Log the number of workouts loaded
            //Console.WriteLine($"[gainzLog] Loaded {completedWorkouts.Count} completed workouts from the database.");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
