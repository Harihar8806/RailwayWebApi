using System.ComponentModel.DataAnnotations.Schema;

namespace RailwayWebApi.Models
{
    [Table("TRAINS")]
    public class Train
    {
        
        public int TRAINID { get; set; }
        public string TRAINNAME { get; set; }
        public int TRAINNUMBER { get; set; }
        public int SOURCESTATIONID { get; set; }
        public  string SOURCESTATIONNAME { get; set; }
        public int DESTNATIONSTATIONID {  get; set; }
        public string DESTNATIONSTATIONNAME { get; set; }
        public int TOTALDISTANCE { get; set; }  
        public string TRAIN_TYPE { get; set; }
    }
}
