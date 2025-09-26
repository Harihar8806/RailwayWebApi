using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RailwayWebApi.Models
{
    public class TrainRouteDTO
    {
        public int TRAINID { get; set; }


        public int STATIONID { get; set; }

        public int STATIONORDER { get; set; }

        public DateTime? SCHEDULEARRIVAL { get; set; }

        public DateTime? SCHEDULEDEPARTURE { get; set; }

        public int PLATFORMNUMBER { get; set; }

        public int DAY { get; set; }

        public int DISTANCE { get; set; }
    }
}

