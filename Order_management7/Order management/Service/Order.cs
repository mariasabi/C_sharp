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
using CsvReader = CsvHelper.CsvReader;
using SQLitePCL;
using System.Reflection;
using static NuGet.Client.ManagedCodeConventions;
using NetTopologySuite.Index.HPRtree;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Drawing.Printing;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel;

namespace Order_management.Service
{

    public class Order : IOrder
    {
        private readonly OrderManagementContext _context;
        private static readonly ILog log = LogManager.GetLogger(typeof(Order));
       // private static List<Item> items = new List<Item>();
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
        /// 
        
            public async Task<string> BulkAddItem(string fileName)
        {
            //get the file path
            string currentDirectory = _webHostEnvironment.ContentRootPath;
            string filePath = Path.Combine(currentDirectory,"BulkUploadFiles", fileName);
            //Check if file exists in the path
            if (!File.Exists(filePath))
            {
                log.Debug("File doesn't exist, Bulk Upload failed");
                throw new ArgumentsException("File does not exist at the specified path.");
            }

            var items = new List<Item>();
            string emptyCell = "";
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture)))
            {
                csv.Read();
                csv.ReadHeader();
                var headerRow = csv.HeaderRecord;

                string[] expectedHeaders = { nameof(Item.Name), nameof(Item.Type), nameof(Item.Quantity) };
                //Checks if Columns are match
                if (!headerRow.SequenceEqual(expectedHeaders, StringComparer.OrdinalIgnoreCase))
                {
                    log.Debug("CSV header does not match the expected format, Bulk Upload failed");
                    throw new CSVException("CSV header does not match the expected format.");
                }
                int i = 0;
                while (csv.Read())
                {
                    i++;
                    // Retrieve the value of each field
                    string? name = csv.GetField<string>(headerRow[0]);
                    string? type = csv.GetField<string>(headerRow[1]);
                    int? quantity = csv.GetField<int?>(headerRow[2]);

                    // Check if any field is null or empty
                    if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(type) || !quantity.HasValue)
                    {
                        log.Debug($"CSV contains empty or null fields at row {i}");
                        emptyCell = ", but some cells are empty or null";
                        continue;
                       // throw new CSVException("CSV contains empty or null fields.");
                    }

                    // Check if the book already exists in the database
                    var existingBook = _context.Items.FirstOrDefault(b => b.Name == name && b.Type == type);

                    if (existingBook != null)
                    {
                        // Update the existing book's NoOfBook
                        existingBook.Quantity += quantity;
                    }
                    else
                    {
                        //If new book, add it
                        var book = new Item
                        {
                            Name = name,
                            Type = type,
                            Quantity = quantity
                        };
                        items.Add(book);
                    }
                }
            }

            await _context.BulkInsertAsync(items);
            _context.SaveChanges();
            log.Debug("Items added from " + fileName + " and added to DB successfully"+emptyCell);
            return "Items added successfully "+emptyCell;
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

        public async Task<List<Item>> GetPaginatedItems(int page, int pageSize)
        {
            var items = await _context.Items.ToListAsync();
            var itemsCount = items.Count;
            var totalPages = (int)Math.Ceiling((decimal)itemsCount / pageSize);
            if (page > totalPages)
            {
                log.Debug($"Page number requested is more than total pages which is {totalPages}.");
                throw new ArgumentsException($"Only {totalPages} pages exist! Please give valid page number.");
            }
            var itemsPerPage = items.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return itemsPerPage;
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
