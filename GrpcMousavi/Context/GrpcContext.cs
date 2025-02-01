using System.Collections.Generic;
using GrpcMousavi.Model;
using Microsoft.EntityFrameworkCore;
namespace GrpcMousavi.Context
{
    public class GrpcContext : DbContext
    {       

        public GrpcContext(DbContextOptions<GrpcContext> options) : base(options)
        {

        }
        public DbSet<Person> People { get; set; }

    }
}
