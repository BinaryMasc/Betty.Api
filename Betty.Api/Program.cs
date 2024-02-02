using Betty.Api.Infrastructure.Data;
using BettyApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();

string mySqlConnectionString = configuration.GetConnectionString("MySQLConnection");

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



builder.Services.AddSingleton<IDbGenericHandler>(provider =>
{
    var connectionString = mySqlConnectionString;

    return new DbGenericHandler(connectionString);
});

/*  test  */


//var dbcontext = new DbGenericHandler(mySqlConnectionString);

//await dbcontext.RunInsert(new User
//{
//    Email = "testFromApi@protonmail.onion",
//    Name = "user",
//    Lastname = "user",
//    UserId = 1,
//    Username = "user",
//    UserStateCode = 1,
//});

//var user = await dbcontext.RunQuery<User>(u => u.Email == "testFromApi@protonmail.onion");
//;

////////

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
