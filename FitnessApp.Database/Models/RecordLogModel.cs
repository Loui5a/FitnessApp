using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Database.Models
{
    public class RecordLogModel
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int Reps { get; set; }
        public int Sets { get; set; }
        public decimal Weight { get; set; }

        public int ExerciseId { get; set; }
        public ExerciseModel? Exercise { get; set; }
    }
}
