using FitnessApp.Database;
using FitnessApp.Database.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.FluentUI.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace FitnessApp.Components.Pages
{
    public partial class ProgramEntries : ComponentBase
    {
        FluentDataGrid<ExerciseModel> dataGridProgramEntry = default!;
        bool loadingProgram = true;
        PaginationState paginationProgramEntry = new PaginationState { ItemsPerPage = 10 };
        string? _TypeFilterProgram = "";
        string? _CategoryFilterProgram = "";
        string? _ExerciseFilterProgram = "";

        [SupplyParameterFromForm]

        private IQueryable<ExerciseModel>? ExercisesProgramEntry { get; set; }
        private ExercisePlaceholder ModelProgramEntry { get; set; } = new();
        private IQueryable<ProgramEntryModel>? ProgramEntry { get; set; }
        [Inject]
        FitnessContext contextProgramEntry { get; set; } = default!;  
        GridItemsProviderRequest<ExerciseModel>? CurrentReqProgramEntry;

        protected async Task RefreshItemsAsyncProgramEntry(GridItemsProviderRequest<ExerciseModel> req)
        {
            loadingProgram = true;
            await InvokeAsync(StateHasChanged);

            var query = contextProgramEntry.ExerciseModels.Skip(req.StartIndex).Take(req.Count ?? 10);


            if (!string.IsNullOrWhiteSpace(_TypeFilterProgram))
            {
                query = query.Where(e => e.Type.Contains(_TypeFilterProgram));
            }
            else if (!string.IsNullOrWhiteSpace(_CategoryFilterProgram))
            {
                query = query.Where(e => e.Category.Contains(_CategoryFilterProgram));
            }
            else if (!string.IsNullOrWhiteSpace(_ExerciseFilterProgram))
            {
                query = query.Where(e => e.Exercise.Contains(_ExerciseFilterProgram));
            }
            var sort = req.GetSortByProperties().FirstOrDefault();
            if (req.SortByColumn != null && !string.IsNullOrEmpty(sort.PropertyName))
            {
                query = req.GetSortByProperties().FirstOrDefault().Direction != SortDirection.Descending
                    ? query.OrderBy(e => req.SortByColumn)
                    : query.OrderByDescending(e => req.SortByColumn);
            }

            ExercisesProgramEntry = query.AsQueryable();
            await paginationProgramEntry.SetTotalItemCountAsync(ExercisesProgramEntry.Count());

            loadingProgram = false;
            await InvokeAsync(StateHasChanged);

            CurrentReqProgramEntry = req;
        }

        //protected async Task RefreshItemsAsyncProgramEntry(GridItemsProviderRequest<ExerciseModel> req)
        //{
        //    await InvokeAsync(StateHasChanged);

        //}

        public void ClearFilters()
        {
            _TypeFilterProgram = null;
            _CategoryFilterProgram = null;
            _ExerciseFilterProgram = null;
        }

        public async Task DataGridRefreshDataAsyncProgramEntry()
        {
            await dataGridProgramEntry.RefreshDataAsync(true);
        }
        public async Task Delete(int exerciseId)
        {
            var exercise = await contextProgramEntry.ExerciseModels.FirstAsync(e => e.Id == exerciseId);
            contextProgramEntry.ExerciseModels.Remove(exercise);
            await contextProgramEntry.SaveChangesAsync();
            if (CurrentReqProgramEntry is null)
            {
                await OnInitializedAsync();
            }
            else
            {
                await RefreshItemsAsyncProgramEntry(CurrentReqProgramEntry.Value);
            }
        }
        
        protected override async Task OnInitializedAsync()
        {
            ExercisesProgramEntry = contextProgramEntry.ExerciseModels.AsQueryable();
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
