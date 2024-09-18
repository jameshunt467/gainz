using gainz.Models;
using gainz.Services;
using SQLite;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;

namespace gainz.Pages;

public partial class ManageCategoriesPage : ContentPage
{
    public ObservableCollection<Category> Categories { get; set; }

    public ManageCategoriesPage()
	{
		InitializeComponent();
        LoadCategories();
    }

    private void LoadCategories()
    {
        var categoriesFromDb = DatabaseService.GetAllCategories();
        Categories = new ObservableCollection<Category>(categoriesFromDb);
        CategoriesList.ItemsSource = Categories; // Bind to CollectionView
    }

    private void OnAddCategoryClicked(object sender, EventArgs e)
    {
        var newCategoryName = NewCategoryEntry.Text;

        if (!string.IsNullOrWhiteSpace(newCategoryName))
        {
            try
            {
                // Add to the database
                DatabaseService.AddCategory(newCategoryName);

                // Refresh the list
                Categories.Add(new Category { Name = newCategoryName });
                NewCategoryEntry.Text = string.Empty; // Clear the entry
            }
            catch (SQLiteException ex)
            {
                // Display duplicate error message if found
                if(ex.Message.Equals("UNIQUE constraint failed: Category.Name"))
                {
                    DisplayAlert("Error", $"Failed to add category: Already exists", "OK");
                } else
                {
                    DisplayAlert("Error", $"Failed to add category: {ex.Message}", "OK");
                }
            }
        }
        else
        {
            DisplayAlert("Validation Error", "Please enter a valid category name.", "OK");
        }
    }

    private void OnDeleteCategoryClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var categoryToDelete = (Category)button.CommandParameter;

        if (categoryToDelete != null)
        {
            // Remove from database
            DatabaseService.DeleteCategory(categoryToDelete);

            // Refresh the list
            Categories.Remove(categoryToDelete);
        }
    }
}