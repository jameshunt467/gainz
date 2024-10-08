﻿using gainz.Pages;

namespace gainz
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(BankPage), typeof(BankPage));
            Routing.RegisterRoute(nameof(WorkoutsPage), typeof(WorkoutsPage));
            Routing.RegisterRoute(nameof(HistoryPage), typeof(HistoryPage));

            Routing.RegisterRoute("addexercise", typeof(AddExercisePage));
            Routing.RegisterRoute("exercisedetails", typeof(ExerciseDetailsPage));

            Routing.RegisterRoute("managecategories", typeof(ManageCategoriesPage));

            Routing.RegisterRoute("settingspage", typeof(SettingsPage));

            Routing.RegisterRoute("workoutdetails", typeof(WorkoutDetailsPage));
            Routing.RegisterRoute("createworkout", typeof(CreateWorkoutPage));

            //Routing.RegisterRoute(nameof(ExerciseDetailsPage), typeof(ExerciseDetailsPage));
        }
    }
}
