using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Sale.Fronrend.Shared
{
    public partial class GenericDialog
    {
        [Parameter] public string Message { get; set; } = null!;
        [CascadingParameter] public IMudDialogInstance mudDialog { get; set; } = null!;

        private void Close()
        {
            mudDialog?.Close();
        }
    }
}
