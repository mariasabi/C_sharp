namespace OrderService.DTOs
{
    public class ItemViewDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Type { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }
        public string? Image { get; set; }
        public string Description { get; set; }
    }
}
