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

        [SupplyParameterFromForm]
        private ExercisePlaceholder Model { get; set; } = new();
        private IQueryable<ExerciseModel>? Exercises { get; set; }
        [Inject]
        FitnessContext ContextExerciseEntry { get; set; } = default!;
        GridItemsProviderRequest<ExerciseModel>? CurrentReq;

        protected async Task RefreshItemsAsync(GridItemsProviderRequest<ExerciseModel> req)
        {
            loading = true;
            await InvokeAsync(StateHasChanged);

            var query = ContextExerciseEntry.ExerciseModels.Skip(req.StartIndex).Take(req.Count ?? 10);


            if (!string.IsNullOrWhiteSpace(_TypeFilter))
            {
                query = query.Where(e => e.Type.Contains(_TypeFilter));
            }
                
            else if(!string.IsNullOrWhiteSpace(_CategoryFilter))
            {
                query = query.Where(e => e.Category.Contains(_CategoryFilter));
            }
            else if (!string.IsNullOrWhiteSpace(_ExerciseFilter))
            {
                query = query.Where(e => e.Exercise.Contains(_ExerciseFilter));
            }

            var sort = req.GetSortByProperties().FirstOrDefault();
            if (req.SortByColumn != null && !string.IsNullOrEmpty(sort.PropertyName))
            {
                query = req.GetSortByProperties().FirstOrDefault().Direction != SortDirection.Descending
                    ? query .OrderBy(e => req.SortByColumn)
                    : query.OrderByDescending(e => req.SortByColumn);
            }

            Exercises = query.AsQueryable();
            await pagination.SetTotalItemCountAsync(Exercises.Count());

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

