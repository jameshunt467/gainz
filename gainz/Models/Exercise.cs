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

        // Reference to Category ID instead of storing name directly
        public int CategoryId { get; set; }

        // New property to hold the category name
        private string _categoryName;
        [Ignore]
        public string CategoryName
        {
            get => _categoryName;
            set
            {
                _categoryName = value;
                OnPropertyChanged(nameof(CategoryName));
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
}
