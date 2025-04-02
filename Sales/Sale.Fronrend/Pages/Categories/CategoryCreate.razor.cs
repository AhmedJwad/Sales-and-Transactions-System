using Microsoft.AspNetCore.Components;
using MudBlazor;
using Sale.Fronrend.Repositories;
using Sale.Fronrend.Shared;
using Sale.Share.Entities;

namespace Sale.Fronrend.Pages.Categories
{
    public partial class CategoryCreate
    {
        private Category category = new();
        private FormWithName<Category>? categoryForm ;

        [Inject] private IRepository repository { get; set; } = null!;
        [Inject] private ISnackbar snackbar { get; set; } = null!;
        [Inject] private NavigationManager navigationManager { get; set; } = null!;

        [CascadingParameter] private IMudDialogInstance mudDialog { get; set; } = null!;

        private async Task CreateAsync()
        {
            var responseHttp = await repository.PostAsync("/api/categories", category);
            if (responseHttp.Error)
            {
                mudDialog.Close(DialogResult.Cancel());
                var message = await responseHttp.GetErrorMessageAsync();
                snackbar.Add(message, Severity.Error);
                return;
            }

            mudDialog.Close(DialogResult.Ok(true));
            categoryForm!.FormPostedSuccessfully = true;
            navigationManager.NavigateTo("/categories");
            snackbar.Add("Category created successfully.", Severity.Success);
        }

        private void Return()
        {
            mudDialog.Close(DialogResult.Cancel());
            categoryForm!.FormPostedSuccessfully = true;
            navigationManager.NavigateTo("/categories");
        }
    }
}
