using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjGrupowy.Shared
{
    public class Event
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string EventName { get; set; }
        public int MaxAmountOfPeople { get; set; }
        public string Place { get; set; }
        public DateTime EventDate { get; set; }
        public double Cost { get; set; }
        public int MinAge { get; set; }
        public EventCategory EventCategory { get; set;}

        // kategoria (enum)

        // public Person albo User host
    }
}
