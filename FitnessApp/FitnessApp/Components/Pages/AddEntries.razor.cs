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

        [SupplyParameterFromForm]
        private ExercisePlaceholder Model { get; set; } = new();
        private IQueryable<ExerciseModel>? Exercises { get; set; }
        FitnessContext context = new FitnessContext();
        GridItemsProviderRequest<ExerciseModel>? CurrentReq;

        protected async Task RefreshItemsAsync(GridItemsProviderRequest<ExerciseModel> req)
        {
            loading = true;
            await InvokeAsync(StateHasChanged);

            var query = context.ExerciseModels.Skip(req.StartIndex).Take(req.Count ?? 10);


            if (!string.IsNullOrWhiteSpace(_TypeFilter))
                query = query.Where(e => e.Type.Contains(_TypeFilter));

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
        }

        public async Task DataGridRefreshDataAsync()
        {
            await dataGrid.RefreshDataAsync(true);
        }

        private async Task Submit()
        {
            context.ExerciseModels.Add(new ExerciseModel
            {
                Type = Model.Type!,
                Category = Model.Category!,
                Exercise = Model.Exercise!,
                DefaultDuration = Model.DefaultDuration!,
                Description = Model.Description!,
            });
            await context.SaveChangesAsync();
            Model = new();
            await OnInitializedAsync();
        }

        public async Task Delete(int exerciseId)
        {
            var exercise = await context.ExerciseModels.FirstAsync(e => e.Id == exerciseId);
            context.ExerciseModels.Remove(exercise);
            await context.SaveChangesAsync();
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
            Exercises = context.ExerciseModels.AsQueryable();
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

