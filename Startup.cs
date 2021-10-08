using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace MyFifaApp
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            var client=new MongoClient("mongodb://localhost:27017");
            var db=client.GetDatabase("fifadb");
            var players=db.GetCollection<Player>("players");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/{team}", async context =>
                {
                    var team=context.Request.RouteValues["team"].ToString();
                    var builder=Builders<Player>.Filter;

                    var filter=builder.Where(p=>p.team.Contains(team));
                    var data=(await players.FindAsync(filter)).ToList().Take(100);            
                    await context.Response.WriteAsJsonAsync(data);
                });
            });
        }
    }

    public class Player
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        public int player_id { get; set; }
        public string name { get; set; }
        public string nationality { get; set; }
        public string position { get; set; }
        public int overall { get; set; }
        public int age { get; set; }
        public int hits { get; set; }
        public int potential { get; set; }
        public string team { get; set; }
    }
}
