using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace RailwayWebApi.Models
{
    public class Dailyseatavailable
    {
        [Key]
        public int DAILY_SEAT_ID { get; set; }
        public int TRAINID { get; set; }
        public int STATIONID { get; set; }
        public int STATIONORDERID { get; set; }
        public string COACHNUMBER {  get; set; }
        public string COACHTYPE {  get; set; }
        public int SEATNUMBER { get; set; }
        public string BEARTH {  get; set; }
        public string QUOTANAME { get; set; }
        public string STATUS { get; set; }
        public DateTime? RESERVATIONDATE { get; set; }
        public int BOOKING_ID {  get; set; }

    }
}
