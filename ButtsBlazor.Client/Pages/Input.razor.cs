using System.Diagnostics.CodeAnalysis;
using ButtsBlazor.Api.Model;
using ButtsBlazor.Client.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace ButtsBlazor.Client.Pages;

public partial class Input()
{
    private PersistingComponentStateSubscription? persistingSubscription;
    private Random? random;
    private DotNetObjectReference<Input>? thisJsRef;


    public int SelectedIndex { get; set; }
    private int? Seed { get; set; }

    [Parameter]
    public int Columns { get; set; } = 4;

    [Parameter]
    public int Rows { get; set; } = 4;

    [Parameter]
    public int NumImages { get; set; } = 10;
    GridSquare[]? images;
    private ImageGrid? grid;
    private IJSObjectReference? jsModule;

    protected override async Task OnInitializedAsync()
    {
        persistingSubscription = ApplicationState.RegisterOnPersisting(PersistData);
        RestorePersistedState();
        grid = new ImageGrid(random, Columns, Rows);
        grid.Clear();
        var entities = await ApiClient.GetRecentImages(NumImages);
        if (entities?.Length > 0)
            images = grid.Place(entities);
        else
        {
            images = Array.Empty<GridSquare>();
        }

        await Notifications.SubscribeImage(this, OnNewImage);
    }

    private Task OnNewImage(ImageEntity imageEntity)
    {
        if(SelectedIndex == 0)
            ReloadPage();
        return Task.CompletedTask;
    }

    private void ReloadPage(bool forceLoad=true)
    {
        Navigation.NavigateTo(Navigation.Uri, forceLoad);
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

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            thisJsRef = DotNetObjectReference.Create(this);
            jsModule = await Js.InvokeAsync<IJSObjectReference>("import", "/js/input.js");
            docListenerJsRef = await Js.InvokeAsync<IJSObjectReference>("butts.addDocumentListener", thisJsRef, "keydown", nameof(KeyDown));
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    [JSInvokable]
    public async Task KeyDown(KeyboardEventArgs obj)
    {
        switch (obj.Code)
        {
            case "Tab":
                SetSelectedIndex(SelectedIndex + (obj.ShiftKey ? -1 : 1));
                break;
            case "ArrowRight":
                SetSelectedIndex(SelectedIndex + 1 );
                break;

            case "ArrowLeft":
                SetSelectedIndex(SelectedIndex - 1);
                break;
            case "ArrowUp":
                SetSelectedIndex(SelectedIndex - Columns);
                break;
            case "ArrowDown":
                SetSelectedIndex(SelectedIndex + Columns);
                break;
            case "Enter":
            case "Space":
                await Select(SelectedIndex);
                StateHasChanged();
                return;

        }
    }

    private void SetSelectedIndex(int newIndex)
    {
        var previousIndex = SelectedIndex;
        SelectedIndex = newIndex;
        SelectedIndex %= (images?.Length ?? 0);
        if (SelectedIndex < 0)
            SelectedIndex = (images?.Length ?? 0) + SelectedIndex;
        if (SelectedIndex != previousIndex)
            StateHasChanged();
        ResetActivityTimeout();
    }

    private Timer? activityTimer;
    private IJSObjectReference? docListenerJsRef;

    private void ResetActivityTimeout()
    {
        activityTimer ??= new Timer(_ => ReloadPage(), null, Timeout.Infinite, Timeout.Infinite);
        activityTimer.Change(Options.ActivityTimeout, Timeout.Infinite);
    }

    private async Task Select(int selectedIndex)
    {
        await DisposeAsync();
        Navigation.NavigateTo("/prompt?Selection=" + images?[selectedIndex].Entity?.Path);
    }

    public async ValueTask DisposeAsync()
    {
        if (persistingSubscription is { } sub)
            sub.Dispose();
        if (activityTimer != null)
            await activityTimer.DisposeAsync();
        if (docListenerJsRef != null)
        {
            await docListenerJsRef.InvokeVoidAsync("detach");
            await docListenerJsRef.DisposeAsync();
            docListenerJsRef = null;
        }

        if (jsModule != null)
        {
            await jsModule.DisposeAsync();
            jsModule = null;
        }
        if (thisJsRef != null)
        {
            thisJsRef.Dispose();
            thisJsRef = null;
        }
        Notifications.Dispose();
    }
}
