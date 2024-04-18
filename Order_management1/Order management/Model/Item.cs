namespace Order_management.Model
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; } =string.Empty;
        public string Type { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}
