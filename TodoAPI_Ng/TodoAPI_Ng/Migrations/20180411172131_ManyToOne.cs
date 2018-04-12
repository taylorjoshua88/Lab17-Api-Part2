using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace TodoAPI_Ng.Migrations
{
    public partial class ManyToOne : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ToDo_ToDoList_ToDoListId",
                table: "ToDo");

            migrationBuilder.RenameColumn(
                name: "ToDoListId",
                table: "ToDo",
                newName: "ListId");

            migrationBuilder.RenameIndex(
                name: "IX_ToDo_ToDoListId",
                table: "ToDo",
                newName: "IX_ToDo_ListId");

            migrationBuilder.AddForeignKey(
                name: "FK_ToDo_ToDoList_ListId",
                table: "ToDo",
                column: "ListId",
                principalTable: "ToDoList",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ToDo_ToDoList_ListId",
                table: "ToDo");

            migrationBuilder.RenameColumn(
                name: "ListId",
                table: "ToDo",
                newName: "ToDoListId");

            migrationBuilder.RenameIndex(
                name: "IX_ToDo_ListId",
                table: "ToDo",
                newName: "IX_ToDo_ToDoListId");

            migrationBuilder.AddForeignKey(
                name: "FK_ToDo_ToDoList_ToDoListId",
                table: "ToDo",
                column: "ToDoListId",
                principalTable: "ToDoList",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
