using CsvHelper;
using CsvHelper.Configuration;
using EFCore.BulkExtensions;
using log4net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Order_management.Exceptions;
using Order_management.Models;
using System.ComponentModel;
using System.Reflection;
using Order_management .Interfaces;
using NetTopologySuite.Index.HPRtree;
using Microsoft.Data.SqlClient;
using System.Data;
using Order_management.Logging;

namespace Order_management.Service
{
    public class DynamicOrder:IDynamicOrder
    {
        private readonly OrderManagementContext _context;
        private static readonly ILog log = LogManager.GetLogger(typeof(Order));
        //private static List<Item> items = new List<Item>();
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        DBLogService _dblog;
        public DynamicOrder(OrderManagementContext context, IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _httpContextAccessor = httpContextAccessor;
            _httpContextAccessor = httpContextAccessor;
          //  _dblog = new DBLogService(_context, _httpContextAccessor);
        }
       
        /// <summary>
        /// Adds Items into the database in bulk
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>Task<string></returns>
        /// <exception cref="ArgumentsException"></exception>
        /// <exception cref="CSVException"></exception>
        public async Task<string> BulkAddItem(string fileName)
        {
            //get the file path
            string currentDirectory = _webHostEnvironment.ContentRootPath;
            string filePath = Path.Combine(currentDirectory, "BulkUploadFiles", fileName);
            //Check if file exists in the path
            if (!File.Exists(filePath))
            {
                _dblog.CallStoredProcedure(DateTime.Now, Thread.CurrentThread.ManagedThreadId.ToString(), "DEBUG", nameof(DynamicOrder), "File doesn't exist, Bulk Upload failed");
                log.Debug("File doesn't exist, Bulk Upload failed");
                throw new ArgumentsException("File does not exist at the specified path.");
            }

            Type type = typeof(Item);
            PropertyInfo[] properties = type.GetProperties();
          
            var items = new List<Item>();
            string emptyCell = "";
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture)))
            {
                csv.Read();
                csv.ReadHeader();
                var headerRow = csv.HeaderRecord;
                
                
                var propertyMap = new Dictionary<string, PropertyInfo>();

                foreach (var header in headerRow)
                {   
                    var property = properties.FirstOrDefault(p => p.Name.Equals(header, StringComparison.OrdinalIgnoreCase));
                    // Dynamically check and map headers to properties if such a property exists
                    if (property != null)
                    {
                        propertyMap[header] = property;
                    }
                    //Exception is thrown if a header does not match property
                    else
                    {
                        _dblog.CallStoredProcedure(DateTime.Now, Thread.CurrentThread.ManagedThreadId.ToString(), "DEBUG", nameof(DynamicOrder), $"CSV header '{header}' does not match any property in the Item class.");
                        log.Debug($"CSV header '{header}' does not match any property in the Item class.");
                        throw new CSVException($"CSV header '{header}' does not match any property in the Item class.");
                    }
                }

                int i = 0;
                bool flag = false;
                
                //Read each row
                while (csv.Read())
                {
                    i++;
                    var item = new Item();
                    foreach (var header in headerRow)
                    {
                        //Get each cell value
                        var value = csv.GetField(header);
                        //If any field is empty, give proper message and read next row
                        if (string.IsNullOrEmpty(value))
                        {
                            _dblog.CallStoredProcedure(DateTime.Now, Thread.CurrentThread.ManagedThreadId.ToString(), "DEBUG", nameof(DynamicOrder), $"CSV contains empty or null fields at row {i}");
                            log.Debug($"CSV contains empty or null fields at row {i}");
                            emptyCell = ", but some cells are empty or null";
                            flag = true;
                            break;
                        }
                        var property = propertyMap[header];
                        object convertedValue;
                        //Check if property is nullable type
                        if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            var nullableConverter = new NullableConverter(property.PropertyType);
                            convertedValue = nullableConverter.ConvertFrom(value);

                        }
                        else
                        {
                            convertedValue = Convert.ChangeType(value, property.PropertyType);
                        }
                        
                        property.SetValue(item, convertedValue);
                    }
                    if (flag)
                    {
                        flag = false;
                        continue;
                    }
                    var existingItem = _context.Items.FirstOrDefault(b => b.Name == item.Name && b.Type == item.Type);
                    if (existingItem != null)
                    {
                        // Update the existing book's properties
                        foreach (PropertyInfo property in existingItem.GetType().GetProperties().Where(p => p.Name != "Name" && p.Name != "Type" && p.Name != "Id"))
                        {
                            var propertyValue = property.GetValue(item);
                            property.SetValue(existingItem, propertyValue);
                        }
                    }
                    else
                    {
                        items.Add(item);
                    }
                }
            }
            //Add items to database
            foreach(var i in items)
            {
                _context.Items.Add(i);

            }
            await _context.SaveChangesAsync();
            _dblog.CallStoredProcedure(DateTime.Now, Thread.CurrentThread.ManagedThreadId.ToString(), "INFO", nameof(DynamicOrder), "Items added from " + fileName + " and added to DB successfully" + emptyCell);
            log.Info("Items added from " + fileName + " and added to DB successfully" + emptyCell);
            return "Items added successfully " + emptyCell;

        }
        /// <summary>
        /// Updates an item identified by its Id
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Task<Item></returns>
        /// <exception cref="IdNotFoundException"></exception>
        public async Task<Item> UpdateItem(Item request)
        {
            var existingItem = _context.Items.FirstOrDefault(x => x.Id == request.Id);
            if (existingItem == null)
            {
              //  _dblog.CallStoredProcedure(DateTime.Now, Thread.CurrentThread.ManagedThreadId.ToString(), "DEBUG", nameof(DynamicOrder), "Item cannot be retrieved to update.");
                log.Debug("Item cannot be retrieved to update.");
                throw new IdNotFoundException("No such item exists to update");
            }
            // Update the existing book's properties
            foreach (PropertyInfo property in existingItem.GetType().GetProperties().Where(p => p.Name != "Id"))
            {
                var propertyValue = property.GetValue(request);
                property.SetValue(existingItem, propertyValue);
            }
//_dblog.CallStoredProcedure(DateTime.Now, Thread.CurrentThread.ManagedThreadId.ToString(), "INFO", nameof(DynamicOrder), $"Item with ID {request.Id} updated.");
            log.Info($"Item with ID {request.Id} updated.");

            _context.SaveChanges();
            return request;
        }
    }
}
