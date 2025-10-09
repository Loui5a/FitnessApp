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

        [Inject]
        FitnessContext fitnessContext { get; set; } = default!;

        [Inject]
        public IDialogService DialogService { get; set; } = default!;

        public List<ExerciseModel> AllExercises { get; private set; } = new();
        public List<ExerciseModel> ProgramExercises { get; set; } = new();
        bool loading = true;
        public string ProgramName { get; set; }

        public List<ProgramModel> Programs { get; set; } = new();
        public ProgramModel? ActiveProgram { get; set; } 

        private bool _trapFocus = true;
        private bool _modal = true;

        protected override async Task OnInitializedAsync()
        {
            AllExercises = await fitnessContext.ExerciseModels.ToListAsync();
            await InvokeAsync(StateHasChanged);
            loading = false;
            Programs = await fitnessContext.ProgramModels.ToListAsync();
        }
        private void AddToProgram(FluentSortableListEventArgs args)
        { 
            if (args is null)
            {
                return;
            }

            // get the item at the old index in list 1
            var item = AllExercises[args.OldIndex];

            var clone = item;

            // add it to the new index in list 2
            ProgramExercises.Insert(args.NewIndex, clone);
        }

        private void RemoveFromProgram(FluentSortableListEventArgs args)
        {
            if (args is null)
            {
                return;
            }
            ProgramExercises.RemoveAt(args.OldIndex);
        }


        private void ReorderProgramExercises(FluentSortableListEventArgs args)
        {
            if (args is null || args.OldIndex == args.NewIndex)
            {
                return;
            }

            var oldIndex = args.OldIndex;
            var newIndex = args.NewIndex;

            var exerciseToMove = ProgramExercises[oldIndex];
            ProgramExercises.RemoveAt(oldIndex);

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
        {
            if (ActiveProgram is null)
                return;
            ActiveProgram.ProgramEntries = ProgramExercises.Select((exercise, index) => new ProgramEntryModel
            {
                ExerciseId = exercise.Id,
                Order = index,
                Program = ActiveProgram
            }).ToList();
            fitnessContext.Update(ActiveProgram);
            await fitnessContext.SaveChangesAsync();
        }

        private async Task AddProgram()
        {
            if (ActiveProgram == null) ActiveProgram = new ProgramModel() {Name = string.Empty };
            DialogParameters parameters = new()
            {
                Title = "Add Program", 
                PrimaryAction = "Add",
                SecondaryAction = "Cancel",
                Width = "500px",
                TrapFocus = _trapFocus,
                Modal = _modal,
                PreventScroll = true
            };

            var dialog = await DialogService.ShowDialogAsync<AddDialog>(ActiveProgram, parameters);
            var result = await dialog.Result;
            if (result.Cancelled) return;
            if (result.Data is not null)
            {
                ProgramModel? program = result.Data as ProgramModel;
                fitnessContext.ProgramModels.Add(new ProgramModel
                {
                    Name = program!.Name!,
                });
                await fitnessContext.SaveChangesAsync();
                await OnInitializedAsync();
            }
            else 
            {
                // Handle the case where result.Data is null
            }

        }
        private async Task DeleteProgramAsync()
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
            };

            IDialogReference dialog = await DialogService.ShowDialogAsync<DeleteDialog>(ActiveProgram, parameters);
            DialogResult? result = await dialog.Result;
            if (result.Cancelled) return;
            fitnessContext.Remove(ActiveProgram);
            await fitnessContext.SaveChangesAsync();
            await OnInitializedAsync();
        }
    }
}
