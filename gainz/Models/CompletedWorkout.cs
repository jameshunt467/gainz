using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gainz.Models
{
    // CompletedWorkout has a list of CompletedSet entries.
    // Each CompletedSet references an Exercise via ExerciseId
    // and a CompletedWorkout via CompletedWorkoutId.
    //
    // This is two way as the
    // we have a list of CompletedSet in CompletedWorkout and a list of
    // CompletedWorkout in CompletedSet.

    public class CompletedWorkout
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string WorkoutName { get; set; }
        public DateTime WorkoutDate { get; set; }
        public int TotalVolume { get; set; }
        public int TotalSets { get; set; }
        // Navigation property (not saved in SQLite)
        [Ignore]
        public List<CompletedSet> Sets { get; set; }  // List of sets in this workout
    }
}
