namespace RailwayWebApi.Models
{
    public class TrainBetweenStation
    {
        public int TRAINID { get; set; }
        public string TRAINNAME { get; set; }

        public int SOURCE_STATION { get; set; }
        public DateTime ? SCHEDULEDEPARTURE { get; set; }
        public int SOURCE_PLATFORM { get; set; }
        public int DESTINATION_STATION { get; set; }
        public DateTime? SCHEDULEARRIVAL {  get; set; } 
        public int DESTINATION_PLATFORM { get;set; }

    }
}
