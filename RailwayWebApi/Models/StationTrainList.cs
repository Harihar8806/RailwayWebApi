namespace RailwayWebApi.Models
{
    public class StationTrainList
    {
        public string TRAINNAME {  get; set; }
        public int TRAINNUMBER { get; set; }

        public int STATIONID { get; set; }

       
        public DateTime? SCHEDULEARRIVAL { get; set; }

        public DateTime? SCHEDULEDEPARTURE { get; set; }

        public int PLATFORMNUMBER { get; set; }

    }
}
