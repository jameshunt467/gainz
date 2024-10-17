using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gainz.Models
{
    public class CompletedSet
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int Weight { get; set; }
        public int Reps { get; set; }
        public int CompletedWorkoutId { get; set; }  // Foreign key to CompletedWorkout
        public int ExerciseId { get; set; }  // Foreign key to Exercise
        [Ignore]  // Ignore to prevent saving Exercise in the DB, it's just for referencing
        public Exercise Exercise { get; set; }  // Reference to the Exercise
    }
}
