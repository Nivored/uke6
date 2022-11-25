using System.Data.SqlClient;
using Dapper;
using MortenToDo.Model;

namespace MortenToDo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();
            const string connStr = @"Data Source=(localdb)\local;Initial Catalog=Todo;Integrated Security=True;";

            app.MapGet("/todo", async () =>
            {
               var conn = new SqlConnection(connStr);
               const string sql = "SELECT Id, Text, Done FROM Todo";
               var todoItems = await conn.QueryAsync<TodoItem>(sql);
               return todoItems;
            });
            app.MapPost("/todo", async (TodoItem todoItem) =>
            {
                var conn = new SqlConnection(connStr);
                const string sql = " INSERT Todo(Id, Text) VALUES (@id, @Text)";
                var rowsAffected = await conn.ExecuteAsync(sql, todoItem);
                return rowsAffected;
            });
            app.MapPut("/todo", async (TodoItem todoItem) =>
            {
                todoItem.Done = DateTime.Today;
                var conn = new SqlConnection(connStr);
                const string sql = " UPDATE Todo SET Done = @Done WHERE Id = @Id";
                var rowsAffected = await conn.ExecuteAsync(sql, todoItem);
                return rowsAffected;
            });
            app.MapDelete("/todo/{id}", async(Guid id) =>
            {
                var conn = new SqlConnection(connStr);
                const string sql = "DELETE FROM Todo WHERE Id = @Id";
                var rowsAffected = await conn.ExecuteAsync(sql, new {Id = id});
                return rowsAffected;
            });

            app.UseStaticFiles();
            app.Run();
        }
    }
}