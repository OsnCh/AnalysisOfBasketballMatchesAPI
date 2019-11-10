using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public class Startup
    {
        public static void Configure(string connectionString, IServiceCollection services)
        { 
            Infrastructure.Startup.Configure(connectionString, services);
        }
    }
}
