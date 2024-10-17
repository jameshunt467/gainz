using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace gainz.ViewModels
{
    public class ExerciseWithSetsViewModel : INotifyPropertyChanged
    {
        public string ExerciseName { get; set; }
        public string ImageUrl { get; set; }
        public int ExerciseId { get; set; }
        public ObservableCollection<CompletedSetViewModel> Sets { get; set; }

        public ICommand NavigateToExerciseDetailsCommand { get; }

        public ExerciseWithSetsViewModel()
        {
            // Initialize the command for navigation
            NavigateToExerciseDetailsCommand = new Command(async () =>
            {
                Debug.WriteLine($"[{gainz.App.Constants.LogTag}] Displaying exercise {ExerciseId}");
                if (ExerciseId != 0)
                {
                    // Navigate to the ExerciseDetailsPage with the ExerciseId
                    await Shell.Current.GoToAsync($"exercisedetails?exerciseId={ExerciseId}");
                }
                else
                {
                    // Display a popup/alert that the exercise has been deleted
                    await Application.Current.MainPage.DisplayAlert("Exercise Deleted", "This exercise has been deleted.", "OK");
                }
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
