using gainz.Models;
using gainz.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gainz.ViewModels
{
    // holds data about the set, including the Exercise it belongs to
    // represents a single set, not a collection of sets.
    public class CompletedSetViewModel : INotifyPropertyChanged
    {
        public string ExerciseName { get; set; }
        public string ImageUrl { get; set; }
        public int Weight { get; set; }
        public int Reps { get; set; }

        public CompletedSetViewModel(CompletedSet set)
        {
            // Retrieve the associated Exercise using the ExerciseId from the set
            var exercise = DatabaseService.GetExerciseWithId(set.ExerciseId);

            if (exercise != null)
            {
                ExerciseName = exercise.Name;
                ImageUrl = exercise.ImageUrl;
            }
            else
            {
                ExerciseName = "Unknown Exercise";  // Fallback if exercise is not found
                ImageUrl = "missing_image.png";  // Fallback image if not found
            }

            Weight = set.Weight;
            Reps = set.Reps;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
