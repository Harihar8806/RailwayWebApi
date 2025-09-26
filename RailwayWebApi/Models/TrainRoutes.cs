using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace RailwayWebApi.Models
{
    [Table("TRAINROUTES")]
    
    public class TrainRoutes
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ROUTEID { get; set; }
        public int TRAINID { get; set; }


        public int STATIONID { get; set; }

        public int STATIONORDER { get; set; }

        public DateTime? SCHEDULEARRIVAL { get; set; }

        public DateTime? SCHEDULEDEPARTURE { get; set; }

        public int PLATFORMNUMBER { get; set; }

        public int DAY { get; set; }

        public int DISTANCE {  get; set; }
    }
}
