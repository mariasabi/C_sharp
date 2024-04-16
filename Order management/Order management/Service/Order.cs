using Order_management.Model;

namespace Order_management.Service
{
    public interface IOrder
    {
        Task<List<Item>> GetItems();
        Task<Item> GetItem(int id);
        Task<List<Item>> AddItem(Item request);
        Task<List<Item>> UpdateItem(Item request);
        Task<Item> DeleteItem(int id);


    }
    public class Order:IOrder
    {
        private static List<Item> items = new List<Item>()
            {
                new Item()
                {
                    Id =1,
                    Name="Dove soap",
                    Type="Bath & body",
                    Quantity=10
                },
                new Item()
                {
                    Id=2,
                    Name="Lux soap",
                    Type="Bath & body",
                    Quantity=15
                }
            };
        public async Task<List<Item>> GetItems()
        {
            return items;
        }
        public async Task<Item> GetItem(int id)
        {
            var item = items.Find (x => x.Id == id);
            return item;
        }
        public async Task<List<Item>> AddItem(Item request)
        {
            var existingItem = items.Find(x => x.Id == request.Id);
            if (existingItem != null)
            {
                return null;
            }
            items.Add(request);
            return items;
        }
        public async Task<List<Item>> UpdateItem(Item request)
        {
            var existingItem = items.Find(x => x.Id == request.Id);
            if (existingItem == null)
            {
                return null;
            }
            existingItem.Quantity = request.Quantity;
            existingItem.Name = request.Name;
            existingItem.Type = request.Type;
            return items;
        }
        public async Task<Item> DeleteItem(int id)
        {
            var item = items.Find(x => x.Id == id);
            if (item == null)
            {
             return null;
            }
            items.Remove(item);
            return item;
        }
    }
}
