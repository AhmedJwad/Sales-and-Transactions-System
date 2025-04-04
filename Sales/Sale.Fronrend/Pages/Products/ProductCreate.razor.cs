using Microsoft.AspNetCore.Components;
using MudBlazor;
using Sale.Fronrend.Repositories;
using Sale.Share.DTOs;
using Sale.Share.Entities;

namespace Sale.Fronrend.Pages.Products
{
    public partial class ProductCreate
    {
        private ProductDTO productDTO = new()
        {
            ProductCategoryIds = new List<int>(),
            ProductImages = new List<string>(),
            SerialNumbers=new List<string>(),
        };
        private ProductForm? productForm;
        private List<Category> nonSelectedCategories = new();
        private bool loading = true;
        [Inject] private IRepository repository { get; set; } = null!;
        [Inject] private NavigationManager navigationManager { get; set; } = null!;
        [Inject] private ISnackbar snackbar { get; set; } = null!;
        [CascadingParameter] private IMudDialogInstance mudDialog { get; set; } = null!;

        protected override async Task OnInitializedAsync()
        {
            productDTO.SerialNumbers!.Add("SN-001");
            productDTO.Barcode = "test";
            var httpActionResponse = await repository.GetAsync<List<Category>>("/api/categories/combo");
            loading = false;

            if (httpActionResponse.Error)
            {
                var message = await httpActionResponse.GetErrorMessageAsync();
                snackbar.Add(message, Severity.Error);
                return;
            }

            nonSelectedCategories = httpActionResponse.Response!;
        }
        private async Task CreateAsync()
        {              
            var httpActionResponse = await repository.PostAsync("/api/products/full", productDTO);
            if (httpActionResponse.Error)
            {
                var message = await httpActionResponse.GetErrorMessageAsync();
                snackbar.Add(message, Severity.Error);
                return;
            }

            mudDialog.Close(DialogResult.Ok(true));
            productForm!.FormPostedSuccessfully = true;
            navigationManager.NavigateTo($"/products");
            snackbar.Add("Product created successfully.", Severity.Success);
        }

        private void Return()
        {
            mudDialog.Close(DialogResult.Cancel());
            productForm!.FormPostedSuccessfully = true;
            navigationManager.NavigateTo($"/products");
        }
    }
}
