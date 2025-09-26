namespace RailwayWebApi.Models
{
    public class CalculateFare
    {
        public string SOURCESTATION { get; set; }
        public string DESTINATIONSTATION { get; set; }
        public string TRAINNAME { get; set; }
        public int TRAINNUMBER { get; set; }
        public string TRAIN_TYPE {  get; set; }
        public string COACH_TYPE { get; set; }  
        public int TOTAL_DISTANCE { get; set; }
        public int FARE { get; set; }
    }
}
