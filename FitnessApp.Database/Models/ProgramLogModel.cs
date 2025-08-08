using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Database.Models
{
    public class ProgramLogModel
    {
        public int Id { get; set; }
        public int ExerciseId { get; set; }
        public int ProgrammeId { get; set; }
        public int Order { get; set; }
    }
}
