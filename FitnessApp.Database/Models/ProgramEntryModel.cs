using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Database.Models
{
    public class ProgramEntryModel
    {
        public int Id { get; set; }
        public int Order { get; set; }

        public int ExerciseId { get; set; }
        public ExerciseModel? Exercise { get; set; }

        public int ProgramId { get; set; }
        public ProgramModel? Program { get; set; }
    }
}
