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
using static gainz.App;

namespace gainz.ViewModels
{
    public class CompletedWorkoutViewModel : INotifyPropertyChanged
    {
        public string WorkoutName { get; set; }
        public DateTime WorkoutDate { get; set; }
        public ObservableCollection<CompletedSetViewModel> Sets { get; set; }
        public ObservableCollection<ExerciseWithSetsViewModel> Exercises { get; set; }

        private bool _isExpanded;   // !! Not currently using.  Have switched to toolkit:expander in maui
        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                _isExpanded = value;
                OnPropertyChanged(nameof(IsExpanded));
            }
        }

        public CompletedWorkoutViewModel(CompletedWorkout workout)
        {
            WorkoutName = workout.WorkoutName;
            WorkoutDate = workout.WorkoutDate;

            // Initialize the Sets collection using CompletedSet data
            Sets = new ObservableCollection<CompletedSetViewModel>(
                workout.Sets.Select(set => new CompletedSetViewModel(set))
            );

            // Group sets by ExerciseId and build a collection of ExerciseWithSetsViewModel
            var exerciseGroups = workout.Sets
                .GroupBy(set => set.ExerciseId)
                .Select(group =>
                {
                    var exercise = DatabaseService.GetExerciseWithId(group.Key);

                    // Check if the exercise exists before creating the ExerciseWithSetsViewModel
                    if (exercise != null)
                    {
                        return new ExerciseWithSetsViewModel
                        {
                            ExerciseName = exercise.Name,
                            ImageUrl = exercise.ImageUrl,
                            ExerciseId = exercise.Id,
                            Sets = new ObservableCollection<CompletedSetViewModel>(
                                group.Select(set => new CompletedSetViewModel(set))
                            )
                        };
                    }
                    else
                    {
                        // Handle the case where the exercise was deleted or is null
                        Debug.WriteLine($"[Warning] Exercise with ID {group.Key} not found. Skipping this exercise.");
                        return new ExerciseWithSetsViewModel
                        {
                            ExerciseName = "Deleted exercise",
                            ImageUrl = "missing_image.png",
                            ExerciseId = 0,
                            Sets = new ObservableCollection<CompletedSetViewModel>()
                        };
                    }
                })
                .Where(exerciseWithSets => exerciseWithSets != null);  // Filter out null exercises

            Exercises = new ObservableCollection<ExerciseWithSetsViewModel>(exerciseGroups);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
