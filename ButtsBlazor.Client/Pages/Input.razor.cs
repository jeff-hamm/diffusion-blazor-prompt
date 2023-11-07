using System.Diagnostics.CodeAnalysis;
using ButtsBlazor.Api.Model;
using ButtsBlazor.Client.Components;
using ButtsBlazor.Client.Layout;
using ButtsBlazor.Shared.Services;
using ButtsBlazor.Shared.ViewModels;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Runtime.InteropServices;

namespace ButtsBlazor.Client.Pages;

public partial class Input()
{
    private PersistingComponentStateSubscription? persistingSubscription;
    private Random? random;
    public int SelectedIndex { get; set; }
    private int? Seed { get; set; }

    [Parameter]
    public int Columns { get; set; } = 6;

    [Parameter]
    public int Rows { get; set; } = 4;

    [Parameter]
    public int NumImages { get; set; } = 18;
    public RootPageControls? Page { get; set; } = null!;

    GridSquare[]? images;
    private ImageGrid? grid;
    private void NavigateBack()
    {
        Navigation.NavigateTo("/input");
    }

    protected override async Task OnInitializedAsync()
    {
        History.ClearHistory();
        IsLoading = true;
        StateHasChanged();
        persistingSubscription = ApplicationState.RegisterOnPersisting(PersistData);
        RestorePersistedState();
        grid = new ImageGrid(random, Columns, Rows);

        // Don't block here?
        await Notifications.SubscribeImage(this, OnNewImage);
    }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        await Reload();
    }
    private async Task Reload()
    {
        IsLoading = true;
        StateHasChanged();
        grid?.Clear();
        var entities = await ApiClient.GetRecentImages(NumImages);
        images = entities?.Length > 0 ? grid?.Place(entities) : Array.Empty<GridSquare>();
        IsLoading = false;
        StateHasChanged();
    }

    public bool IsLoading { get; set; } = true;

    private Task OnNewImage(ImageEntity imageEntity)
    {
        if(SelectedIndex == 0)
            Navigation.Refresh();
        return Task.CompletedTask;
    }



    [MemberNotNull(nameof(random))]
    private void RestorePersistedState()
    {
        if (!ApplicationState.TryTakeFromJson<int>(
                "seed", out var seed))
            seed = Random.Shared.Next();
        Seed = seed;
        random = new Random(seed);
    }

    private async Task OnClick(MouseEventArgs obj, GridSquare clicked)
    {

        if (images?.Length > 0)
        {
            for (int i = 0; i < images.Length; i++)
            {
                if (images[i].Entity == clicked.Entity)
                {
                    if(SelectedIndex == i)
                        await Select(i);
                    else
                        SetSelectedIndex(i);
                    return;
                }
            }
        }
    }
    private Task PersistData()
    {
        if (Seed.HasValue)
            ApplicationState.PersistAsJson("seed", Seed.Value);
        ApplicationState.PersistAsJson("selectedIndex", SelectedIndex);
        return Task.CompletedTask;
    }

    private void SetSelectedIndex(int newIndex)
    {
        var previousIndex = SelectedIndex;
        SelectedIndex = newIndex;
        SelectedIndex %= (images?.Length ?? 0);
        if (SelectedIndex < 0)
            SelectedIndex = (images?.Length ?? 0) + SelectedIndex;
        if (SelectedIndex != previousIndex)
        {
            Console.WriteLine("Input index changed from {0} to {1}", previousIndex, SelectedIndex);
            StateHasChanged();
        }
    }

    //private Task TimeoutReload()
    //{
    //    Console.WriteLine("Timeout expired", DateTime.Now);
    //    Navigation.Refresh(true);
    //    return Task.CompletedTask;
    //}


    private Task Select(int selectedIndex)
    {
//        await DisposeAsync();

        Navigation.NavigateTo($"/prompt?image={images?[selectedIndex].Entity?.Path}&code={Seed.ToBase64String()[..^2]}");
        return Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        if (persistingSubscription is { } sub)
            sub.Dispose();

        Notifications.Dispose();
    }

}
