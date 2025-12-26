using FitnessApp.Components.Dialogs;
using FitnessApp.Database;
using FitnessApp.Database.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.FluentUI.AspNetCore.Components;

namespace FitnessApp.Components.Pages
{
    public partial class ProgramEntry : ComponentBase
    {
        #region Variables, Objects and Injections
        // Declaring variables and objects: 
        bool loading = true;
        public string ProgramName { get; set; }
        // Dialog configuration variables
        private bool _trapFocus = true;
        private bool _modal = true;

        public List<ExerciseModel> AllExercises { get; private set; } = new();
        public List<ExerciseModel> ProgramExercises { get; set; } = new();
        public List<ProgramModel> Programs { get; set; } = new();
        public ProgramModel? ActiveProgram { get; set; }
        
        // Injecting the FitnessContext for database operations
        [Inject]
        FitnessContext fitnessContext { get; set; } = default!;

        // Injecting the DialogService for displaying dialogs
        [Inject]
        public IDialogService DialogService { get; set; } = default!;
        #endregion
        protected override async Task OnInitializedAsync()
        // OnInitializedAsync method is called when the component is initialized.
        {
            // Fetching all exercise models from the database ordered by exercise name for SortableList in Razor component
            AllExercises = await fitnessContext.ExerciseModels.OrderBy(a => a.Exercise).ToListAsync(); 
            await InvokeAsync(StateHasChanged);
            loading = false;
            // Fetching all program models from the database for Select in Razor component
            Programs = await fitnessContext.ProgramModels.ToListAsync(); 
        }
        #region Add/Delete Program
        private async Task AddProgram()
        // Method to add a new program using a dialog (AddDialog.razor)
        {
            if (ActiveProgram == null) ActiveProgram = new ProgramModel() { Name = string.Empty };
            DialogParameters parameters = new()
            {
                Title = "Add Program",
                PrimaryAction = "Add",
                SecondaryAction = "Cancel",
                Width = "500px",
                TrapFocus = _trapFocus,
                Modal = _modal,
                PreventScroll = true
            }; // Configuring dialog parameters

            var dialog = await DialogService.ShowDialogAsync<AddDialog>(ActiveProgram, parameters); // Show AddDialog with ActiveProgram as parameter
            var result = await dialog.Result;
            if (result.Cancelled) return;
            if (result.Data is not null)
            {
                ProgramModel? program = result.Data as ProgramModel; // Cast result.Data to ProgramModel
                fitnessContext.ProgramModels.Add(new ProgramModel
                {
                    Name = program!.Name!,
                }); // Add new ProgramModel to the database context
                await fitnessContext.SaveChangesAsync();
                await OnInitializedAsync();
            }
            else
            {
                // Handle the case where result.Data is null
            }

        }
        private async Task DeleteProgramAsync()
        // Method to delete the active program using a dialog (DeleteDialog.razor)
        {
            if (ActiveProgram == null) return;

            DialogParameters parameters = new()
            {
                Title = "Delete Program",
                PrimaryAction = "Delete",
                SecondaryAction = "Cancel",
                Width = "500px",
                TrapFocus = _trapFocus,
                Modal = _modal,
                PreventScroll = true
            }; // Configuring dialog parameters

            IDialogReference dialog = await DialogService.ShowDialogAsync<DeleteDialog>(ActiveProgram, parameters); // Show DeleteDialog with ActiveProgram as parameter
            DialogResult? result = await dialog.Result;
            if (result.Cancelled) return;
            fitnessContext.Remove(ActiveProgram); // Remove the ActiveProgram from the database context
            await fitnessContext.SaveChangesAsync();
            await OnInitializedAsync();
        }
        #endregion

        #region Add/Remove/Reorder Exercises
        private void AddToProgram(FluentSortableListEventArgs args)
        // Method to add an exercise to the program exercises list
        {
            if (args is null)
            {
                return;
            }

            // get the item at OldIndex in AllExercises-list
            var item = AllExercises[args.OldIndex];

            var clone = item;

            // add  the exercise at NewIndex to ProgramExercises-list
            ProgramExercises.Insert(args.NewIndex, clone);
        }

        private void RemoveFromProgram(FluentSortableListEventArgs args)
        // Method to remove an exercise from the programExercises-list
        {
            if (args is null)
            {
                return;
            }
            ProgramExercises.RemoveAt(args.OldIndex);
        }


        private void ReorderProgramExercises(FluentSortableListEventArgs args)
        // Method to reorder exercises in the programExercises-list.
        // The exercise at OldIndex is removed and inserted at NewIndex.
        {
            if (args is null || args.OldIndex == args.NewIndex)
            {
                return;
            }

            var oldIndex = args.OldIndex;
            var newIndex = args.NewIndex;

            var exerciseToMove = ProgramExercises[oldIndex];
            ProgramExercises.RemoveAt(oldIndex);
            // if newIndex is greater than the count of ProgramExercises, add to the end of the list
            if (newIndex < ProgramExercises.Count)
            {
                ProgramExercises.Insert(newIndex, exerciseToMove);
            }
            else
            {
                ProgramExercises.Add(exerciseToMove);
            }
        }
        private async Task Submit()
        // Method to submit the program exercises and save them to the database
        {
            if (ActiveProgram is null)
                return;
            ActiveProgram.ProgramEntries = ProgramExercises.Select((exercise, index) => new ProgramEntryModel
            {
                ExerciseId = exercise.Id,
                Order = index,
                Program = ActiveProgram
            }).ToList(); // Creating a list of ProgramEntryModel objects from the ProgramExercises list
            fitnessContext.Update(ActiveProgram); // Updating the ActiveProgram in the database context
            await fitnessContext.SaveChangesAsync();
        }
        #endregion

    }
}
