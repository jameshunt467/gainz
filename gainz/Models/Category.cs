using gainz.Services;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gainz.Models
{
    public class Category
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [MaxLength(100), Unique]
        public string Name { get; set; }


        // Ignored by SQLite, used for navigation and convenience
        [Ignore]
        public List<Exercise> Exercises { get; set; } = new List<Exercise>();

        // Lazy loading method
        public void LoadExercises()
        {
            var db = DatabaseService.Connection;
            Exercises = db.Table<Exercise>().Where(e => e.CategoryId == Id).ToList();
        }
    }
}
