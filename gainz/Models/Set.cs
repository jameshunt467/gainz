using System.ComponentModel;

namespace gainz.Models
{
    public class Set : INotifyPropertyChanged
    {
        private int _weight;
        public int Weight
        {
            get => _weight;
            set
            {
                _weight = value;
                OnPropertyChanged(nameof(Weight));
                OnWeightOrRepsChanged?.Invoke();  // Null check to avoid calling a null reference
            }
        }

        private int _reps;
        public int Reps
        {
            get => _reps;
            set
            {
                _reps = value;
                OnPropertyChanged(nameof(Reps));
                OnWeightOrRepsChanged?.Invoke();  // Null check to avoid calling a null reference
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Delegate to notify the parent ViewModel of changes
        public event Action OnWeightOrRepsChanged;
    }
}