using FitnessApp.Components.Dialogs;
using FitnessApp.Database;
using FitnessApp.Database.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.FluentUI.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using static System.Net.WebRequestMethods;


namespace FitnessApp.Components.Pages
{
    public partial class AddEntries : ComponentBase
    {
        #region Variables, Objects and Injections
        // Declaring variables and objects:
        FluentDataGrid<ExerciseModel> dataGrid = default!;
        PaginationState pagination = new PaginationState { ItemsPerPage = 10 };
        private IQueryable<ExerciseModel>? Exercises { get; set; } // queryable collection of ExerciseModel

        bool loading = true;
        //predefined bind-Value in FluentAccordion for filtering according to Type/Category/Exercise:
        string? _TypeFilter = ""; 
        string? _CategoryFilter = "";
        string? _ExerciseFilter = ""; 
        private bool _trapFocus = true; // Enable focus trapping within the dialog
        private bool _modal = true; // Make the dialog modal (disables background interaction)

        [SupplyParameterFromForm]
        private ExercisePlaceholder PlaceholderModel { get; set; } = new(); // used as a placeholder for bind-Value in FluentDialogProvider
        
        [Inject]
        FitnessContext ContextExerciseEntry { get; set; } = default!; // Injecting the FitnessContext for database operations
        GridItemsProviderRequest<ExerciseModel>? CurrentReq;
        
        [Inject]
        public IDialogService DialogService { get; set; } = default!; // Injecting the dialog service for displaying dialogs

        #endregion

        protected override async Task OnInitializedAsync()
        {
            Exercises = ContextExerciseEntry.ExerciseModels.AsQueryable(); 
            await InvokeAsync(StateHasChanged);
        }
        private async Task Submit()
        {
            ContextExerciseEntry.ExerciseModels.Add(new ExerciseModel
            {
                Type = PlaceholderModel.Type!,
                Category = PlaceholderModel.Category!,
                Exercise = PlaceholderModel.Exercise!,
                DefaultDuration = PlaceholderModel.DefaultDuration!,
                Description = PlaceholderModel.Description!,
            });
            await ContextExerciseEntry.SaveChangesAsync();
            PlaceholderModel = new();
            await OnInitializedAsync();
        }

        protected async Task RefreshItemsAsync(GridItemsProviderRequest<ExerciseModel> req)
        /// Each time the data grid for visualizing the exercises in AddEntries needs to refresh its data, this method is called.
        /// functionality: filtering according to Type/Category/Exercise, sorting according to the selected column and direction.

        {
            loading = true;
            await InvokeAsync(StateHasChanged);

            var query = ContextExerciseEntry.ExerciseModels.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(_TypeFilter))
            {
                query = query.Where(e => e.Type.ToLower().Contains(_TypeFilter));
            }
                
            else if(!string.IsNullOrWhiteSpace(_CategoryFilter))
            {
                query = query.Where(e => e.Category.ToLower().Contains(_CategoryFilter));
            }

            else if (!string.IsNullOrWhiteSpace(_ExerciseFilter))
            {
                query = query.Where(e => e.Exercise.ToLower().Contains(_ExerciseFilter));
            }

            var sort = req.GetSortByProperties().FirstOrDefault();
            if (req.SortByColumn != null && !string.IsNullOrEmpty(sort.PropertyName))
            {
                switch (req.SortByColumn.Title)
                {
                    case "Type":
                        query = sort.Direction != SortDirection.Descending
                            ? query.OrderBy(e => e.Type)
                            : query.OrderByDescending(e => e.Type);
                        break;
                    case "Category":
                        query = sort.Direction != SortDirection.Descending
                            ? query.OrderBy(e => e.Category)
                            : query.OrderByDescending(e => e.Category);
                        break;
                    case "Exercise":
                        query = sort.Direction != SortDirection.Descending
                            ? query.OrderBy(e => e.Exercise)
                            : query.OrderByDescending(e => e.Exercise);
                        break;
                    default:
                        throw new NotImplementedException($"{req.SortByColumn.Title} not found in the switch-case");
                }
            }
            //query = query.Skip(req.StartIndex).Take(req.Count ?? 10);

            Exercises = query.AsQueryable();
            await pagination.SetTotalItemCountAsync(await ContextExerciseEntry.ExerciseModels.CountAsync());

            loading = false;
            await InvokeAsync(StateHasChanged);

            CurrentReq = req;
        }

        public async Task ClearFilters()
        /// Clears all the filters applied in the FluentAccordion for filtering according to Type/Category/Exercise.
        {
            _TypeFilter = null;
            _CategoryFilter = null;
            _ExerciseFilter = null;
            CurrentReq = null;
            await OnInitializedAsync();
        }

        public async Task DataGridRefreshDataAsync()
        {
            await dataGrid.RefreshDataAsync(true); // Refresh the data grid asynchronously 
        }



        public async Task EditExerciseAsync(int exerciseId)
        // Method to edit an existing exercise using a dialog (EditExerciseDialog.razor)
        {
            // Fetch the exercise to be edited from the database
            var exercise = await ContextExerciseEntry.ExerciseModels.FirstAsync(e => e.Id == exerciseId); 

            DialogParameters parameters = new()
            {
                Title = "Edit Program",
                PrimaryAction = "Edit",
                SecondaryAction = "Cancel",
                Width = "600px",
                TrapFocus = _trapFocus, // Enabling focus trapping within the dialog
                Modal = _modal, // Making the dialog modal (disables background interaction)
                PreventScroll = true // Prevent background scrolling when the dialog is open 
            }; // Configuring dialog parameters
            IDialogReference dialog = await DialogService.ShowDialogAsync<EditExerciseDialog>(exercise, parameters); // Showing the dialog with the exercise data
            DialogResult? result = await dialog.Result;
            if (result.Cancelled) return;
            if (result.Data is not null)
            {
                ExerciseModel? editedExercise = result.Data as ExerciseModel; // Cast result.Data to ExerciseModel
                ContextExerciseEntry.ExerciseModels.Update(editedExercise!); // Update the exercise in the database context
                await ContextExerciseEntry.SaveChangesAsync();
                await OnInitializedAsync();
            }
            else
            {
                // Handle the case where result.Data is null
            }
        }

        public async Task Delete(int exerciseId)
        {
            // Fetch the exercise to be deleted from the database
            var exercise = await ContextExerciseEntry.ExerciseModels.FirstAsync(e => e.Id == exerciseId); 
            ContextExerciseEntry.ExerciseModels.Remove(exercise);  
            await ContextExerciseEntry.SaveChangesAsync();
            if (CurrentReq is null)
            {
                await OnInitializedAsync();
            }
            else
            {
                await RefreshItemsAsync(CurrentReq.Value);
            }
        }



        private class ExercisePlaceholder
        {
            public int Id { get; set; }

            [Required]
            public string? Type { get; set; }

            [Required]
            public string? Category { get; set; }

            [Required]
            public string? Exercise { get; set; }

            [Required]
            public string? DefaultDuration { get; set; }

            [Required]
            public string? Description { get; set; }
        }

    }

}

