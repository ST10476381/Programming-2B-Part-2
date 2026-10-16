
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RaceDayAPI.Data;
using System;

namespace RaceDayAPI.Tests
{
    public class RaceDayWebApplicationFactory
        : WebApplicationFactory<global::Program>
    {
        private readonly string _databaseName =
            "RaceDayTests_" + Guid.NewGuid().ToString();

        protected override void ConfigureWebHost(
            IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<RaceDayContext>();
                services.RemoveAll<
                    DbContextOptions<RaceDayContext>>();

                services.AddDbContext<RaceDayContext>(options =>
                    options.UseInMemoryDatabase(_databaseName));
            });
        }
    }
}
