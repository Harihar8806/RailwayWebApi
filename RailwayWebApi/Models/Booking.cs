using Microsoft.EntityFrameworkCore;
using System.Configuration;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RailwayWebApi.Models
{
    [Table("BOOKINGS")]
    public class Booking
    {
        [Key]
        public int BOOKID { get; set; }
        public int TRAINID { get; set; }

        public DateTime ? RUNNINGDATE { get; set; }
        public int FROMSTATIONID { get; set; }  
        public int TOSTATIONID { get; set; }
        public string COACHTYPE { get; set; }

        public int SEATBOOKED {  get; set; }
        public string BOOKINGSTATUS { get; set; }
        public DateTime? BOOKING_DATE { get; set; }
    }
}
