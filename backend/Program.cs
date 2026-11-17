using ShippingEstimator.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();


builder.Services.AddSingleton<QuoteService>();
builder.Services.AddSingleton<ShipmentService>();

var app = builder.Build();

app.MapControllers();

app.Run();
