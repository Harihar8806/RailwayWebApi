using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RailwayWebApi.Models
{
    public class Passenger
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PASSENGERID { get; set; }
        public int BOOKING_ID { get; set; }
        [ForeignKey("BOOKID")]
        public Booking Booking { get; set; }
        public string FULLNAME { get; set; }
        public int AGE { get; set; }
        public String GENDER { get; set; }
        public string PHONE { get; set; }
        public string EMAIL { get; set; }
    }
}
