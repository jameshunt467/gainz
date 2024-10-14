using gainz.JoinTable;
using gainz.Services;
using gainz.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace gainz.ViewModels
{
    public class ExerciseSelectionViewModel : BindableObject
    {
        public ObservableCollection<SelectableExercise> AvailableExercises { get; set; }

        public ICommand AddSelectedExercisesCommand { get; set; }

        private int _workoutId;

        public ExerciseSelectionViewModel(int workoutId)
        {
            _workoutId = workoutId;
            AvailableExercises = new ObservableCollection<SelectableExercise>();
            LoadAvailableExercises();

            AddSelectedExercisesCommand = new Command(AddSelectedExercises);
        }

        // Load exercises not already in the workout
        private void LoadAvailableExercises()
        {
            var allExercises = DatabaseService.Connection.Table<Exercise>().ToList();
            var workoutExercises = DatabaseService.Connection.Table<ExerciseWorkout>()
                .Where(ew => ew.WorkoutId == _workoutId)
                .Select(ew => ew.ExerciseId)
                .ToList();

            var availableExercises = allExercises.Where(e => !workoutExercises.Contains(e.Id));

            foreach (var exercise in availableExercises)
            {
                AvailableExercises.Add(new SelectableExercise { Exercise = exercise });
            }
        }

        // Add selected exercises to the workout
        private async void AddSelectedExercises()
        {
            var selectedExercises = AvailableExercises.Where(e => e.IsSelected).Select(e => e.Exercise).ToList();

            foreach (var exercise in selectedExercises)
            {
                var exerciseWorkout = new ExerciseWorkout
                {
                    WorkoutId = _workoutId,
                    ExerciseId = exercise.Id
                };
                DatabaseService.Connection.Insert(exerciseWorkout);
            }

            await Application.Current.MainPage.Navigation.PopAsync(); // Return to WorkoutDetailsPage
        }
    }
}
