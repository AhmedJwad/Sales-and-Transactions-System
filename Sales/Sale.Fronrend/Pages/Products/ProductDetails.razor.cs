using Microsoft.AspNetCore.Components;
using MudBlazor;
using Sale.Fronrend.Repositories;
using Sale.Share.Entities;

namespace Sale.Fronrend.Pages.Products
{
    public partial class ProductDetails
    {
        private List<string>? categories;
        private List<string>? SerialNumbers;
        private List<string>? images;
        private bool loading = true;
        private Product? product;
        private bool isAuthenticated;

        [Inject] private NavigationManager navigationManager { get; set; } = null!;
        [Inject] private IRepository repository { get; set; } = null!;
        [Inject] private IDialogService dialogService { get; set; } = null!;
        [Inject] private ISnackbar snackbar { get; set; } = null!;
        [Parameter] public int ProductId { get; set; }
      

       

        protected override async Task OnInitializedAsync()
        {
            await LoadProductAsync();
        }

        private async Task LoadProductAsync()
        {
            loading = true;
            var httpActionResponse = await repository.GetAsync<Product>($"/api/products/{ProductId}");

            if (httpActionResponse.Error)
            {
                loading = false;
                var message = await httpActionResponse.GetErrorMessageAsync();
                snackbar.Add(message, Severity.Error);
                return;
            }

            product = httpActionResponse.Response!;
            categories = product.ProductCategories!.Select(x => x.Category!.Name).ToList();
            images = product.ProductImages!.Select(x => x.Image).ToList();
            SerialNumbers=product.serialNumbers.Select(x=>x.SerialNumberValue).ToList();
            loading = false;
        }
       
    }
}
