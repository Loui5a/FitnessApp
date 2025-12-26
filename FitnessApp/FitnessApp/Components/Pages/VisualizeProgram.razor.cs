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
        // Declaring variables and objects: 
        bool loading = true;
        bool loadingProgram = false;
        public List<ProgramModel> Programs { get; set; } = new(); // List of available programs
        public ProgramModel? ActiveProgram { get; set; } // Currently selected program
        FluentDataGrid<ExerciseModel> dataGrid = default!; // Data grid for displaying exercises
        private IQueryable<ExerciseModel>? Exercises; // queryable collection of ExerciseModel
        GridItemsProviderRequest<ExerciseModel>? CurrentReq; // Current request for grid items

        [Inject]
        FitnessContext fitnessContext { get; set; } = default!; // Injecting the FitnessContext for database operations


        protected override async Task OnInitializedAsync()
        // OnInitializedAsync method is called when the component is initialized. 
        {
            await InvokeAsync(StateHasChanged);
            Programs = await fitnessContext.ProgramModels.ToListAsync(); // Fetching all program models from the database
            loading = false;
        }

        protected async Task RefreshItemsAsync(GridItemsProviderRequest<ExerciseModel> req)
        // RefreshItemsAsync method is responsible for refreshing the items in the data grid based on the provided request.
        // It handles sorting and pagination (TO DO) of the exercise data.
        {
            loading = true;
            await InvokeAsync(StateHasChanged);

            var query = fitnessContext.ExerciseModels.AsEnumerable(); // Starting query from ExerciseModels
            // TODO: make into a for loop to handle multriple sortBy interations
            var sort = req.GetSortByProperties().FirstOrDefault(); // Getting the first sort property from the request
            if (!string.IsNullOrEmpty(sort.PropertyName)) // Checking if the property name is not null or empty
            {
                switch (sort.PropertyName)
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
                        throw new NotImplementedException($"{sort.PropertyName} not found in the switch-case");
                }
            }
            // TODO: implement Pagenation
            //query = query.Skip(req.StartIndex).Take(req.Count ?? 10);

            Exercises = query.AsQueryable(); // Updating the Exercises property with the modified query

            loading = false;
            await InvokeAsync(StateHasChanged);

            CurrentReq = req;
        }
        private async Task OnProgramChangeAsync(ProgramModel selectedProgram)
        // OnProgramChangeAsync method is called when the selected program changes.
        {
            loadingProgram = true;
            ActiveProgram = selectedProgram;
            if (ActiveProgram is null)
            {
                Exercises = null;
                return;
            }
            // Fetching exercises associated with the selected program
            List<ExerciseModel> exerciseList = await fitnessContext.ProgramModels.Where(p => p.Id == ActiveProgram.Id).SelectMany(p => p.ProgramEntries!.Select(pe => pe.Exercise)).ToListAsync(); 
            Exercises = exerciseList.AsQueryable();
            loadingProgram = false;
        }
    }
}
