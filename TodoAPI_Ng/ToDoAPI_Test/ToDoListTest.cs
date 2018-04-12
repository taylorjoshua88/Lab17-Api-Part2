using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TodoAPI_Ng.Controllers;
using TodoAPI_Ng.Data;
using TodoAPI_Ng.Models;
using Xunit;

namespace ToDoAPI_Test
{
    public class ToDoListTest
    {
        [Fact]
        public async void CanPostList()
        {
            DbContextOptions<ToDoNgDbContext> options = new DbContextOptionsBuilder<ToDoNgDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using (ToDoNgDbContext context = new ToDoNgDbContext(options))
            {
                // Arrange
                ToDoListController controller = new ToDoListController(context);
                int initialCount = await context.ToDoList.CountAsync();

                // Act
                await controller.Post(new ToDoList()
                {
                    Name = "Hello, world!"
                });

                // Assert
                Assert.True((await context.ToDoList.CountAsync()) > initialCount);
            }
        }

        [Fact]
        public async void CanPutList()
        {
            DbContextOptions<ToDoNgDbContext> options = new DbContextOptionsBuilder<ToDoNgDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using (ToDoNgDbContext context = new ToDoNgDbContext(options))
            {
                // Arrange
                ToDoListController controller = new ToDoListController(context);

                CreatedAtActionResult result = await controller.Post(new ToDoList
                {
                    Name = "Hello, world!"
                }) as CreatedAtActionResult;

                ToDoList newList = result.Value as ToDoList;
                int newListId = newList.Id;
                string newListOriginalName = string.Copy(newList.Name);

                // Act
                await controller.Put(newListId, new ToDoList()
                {
                    Id = newListId,
                    Name = "Goodbye!"
                });

                // Assert
                Assert.NotEqual(newListOriginalName,
                    (await context.ToDoList.FirstOrDefaultAsync(l => l.Id == newListId)).Name);
            }
        }

        [Fact]
        public async void CanGetLists()
        {
            DbContextOptions<ToDoNgDbContext> options = new DbContextOptionsBuilder<ToDoNgDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using (ToDoNgDbContext context = new ToDoNgDbContext(options))
            {
                // Arrange
                await context.ToDoList.AddRangeAsync(
                    new ToDoList()
                    {
                        Name = "Hello",
                    },

                    new ToDoList()
                    {
                        Name = "Goodbye",
                    }
                );

                await context.SaveChangesAsync();

                ToDoListController controller = new ToDoListController(context);

                // Act
                OkObjectResult result = controller.GetAll() as OkObjectResult;
                DbSet<ToDoList> lists = result.Value as DbSet<ToDoList>;

                // Assert
                Assert.Equal(2, await lists.CountAsync());
            }
        }

        [Fact]
        public async void CanGetListWithItems()
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
                OkObjectResult getResult = await listController.GetToDoList(listId) as OkObjectResult;
                PropertyInfo itemsPropertyInfo = getResult.Value.GetType().GetProperty("items");
                IEnumerable<ToDo> associatedItems = itemsPropertyInfo.GetValue(getResult.Value) as IEnumerable<ToDo>;

                // Assert
                Assert.Contains(associatedItems, i => i.Id == itemId);
            }
        }

        [Fact]
        public async void CanDeleteList()
        {
            DbContextOptions<ToDoNgDbContext> options = new DbContextOptionsBuilder<ToDoNgDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using (ToDoNgDbContext context = new ToDoNgDbContext(options))
            {
                // Arrange
                ToDoListController controller = new ToDoListController(context);

                CreatedAtActionResult postResult = await controller.Post(new ToDoList()
                {
                    Name = "Hello, world!"
                }) as CreatedAtActionResult;

                int afterPostCount = await context.ToDoList.CountAsync();

                // Act
                await controller.Delete((postResult.Value as ToDoList).Id);

                // Assert
                Assert.True(afterPostCount > (await context.ToDoList.CountAsync()));
            }
        }
    }
}
