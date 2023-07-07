using System.Collections.Concurrent;
using Kube.Demo;
using Microsoft.AspNetCore.Mvc;

var memory = new ConcurrentBag<byte[]>();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<State>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGet("api/startup", () => Results.Ok());

app.MapGet("api/readiness", (State state) =>
{
    lock (state.Lock)
    {
        return state.Ready switch
        {
            true => Results.Ok(true),
            false => Results.BadRequest(false)
        };
    }
});

app.MapGet("api/liveness", (State state) =>
{
    lock (state.Lock)
    {
        return state.Alive switch
        {
            true => Results.Ok(true),
            false => Results.BadRequest(false)
        };
    }
});

app.MapPost("api/pause", ([FromQuery] string probe, [FromQuery] int sec, State state) =>
{
    if (sec < 1)
        return Results.BadRequest();
    lock (state.Lock)
    {
        switch (probe)
        {
            case "readiness":
                state.Ready = false;
                _ = Task.Run(async () =>
                {
                    await Task.Delay(sec * 1000);
                    lock (state.Lock) { state.Ready = true; }
                });
                break;
            case "liveness":
                state.Alive = false;
                _ = Task.Run(async () =>
                {
                    await Task.Delay(sec * 1000);
                    lock (state.Lock) { state.Alive = true; }
                });
                break;
            default:
                return Results.BadRequest();
        }
    }
    return Results.Ok(false);
});

app.MapPost("api/alloc", ([FromQuery] int mb) =>
{
    if (mb < 1)
        return Results.BadRequest();
    memory.Add(new byte[mb * 1000000]);
    return Results.Ok();
});

app.MapPost("api/cpu", ([FromQuery] int sec) =>
{
    if (sec < 1)
        return Results.BadRequest();
    _ = Task.Run(() =>
    {
        var stop = DateTime.UtcNow + TimeSpan.FromSeconds(sec);
        while (DateTime.UtcNow < stop)
        {
            var sum = 0;
            for (var i = 0; i < 1_000_000; i++)
            {
                if (DateTime.UtcNow >= stop)
                    break;
                sum += i;
            }
            _ = sum;
        }
    });
    return Results.Ok();
});

var delay = app.Configuration.GetValue<int>("startup_delay");
await Task.Delay(delay * 1000);

app.Run();
