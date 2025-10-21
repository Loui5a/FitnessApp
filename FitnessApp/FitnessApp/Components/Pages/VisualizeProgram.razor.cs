using FitnessApp.Database;
using FitnessApp.Database.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.FluentUI.AspNetCore.Components;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace FitnessApp.Components.Pages
{
    public partial class VisualizeProgram : ComponentBase
    {
        [Inject]
        FitnessContext fitnessContext { get; set; } = default!;
        public List<ProgramModel> Programs { get; set; } = new();
        public ProgramModel? ActiveProgram { get; set; }
        FluentDataGrid<ExerciseModel> dataGrid = default!;
        private IQueryable<ExerciseModel>? Exercises;
        GridItemsProviderRequest<ExerciseModel>? CurrentReq;
        bool loading = true;
        bool loadingProgram = false;
        protected override async Task OnInitializedAsync()
        {
            await InvokeAsync(StateHasChanged);
            Programs = await fitnessContext.ProgramModels.ToListAsync();
            loading = false;
        }
        protected async Task RefreshItemsAsync(GridItemsProviderRequest<ExerciseModel> req)
        {
            loading = true;
            await InvokeAsync(StateHasChanged);

            var query = fitnessContext.ExerciseModels.AsEnumerable();

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
           
            loading = false;
            await InvokeAsync(StateHasChanged);

            CurrentReq = req;
        }
        private async Task OnProgramChangeAsync(ProgramModel selectedProgram)
        {
            loadingProgram = true;
            ActiveProgram = selectedProgram;
            if (ActiveProgram is null)
            {
                Exercises = null;
                return;
            }
            List<ExerciseModel> exerciseList = await fitnessContext.ProgramModels.Where(p => p.Id == ActiveProgram.Id).SelectMany(p => p.ProgramEntries!.Select(pe => pe.Exercise)).ToListAsync();
            Exercises = exerciseList.AsQueryable();
            loadingProgram = false;
        }
    }
}
