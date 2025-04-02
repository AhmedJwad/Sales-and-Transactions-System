using Microsoft.AspNetCore.Components;

namespace Sale.Fronrend.Shared
{
    public partial class Loading
    {
        [Parameter] public string Label { get; set; } = "Please wait...";
    }
}
