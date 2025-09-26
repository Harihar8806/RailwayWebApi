using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RailwayWebApi.Models
{
    public class Coaches
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int COACHID { get; set; }
        public int TRAINID { get; set; }
        public int COACHPOSITION { get; set; }
        public string COACHNUMBER { get; set; }
        public string COACHTYPE { get; set; }
        public int TOTALSEATES {  get; set; }
    }
}
