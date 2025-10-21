using FitnessApp.Database.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

namespace FitnessApp.Components.Dialogs
{
    public partial class EditExerciseDialog : ComponentBase, IDialogContentComponent<ExerciseModel>
    {
        [Parameter]
        public ExerciseModel Content { get; set; } = default!;

        [CascadingParameter]
        public FluentDialog? Dialog { get; set; }

        private void ToggleDialogPrimaryActionButton(bool enable)
        {
            Dialog!.TogglePrimaryActionButton(enable);
        }
    }
}
