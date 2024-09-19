using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gainz.Models
{
    public class Workout
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }
        // New Description field
        public string Description { get; set; }

        // Exercises will be associated via a join table.
        // The join table will handle the relationship between Workout and Exercise.

        // This property will be ignored by SQLite (not stored in the database)
        [Ignore]
        public List<Exercise> Exercises { get; set; } = new List<Exercise>();
    }
}
