using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gainz.JoinTable
{
    public class ExerciseWorkout
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int WorkoutId { get; set; } // Foreign key to Workout table
        public int ExerciseId { get; set; } // Foreign key to Exercise table
    }
}
