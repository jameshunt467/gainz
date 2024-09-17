using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gainz
{
    public class Exercise : INotifyPropertyChanged
    {
        private string name;
        public string Name
        {
            get => name;
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        private string description;
        public string Description
        {
            get => description;
            set
            {
                if (description != value)
                {
                    description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }

        private string category;
        public string Category
        {
            get => category;
            set
            {
                if (category != value)
                {
                    category = value;
                    OnPropertyChanged(nameof(Category));
                }
            }
        }

        private string imageUrl;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string ImageUrl
        {
            get => imageUrl;
            set
            {
                if (imageUrl != value)
                {
                    imageUrl = value;
                    OnPropertyChanged(nameof(ImageUrl));
                }
            }
        }
    }

    public class BankViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Exercise> Exercises { get; set; }

        public BankViewModel()
        {
            // Populate the list with sample data (replace with actual data later)
            Exercises = new ObservableCollection<Exercise>
            {
                new Exercise { Name = "Bench Press", Category = "Chest", ImageUrl = "benchpress.png", Description = "A chest exercise..." },
                new Exercise { Name = "Squat", Category = "Legs", ImageUrl = "squat.png", Description = "A leg exercise..." }
            };
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        // INotifyPropertyChanged implementation here...
    }

}
