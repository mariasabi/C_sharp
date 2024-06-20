using log4net;
using log4net.Config;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Order_management.Interfaces;
using Order_management.Models;
using log4net;
using Order_management.Exceptions;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Hosting.Internal;
using Elfie.Serialization;
using EFCore.BulkExtensions;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using CsvHelper.Configuration;
using CsvHelper;

namespace Order_management.Service
{
    
    public class Order:IOrder
    {
        private readonly OrderManagementContext _context;
        private static readonly ILog log = LogManager.GetLogger(typeof(Order));
        private static List<Item> items = new List<Item>();
        private readonly IWebHostEnvironment _webHostEnvironment;
        public Order(OrderManagementContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        /// <summary>
        /// Adds items from a CSV file in bulk.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>Task<string></returns>
        /// <exception cref="ArgumentsException"></exception>
        /// <exception cref="CSVException"></exception>
        public async Task<string> BulkAddItem(string fileName)
        {
            string currentDirectory = _webHostEnvironment.ContentRootPath;
            string csvPath = Path.Combine(currentDirectory, "BulkUploadFiles", fileName);
            //Verify file path exists and is not empty
            if (string.IsNullOrEmpty(csvPath))
            {
                log.Debug("File path may be null or empty");
                throw new ArgumentsException("File path cannot be null or empty.");
            }
            if (!File.Exists(csvPath))
            {
                log.Debug("File does not exist at the specified path.");
                throw new ArgumentsException("File does not exist at the specified path.");
            }
            using (var reader = new StreamReader(csvPath))
            using (var csv = new CsvHelper.CsvReader(reader, new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture)))
            {
                csv.Read();
                csv.ReadHeader();
                var headerRow = csv.HeaderRecord;

                // Check if the CSV header matches the expected headers
                string[] expectedHeaders = { nameof(Item.Name), nameof(Item.Type), nameof(Item.Quantity) };
                if (!headerRow.SequenceEqual(expectedHeaders, StringComparer.OrdinalIgnoreCase))
                {
                    log.Debug("CSV header does not match the expected format, Bulk Upload failed");
                    throw new CSVException("CSV header does not match the expected format.");
                }

                while (csv.Read())
                {
                    string? itemName = csv.GetField<string>(headerRow[0]);
                    string? itemType = csv.GetField<string>(headerRow[1]);
                    int? itemQuantity = csv.GetField<int?>(headerRow[2]);

                    // Check if any field is null or empty
                    if (string.IsNullOrEmpty(itemName) || string.IsNullOrEmpty(itemType) || !itemQuantity.HasValue)
                    {
                        log.Debug("CSV contains empty or null fields, Bulk Upload failed");
                        throw new CSVException("CSV contains empty or null fields.");
                    }
                
                    List<Item> items = new List<Item>();
               
                    var existingBook = _context.Items.FirstOrDefault(item => item.Name == itemName && item.Type == itemType);
                    //If item exists, increment count of quantity of existing item
                    if (existingBook != null)
                    {
                        existingBook.Quantity += itemQuantity;
                        
                    }
                    //Add new item
                    else
                    {
                        Item item = new Item
                        {
                            Name = itemName,
                            Type = itemType,
                            Quantity = itemQuantity
                        };
                        items.Add(item);
                    }
                  
                }
            }
            await _context.BulkInsertAsync(items);
            _context.SaveChanges();
            log.Info("Added all items in bulk from "+fileName+"to database.");
            return "Items added successfully";
        }
        /// <summary>
        /// Retrieve all items
        /// </summary>
        /// <returns>Task<List<Item>></returns>
        public async Task<List<Item>> GetItems()
        {
            var items = await _context.Items.ToListAsync();
            if (items.Count==0)
            {
                log.Debug("No items found to retrieve.");
            }
            log.Info("Retrieved all items.");
            return items;
        }
        /// <summary>
        /// Retrieve item with Id given
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Task<Item></returns>
        /// <exception cref="IdNotFoundException"></exception>
        public async Task<Item> GetItem(int id)
        {
            var item =  _context.Items.FirstOrDefault(x => x.Id == id);
            if (item == null)
            {
                log.Debug("Item cannot be retrieved.");
                throw new IdNotFoundException("No such item exists");
                
            }
            log.Info($"Retrieved item with ID {id}.");
            return item;
        }
        /// <summary>
        /// Add item given in request
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Task<Item></returns>
        public async Task<Item> AddItem(Item request)
        {
            _context.Items.Add(request);
            _context.SaveChanges();
            log.Info($"Item with ID {request.Id} added.");
            return request;
        }
        /// <summary>
        /// Update item identified by Id in request with new values
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Task<Item></returns>
        /// <exception cref="IdNotFoundException"></exception>
        public async Task<Item> UpdateItem(Item request)
        {
            var existingItem = _context.Items.FirstOrDefault(x => x.Id == request.Id);
            if (existingItem == null)
            {
                log.Debug("Item cannot be retrieved to update.");
                throw new IdNotFoundException("No such item exists to update");
            }
            log.Info($"Item with ID {request.Id} updated.");
            existingItem.Quantity = request.Quantity;
            existingItem.Name = request.Name;
            existingItem.Type = request.Type;
            _context.SaveChanges();
            return request;
        }
        /// <summary>
        /// Delete item identified by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Task<Item></returns>
        /// <exception cref="IdNotFoundException"></exception>
        public async Task<Item> DeleteItem(int id)
        {
            var item = _context.Items.FirstOrDefault(x => x.Id == id);
            if (item == null)
            {
                log.Debug("Item cannot be retrieved to delete.");
                throw new IdNotFoundException("No such item exists to delete");
            }
            log.Info($"Item with ID {id} deleted.");
            _context.Items.Remove(item);
            _context.SaveChanges();
            return item;
        }
    }
}
