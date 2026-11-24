namespace DriveNow.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public int CarId { get; set; }
        public Car Car { get; set; }
        public string UserEmail { get; set; }
        public int Days { get; set; }
    }
}
