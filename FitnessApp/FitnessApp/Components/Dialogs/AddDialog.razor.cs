using FitnessApp.Database.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using System;

namespace FitnessApp.Components.Dialogs
{
    public partial class AddDialog : ComponentBase, IDialogContentComponent<ProgramModel>
    {
        [Parameter]
        public ProgramModel Content { get; set; } = default!;

        [CascadingParameter]
        public FluentDialog? Dialog { get; set; }

        private void ToggleDialogPrimaryActionButton(bool enable)
        {
            Dialog!.TogglePrimaryActionButton(enable);
        }
    }
}
