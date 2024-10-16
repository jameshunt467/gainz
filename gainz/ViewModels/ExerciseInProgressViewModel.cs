using gainz.Models;
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
    public class ExerciseInProgressViewModel : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public string ImageUrl { get; set; }  // Image property for the exercise
        public ObservableCollection<Set> Sets { get; set; }
        public ICommand AddSetCommand { get; set; }
        public ICommand DeleteSetCommand { get; set; }
        public ICommand ToggleDropdownCommand { get; set; }

        public event Action OnSetChanged;  // Event to notify the parent ViewModel

        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                _isExpanded = value;
                OnPropertyChanged(nameof(IsExpanded));
            }
        }

        public ExerciseInProgressViewModel(Exercise exercise)
        {
            Name = exercise.Name;
            ImageUrl = exercise.ImageUrl;
            Sets = new ObservableCollection<Set>();
            AddSetCommand = new Command(AddSet);
            DeleteSetCommand = new Command<Set>(DeleteSet);
            ToggleDropdownCommand = new Command(ToggleDropdown);
            //PropertyChanged = new PropertyChangedEventHandler((sender, e) => { });

            // Subscribe to changes in each set
            Sets.CollectionChanged += (s, e) => SubscribeToSetChanges();
        }       

        private void AddSet()
        {
            var newSet = new Set { Weight = 0, Reps = 0 };

            // Ensure the OnWeightOrRepsChanged event is assigned properly
            newSet.OnWeightOrRepsChanged += HandleSetChanged;

            Sets.Add(newSet);  // Placeholder for user input
            OnPropertyChanged(nameof(Sets));
        }
        private void DeleteSet(Set set)
        {
            if (set != null)
            {
                Sets.Remove(set);
                OnPropertyChanged(nameof(Sets));  // Notify the UI to refresh after deletion
            } else
            {
                System.Diagnostics.Debug.WriteLine($"[{Constants.LogTag}] Set empty");
            }
        }

        private void SubscribeToSetChanges()
        {
            foreach (var set in Sets)
            {
                set.OnWeightOrRepsChanged += HandleSetChanged;
            }
        }

        private void HandleSetChanged()
        {
            OnSetChanged?.Invoke();  // Notify the parent when a set changes
        }

        private void ToggleDropdown()
        {
            IsExpanded = !IsExpanded;
            OnPropertyChanged(nameof(IsExpanded));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
