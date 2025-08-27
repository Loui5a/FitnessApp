using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Database.Models
{
    public class ProgramModel
    {
        public int Id { get; set; }
        public required string Name { get; set; }

        public ICollection<ProgramEntryModel>? ProgramEntries { get; }
    }
}
