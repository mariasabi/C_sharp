using System.Dynamic;
using System.Text.Json;
using UserService.DTOs;
using UserService.Interfaces;
using Newtonsoft.Json;
using UserService.Exceptions;
using System.Text;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace UserService.Services
{
    public class InventoryService:IInventoryService
    {
        private readonly HttpClient _httpClient;
       

        public InventoryService(HttpClient httpClient) 
        {
            _httpClient = httpClient;
         }
       
        public async Task<string> CheckInventoryItemQuantity(Item item,int quantity)
        {
            var inventoryQuantity = (int)item.Quantity;
            if (inventoryQuantity < quantity)
                throw new OutOfStockException($"Insufficient stock for item '{item.Name}'. Only {inventoryQuantity} available.");

            var res = ReduceInventoryItemQuantity(item, quantity);
            return "Quantity reduced";
        }

        public async Task<Item> GetInventoryItem(string itemName)
        {
            var response = await _httpClient.GetAsync($"https://localhost:7173/api/Order/getItemByName?name={itemName}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine(json);

                try
                {
                    var item = JsonConvert.DeserializeObject<Item>(json);
                    if (item != null)
                        return item;
                    else
                        return null;

                }


                catch (Newtonsoft.Json.JsonException ex)
                {
                    Console.WriteLine($"JSON Deserialization failed: {ex.Message}");
                    return null;
                }
            }
            else
            throw new ArgumentsException($"Item '{itemName}' not found in inventory.");
        }
         public async Task<string> ReduceInventoryItemQuantity(Item item,int reduceBy )
        {           
            var newQuant = item.Quantity - reduceBy;
            var response = await _httpClient.PutAsync($"https://localhost:7173/api/Order/updateItemQuantity?itemname={item.Name}&quantity={newQuant}",null); 
            if (response.IsSuccessStatusCode)
                return "Updated quantity";
            else
                throw new ArgumentsException("Failed to update quantity in inventory");
        
        
        }
    }
}
