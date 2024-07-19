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
using Microsoft.AspNetCore.Identity;
using Order_management.Models;
using System.Security.Claims;
using Azure.Core;
using Order_management.Logging;
namespace Order_management.Service
{

    public class Order : IOrder
    {
        private readonly OrderManagementContext _context;
        private static readonly ILog log = LogManager.GetLogger(typeof(Order));
       // private static List<Item> items = new List<Item>();
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        DBLogService _dblog;
       
        public Order(OrderManagementContext context, IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _httpContextAccessor = httpContextAccessor;
            //_dblog = new DBLogService(_context,_httpContextAccessor);
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
                _dblog.CallStoredProcedure(DateTime.Now, Thread.CurrentThread.ManagedThreadId.ToString(), "DEBUG",nameof(Order), "No items found to retrieve.");
                log.Debug("No items found to retrieve.");
            }
           // _dblog.CallStoredProcedure(DateTime.Now, Thread.CurrentThread.ManagedThreadId.ToString(), "INFO", nameof(Order), "Retrieved all items.");
            log.Info("Retrieved all items.");
            return items;
        }
        /// <summary>
        /// Retrieves the items in a particular page
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns>Task<List<Item>></returns>
        /// <exception cref="ArgumentsException"></exception>
        public async Task<List<Item>> GetPaginatedItems(int page, int pageSize)
        {
            var items = await _context.Items.ToListAsync();
            var itemsCount = items.Count;
            var totalPages = (int)Math.Ceiling((decimal)itemsCount / pageSize);
            if (page > totalPages)
            {
                _dblog.CallStoredProcedure(DateTime.Now, Thread.CurrentThread.ManagedThreadId.ToString(), "DEBUG", nameof(Order), $"Page number requested is more than total pages which is {totalPages}.");
                log.Debug($"Page number requested is more than total pages which is {totalPages}.");
                throw new ArgumentsException($"Only {totalPages} pages exist! Please give valid page number.");
            }
            _dblog.CallStoredProcedure(DateTime.Now, Thread.CurrentThread.ManagedThreadId.ToString(), "INFO", nameof(Order), "Retrieved all paginated items.");
            log.Info("Retrieved all paginated items.");
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
                _dblog.CallStoredProcedure(DateTime.Now, Thread.CurrentThread.ManagedThreadId.ToString(), "DEBUG", nameof(Order), "Item cannot be retrieved.");
                log.Debug("Item cannot be retrieved.");
                throw new IdNotFoundException("No such item exists");
                
            }
            _dblog.CallStoredProcedure(DateTime.Now, Thread.CurrentThread.ManagedThreadId.ToString(), "INFO", nameof(Order), $"Retrieved item with ID {id}.");
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
            _dblog.CallStoredProcedure(DateTime.Now, Thread.CurrentThread.ManagedThreadId.ToString(), "INFO", nameof(Order), $"Item with ID {request.Id} added.");
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
            _dblog.CallStoredProcedure(DateTime.Now, Thread.CurrentThread.ManagedThreadId.ToString(), "INFO", nameof(Order), $"Item with ID {request.Id} updated.");
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
            _dblog.CallStoredProcedure(DateTime.Now, Thread.CurrentThread.ManagedThreadId.ToString(), "INFO", nameof(Order), $"Item with ID {id} deleted.");
            return item;
        }
    }
}
