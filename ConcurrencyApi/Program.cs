using System.Threading.Channels;
using ConcurrencyApi.ChannelsCode;
using ConcurrencyApi.ChannelsCode.ChannelReaders;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSwaggerGen();
builder.Services.AddRouting();
builder.Services.AddControllers();

// DI for Channels code:
// channels work as Queue, so if the one of readers took a new request, the other ones won't take it
// so, I decided to split requests to 2 channels. Btw, Channels are faster than the other solutions like Concurrent Structures
builder.Services.AddSingleton(
    Channel.CreateBounded<OrderReceiptRequest>(new BoundedChannelOptions(2)
    {
        SingleReader = true, //only OrderReceiptsCreatorJob
        FullMode = BoundedChannelFullMode.Wait, //Wait kind will freeze the endpoint, so keep that in mind
    }));
builder.Services.AddSingleton(
    Channel.CreateUnbounded<SendOrderToRestaurantsRequest>(new UnboundedChannelOptions
    {
        SingleReader = true, //only OrderSenderToRestaurantsJob

        // Continuation is the code that keep going after waiting(like WaitToReadAsync)
        // true - continuations will be done on the producer's thread synchronously
        // false - all continuations should be invoked asynchronously, it means that a producer just throws its consumers continuations to ThreadPoll 
        AllowSynchronousContinuations = false
    }));

builder.Services.AddSingleton<DishRepository>();
builder.Services.AddSingleton<DishOrdersRepository>();
builder.Services.AddSingleton<RestaurantsService>();

builder.Services.AddHostedService<OrderSenderToRestaurantsJob>();
builder.Services.AddHostedService<OrderReceiptsCreatorJob>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.MapControllers();

app.Run();