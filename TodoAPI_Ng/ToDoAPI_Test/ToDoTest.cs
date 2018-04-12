using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using TodoAPI_Ng.Controllers;
using TodoAPI_Ng.Data;
using TodoAPI_Ng.Models;
using Xunit;

namespace ToDoAPI_Test
{
    public class ToDoTest
    {
        [Fact]
        public async void CanPostItem()
        {
            DbContextOptions<ToDoNgDbContext> options = new DbContextOptionsBuilder<ToDoNgDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using (ToDoNgDbContext context = new ToDoNgDbContext(options))
            {
                // Arrange
                ToDoController controller = new ToDoController(context);
                int initialCount = await context.ToDo.CountAsync();

                // Act
                await controller.Post(new ToDo()
                {
                    Message = "Test Item",
                    IsDone = false,
                    ListId = null
                });

                // Assert
                Assert.True((await context.ToDo.CountAsync()) > initialCount);
            }
        }

        [Fact]
        public async void CanPutItem()
        {
            DbContextOptions<ToDoNgDbContext> options = new DbContextOptionsBuilder<ToDoNgDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using (ToDoNgDbContext context = new ToDoNgDbContext(options))
            {
                // Arrange
                ToDoController controller = new ToDoController(context);

                CreatedAtActionResult result = await controller.Post(new ToDo
                {
                    Message = "Hello, world",
                    IsDone = false,
                    ListId = null
                }) as CreatedAtActionResult;

                ToDo newItem = result.Value as ToDo;
                int newItemId = newItem.Id;
                string newItemOriginalMessage = string.Copy(newItem.Message);

                // Act
                await controller.Put(newItemId, new ToDo()
                {
                    Id = newItemId,
                    Message = "Goodbye!",
                    IsDone = true,
                    ListId = null
                });

                // Assert
                Assert.NotEqual(newItemOriginalMessage,
                    (await context.ToDo.FirstOrDefaultAsync(i => i.Id == newItemId)).Message);
            }
        }

        [Fact]
        public async void CanGetItems()
        {
            DbContextOptions<ToDoNgDbContext> options = new DbContextOptionsBuilder<ToDoNgDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using (ToDoNgDbContext context = new ToDoNgDbContext(options))
            {
                // Arrange
                await context.ToDo.AddRangeAsync(
                    new ToDo()
                    {
                        Message = "Hello",
                        IsDone = true
                    },

                    new ToDo()
                    {
                        Message = "Goodbye",
                        IsDone = false
                    }
                );

                await context.SaveChangesAsync();

                ToDoController controller = new ToDoController(context);

                // Act
                OkObjectResult result = controller.GetAll() as OkObjectResult;
                DbSet<ToDo> items = result.Value as DbSet<ToDo>;

                // Assert
                Assert.Equal(2, await items.CountAsync());
            }
        }

        [Fact]
        public async void CanGetItemWithList()
        {
            DbContextOptions<ToDoNgDbContext> options = new DbContextOptionsBuilder<ToDoNgDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using (ToDoNgDbContext context = new ToDoNgDbContext(options))
            {
                // Arrange
                ToDoController itemController = new ToDoController(context);
                ToDoListController listController = new ToDoListController(context);

                CreatedAtActionResult listResult = await listController.Post(new ToDoList()
                {
                    Name = "Foo"
                }) as CreatedAtActionResult;

                int listId = (listResult.Value as ToDoList).Id;

                CreatedAtActionResult itemResult = await itemController.Post(new ToDo()
                {
                    Message = "Hello, world!",
                    IsDone = false,
                    ListId = listId
                }) as CreatedAtActionResult;

                int itemId = (itemResult.Value as ToDo).Id;

                // Act
                OkObjectResult getResult = await itemController.GetToDo(itemId) as OkObjectResult;
                ToDoList associatedList = (getResult.Value as ToDo).List;

                // Assert
                Assert.Equal(listId, associatedList.Id);
            }
        }

        [Fact]
        public async void CanDeleteItem()
        {
            DbContextOptions<ToDoNgDbContext> options = new DbContextOptionsBuilder<ToDoNgDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using (ToDoNgDbContext context = new ToDoNgDbContext(options))
            {
                // Arrange
                ToDoController controller = new ToDoController(context);

                CreatedAtActionResult postResult = await controller.Post(new ToDo()
                {
                    Message = "Test Item",
                    IsDone = false,
                    ListId = null
                }) as CreatedAtActionResult;

                int afterPostCount = await context.ToDo.CountAsync();

                // Act
                await controller.Delete((postResult.Value as ToDo).Id);

                // Assert
                Assert.True(afterPostCount > (await context.ToDo.CountAsync()));
            }
        }
    }
}
