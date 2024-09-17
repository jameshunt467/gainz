using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gainz
{
    public class ExerciseDetailsViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Exercise> _exercises;  // Same as in ExerciseDetailsPage
        private Exercise _selectedExercise; // Same as in ExerciseDetailsPage
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

        public string Category
        {
            get => Exercise.Category;
            set
            {
                if (Exercise.Category != value)
                {
                    Exercise.Category = value;
                    OnPropertyChanged(nameof(Category));
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
        public ExerciseDetailsViewModel(ObservableCollection<Exercise> exercises, Exercise selectedExercise)
        {
            _exercises = exercises;
            _selectedExercise = selectedExercise;

            // Initialize the command for deleting an exercise
            DeleteExerciseCommand = new Command(OnDeleteExercise);
        }

        private async void OnDeleteExercise()
        {
            bool confirmDelete = await Application.Current.MainPage.DisplayAlert(
                "Confirm Delete",
                $"Are you sure you want to delete the exercise: {Exercise.Name}?",
                "Yes", "No");

            if (confirmDelete && _exercises.Contains(_selectedExercise))
            {
                // Logic to delete the exercise (remove from the data source)
                _exercises.Remove(_selectedExercise);
                // Notify that the exercise has been deleted, navigate back to BankPage, etc.
                await Application.Current.MainPage.Navigation.PopAsync();
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
