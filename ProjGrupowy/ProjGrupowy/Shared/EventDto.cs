using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjGrupowy.Shared
{
    public class EventDto
    {
        [Required]
        public string EventName { get; set; }
        [Required]
        public int MaxAmountOfPeople { get; set; }
        [Required]
        public string Place { get; set; }
        [Required]
        public DateTime EventDate { get; set; }
        [Required]
        public double Cost { get; set; }
        [Required]
        public int MinAge { get; set; }
        [Required]
        public string EventCategory { get; set; }
    }
}
