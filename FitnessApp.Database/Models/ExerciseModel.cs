using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Database.Models
{
    public class ExerciseModel
    {
        public int Id { get; set; }
        public required string Type { get; set; }
        public required string Category { get; set; }
        public required string Exercise { get; set; }
        public required string DefaultDuration { get; set; }
        public required string DefaultReps { get; set; }
        public required string Description { get; set; }
    }
}
