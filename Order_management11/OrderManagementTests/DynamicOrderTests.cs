using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Order_management.Exceptions;
using Order_management.Models;
using Order_management.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementTests
{
    public class DynamicOrderTests
    {
        private readonly Mock<IWebHostEnvironment> _webHostEnvironment;

        public DynamicOrderTests()
        {
            _webHostEnvironment = new Mock<IWebHostEnvironment>();
        }
        private async Task<OrderManagementContext> GetDatabaseContext(int count = 3)
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var optionsBuilder = new DbContextOptionsBuilder<OrderManagementContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .UseInternalServiceProvider(serviceProvider);
            var context = new OrderManagementContext(optionsBuilder.Options);
           
            if (await context.Items.CountAsync() <= 0)
            {
                for (int i = 0; i < count; i++)
                {
                    context.Items.Add(sampleItems[i]);
                  
                }
                await context.SaveChangesAsync();
            }

            return context;
        }
        private List<Item> sampleItems = new List<Item>()
        {
            new Item{Id=1,Name="Vicks",Type="medicine",Quantity=10,Price=30},
            new Item{Id=2,Name="Dolo",Type="medicine",Quantity=5,Price=3},
            new Item{Id=3,Name="Realme buds",Type="earphone",Quantity=2,Price=600}
        };
        [Fact]
        public async Task BulkAddItem_FileDoesNotExist_ThrowsArgumentsException()
        {
            var _mockContext = await GetDatabaseContext();
            var _orderRepository = new DynamicOrder(_mockContext, _webHostEnvironment.Object);
           
            string fileName = "nonexistentfile.csv";
            _webHostEnvironment.Setup(env => env.ContentRootPath).Returns(Directory.GetCurrentDirectory());
            await Assert.ThrowsAsync<ArgumentsException>(() => _orderRepository.BulkAddItem(fileName));
           
        }
        [Fact]
        public async Task BulkAddItem_CsvHeaderDoesNotMatch_ThrowsCSVException()
        {
            
            var _mockContext = await GetDatabaseContext();
            var _orderRepository = new DynamicOrder(_mockContext, _webHostEnvironment.Object);
            string fileName = "invalidheader.csv";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "BulkUploadFiles", fileName);
            await File.WriteAllTextAsync(filePath, "Name,Type,Quant\n");

            _webHostEnvironment.Setup(env => env.ContentRootPath).Returns(Directory.GetCurrentDirectory());
            await Assert.ThrowsAsync<CSVException>(() => _orderRepository.BulkAddItem(fileName));
           
        }
        [Fact]
        public async Task BulkAddItem_CsvContainsEmptyOrNullFields_ReturnsMessageWithEmptyCellWarning()
        {
           
            var _mockContext = await GetDatabaseContext();
            var _orderRepository = new DynamicOrder(_mockContext, _webHostEnvironment.Object);
            string fileName = "withnullfields.csv";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "BulkUploadFiles", fileName);
            await File.WriteAllTextAsync(filePath, "Name,Type,Quantity,Price\nDolo,,10,50\nLays,food,10,20\n");
            _webHostEnvironment.Setup(env => env.ContentRootPath).Returns(Directory.GetCurrentDirectory());
                        
            var result = await _orderRepository.BulkAddItem(fileName);

            result.Should().Contain("some cells are empty or null");
        }
        [Fact]
        public async Task BulkAddItem_CsvContainsValidData_AddsItemsSuccessfully()
        {           
            string fileName = "validitems.csv";

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "BulkUploadFiles", fileName);
            _webHostEnvironment.Setup(env => env.ContentRootPath).Returns(Directory.GetCurrentDirectory());
            await File.WriteAllTextAsync(filePath, "Name,Type,Quantity,Price\nLays,food,10,20\nVicks,medicine,20,35\n");
            var _mockContext = await GetDatabaseContext();
            var _orderRepository = new DynamicOrder(_mockContext, _webHostEnvironment.Object);

            var expected = "Items added successfully ";
            var result = await _orderRepository.BulkAddItem(fileName);

            Assert.Equal(expected, result);            
            var totalItems = _mockContext.Items.ToList();
             totalItems.Should().ContainSingle(i => i.Name == "Lays" && i.Type == "food" && i.Quantity == 10 && i.Price==20);
             totalItems.Should().ContainSingle(i => i.Name == "Vicks" && i.Type == "medicine" && i.Quantity == 20 && i.Price==35);
        }
        [Fact]
        public async void UpdateDynamicItem_InvalidId_ThrowsException()
        {
            var newItem = new Item { Id = 4, Name = "Classmate notebook", Type = "stationary", Quantity = 50, Price = 40 };
            var _mockContext = await GetDatabaseContext();
            var _orderRepository = new DynamicOrder(_mockContext, _webHostEnvironment.Object);
            await Assert.ThrowsAsync<IdNotFoundException>(() => _orderRepository.UpdateItem(newItem));

        }
        [Fact]
        public async void UpdateDynamicItem_ValidId_ReturnsUpdatedItem()
        {
            var newItem = new Item { Id = 2, Name = "Classmate notebook", Type = "stationary", Quantity = 50, Price = 40 };
            var _mockContext = await GetDatabaseContext();
            var _orderRepository = new DynamicOrder(_mockContext, _webHostEnvironment.Object);
            var result = await _orderRepository.UpdateItem(newItem);

            Assert.NotNull(result);
            Assert.Equal(newItem, result);
            Assert.IsType<Item>(result);
        }
    }
}
