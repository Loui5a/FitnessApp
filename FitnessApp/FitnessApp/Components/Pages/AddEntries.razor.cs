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
        FluentDataGrid<ExerciseModel> dataGrid = default!;
        bool loading = true;
        PaginationState pagination = new PaginationState { ItemsPerPage = 10 };
        string? _TypeFilter = "";
        string? _CategoryFilter = "";
        string? _ExerciseFilter = "";
        private bool _trapFocus = true;
        private bool _modal = true;

        [SupplyParameterFromForm]
        private ExercisePlaceholder Model { get; set; } = new();
        private IQueryable<ExerciseModel>? Exercises { get; set; }
        
        [Inject]
        FitnessContext ContextExerciseEntry { get; set; } = default!;
        GridItemsProviderRequest<ExerciseModel>? CurrentReq;
        
        [Inject]
        public IDialogService DialogService { get; set; } = default!;
        protected async Task RefreshItemsAsync(GridItemsProviderRequest<ExerciseModel> req)
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

        public void ClearFilters()
        {
            _TypeFilter = null;
            _CategoryFilter = null;
            _ExerciseFilter = null;
        }

        public async Task DataGridRefreshDataAsync()
        {
            await dataGrid.RefreshDataAsync(true);
        }

        private async Task Submit()
        {
            ContextExerciseEntry.ExerciseModels.Add(new ExerciseModel
            {
                Type = Model.Type!,
                Category = Model.Category!,
                Exercise = Model.Exercise!,
                DefaultDuration = Model.DefaultDuration!,
                Description = Model.Description!,
            });
            await ContextExerciseEntry.SaveChangesAsync();
            Model = new();
            await OnInitializedAsync();
        }

        public async Task EditExerciseAsync(int exerciseId)
        {
            var exercise = await ContextExerciseEntry.ExerciseModels.FirstAsync(e => e.Id == exerciseId);
           
            DialogParameters parameters = new()
            {
                Title = "Edit Program",
                PrimaryAction = "Edit",
                SecondaryAction = "Cancel",
                Width = "500px",
                TrapFocus = _trapFocus,
                Modal = _modal,
                PreventScroll = true
            };
            IDialogReference dialog = await DialogService.ShowDialogAsync<EditExerciseDialog>(exercise, parameters);
            DialogResult? result = await dialog.Result;
            //if (result.Cancelled) return;
            //fitnessContext.Remove(ActiveProgram);
            //await fitnessContext.SaveChangesAsync();
            //await OnInitializedAsync();
        }
        public async Task Delete(int exerciseId)
        {
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

        protected override async Task OnInitializedAsync()
        {
            Exercises = ContextExerciseEntry.ExerciseModels.AsQueryable();
            await InvokeAsync(StateHasChanged);
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

