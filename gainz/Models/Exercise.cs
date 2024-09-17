using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gainz.Services;
using SQLite;

namespace gainz
{
    public class Exercise : INotifyPropertyChanged
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        //[MaxLength(100)]
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
            //// Populate the list with sample data (replace with actual data later)
            //Exercises = new ObservableCollection<Exercise>
            //{
            //    new Exercise { Name = "Bench Press", Category = "Chest", ImageUrl = "benchpress.png", Description = "A chest exercise..." },
            //    new Exercise { Name = "Squat", Category = "Legs", ImageUrl = "squat.png", Description = "A leg exercise..." }
            //};

            // Load exercises from the database
            LoadExercisesFromDatabase();
        }

        private void LoadExercisesFromDatabase()
        {
            var db = DatabaseService.Connection;

            // Read all exercises from the SQLite database
            var exercisesFromDb = db.Table<Exercise>().ToList();

            // Initialize the ObservableCollection with exercises from the database
            Exercises = new ObservableCollection<Exercise>(exercisesFromDb);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        // INotifyPropertyChanged implementation here...
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
