

using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CosmosDB.Data;
using CosmosDB.Service;

Console.WriteLine("Demo Using a single Container");
Console.WriteLine("Launching.......");

var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

var CosmosConnectionString = config["CosmosConnectionString"];

var services = new ServiceCollection();

services.AddDbContextFactory<CosmosDBContext>(optionbuilders => 
optionbuilders
.UseCosmos(
    connectionString: CosmosConnectionString,
    databaseName:"TransportDB",
    cosmosOptionsAction: options=>
    {
        options.ConnectionMode(Microsoft.Azure.Cosmos.ConnectionMode.Direct);
        options.MaxRequestsPerTcpConnection(20);
        options.MaxTcpConnectionsPerEndpoint(32);
    }));

services.AddTransient<CosmosService>();
services.AddSingleton<CosmosDB.Service.WriteLine>((text, highlight, isException) =>
{
    if (isException)
    {
        Console.ForegroundColor = ConsoleColor.Red;
    }
    else if (highlight)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
    }

    Console.WriteLine(text);
    Console.ResetColor();
});

services.AddSingleton< CosmosDB.Service.WaitForNext >(actionName =>
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine();
    Console.WriteLine($"Press ENTER to run {actionName}");
    //Console.ReadLine();
    //Console.Clear();
    Console.WriteLine($"{actionName}:");
    Console.ResetColor();
});


var serviceProvider = services.BuildServiceProvider();
var cosmosService = serviceProvider.GetRequiredService<CosmosService>();

cosmosService.RunSample();

Console.WriteLine();
Console.WriteLine("Done. Press ENTER to quit.");
Console.ReadLine();


