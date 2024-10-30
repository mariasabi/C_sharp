using Microsoft.AspNetCore.Mvc;

namespace OrderService.DTOs
{
    public class ItemDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Type { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }
        [FromForm]
        public IFormFile? Image { get; set; }
        public string Description { get; set; }
    }
}
