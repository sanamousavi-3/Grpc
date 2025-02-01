
using GrpcMousavi.Services;
using GrpcMousavi.Context;
using Microsoft.EntityFrameworkCore;
using GrpcMousavi.Protos;
using GrpcMousavi.Repositories;

var builder = WebApplication.CreateBuilder(args);

#region Add db context

builder.Services.AddDbContext<GrpcContext>(options =>
{
   options.UseSqlServer(builder.Configuration.GetConnectionString("GrpcConnectionString"));
});

#endregion

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
var app = builder.Build();

app.MapGrpcService<GrpcPersonService>();

if (app.Environment.IsDevelopment())
{
    app.MapGrpcReflectionService();
}

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
