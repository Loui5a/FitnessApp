using FitnessApp.Database;
using FitnessApp.Database.Models;
using Microsoft.AspNetCore.Components;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.FluentUI.AspNetCore.Components;

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
        bool loading = true;
        bool loadingProgram = false;
        protected override async Task OnInitializedAsync()
        {
            await InvokeAsync(StateHasChanged);
            Programs = await fitnessContext.ProgramModels.ToListAsync();
            loading = false;
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
