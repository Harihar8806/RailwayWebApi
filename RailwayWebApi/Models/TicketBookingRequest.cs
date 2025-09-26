namespace RailwayWebApi.Models
{
    public class TicketBookingRequest
    {
        public int TrainId { get; set; }
        public int FromStation {  get; set; }
        public int ToStation { get; set; }
        public string BookingDate { get; set; }
        public string CoachType { get; set; }
        public string QuotaName { get; set; }
        public List<PassengerList> PassengerDetail { get; set; }

        public string Massage { get; set; }
    }
}
