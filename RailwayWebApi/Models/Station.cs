using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using System.ComponentModel.DataAnnotations;

namespace RailwayWebApi.Models
{
    [Table("STATIONS")]
    public class Station
    {
        [Key]
        public int STATIONID { get; set; }
        public string STATIONCODE { get; set; }
        public string STATIONNAME { get; set; }
    }
}
