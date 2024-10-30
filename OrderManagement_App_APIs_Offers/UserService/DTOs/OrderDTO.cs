namespace UserService.DTOs
{
    public class OrderDTO
    {
        public int OrderId { get; set; }
       public string Itemname { get; set; }
       public  int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime OrderTime { get; set; }
        public string? Username { get; set; }


}
}
