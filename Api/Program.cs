using Application.Commands.Base;
using Application.Dispatcher;
using Application.Service;
using Auth.Providers.User;
using Config.Database;
using Domain.Notification;
using Infra.Context;
using Infra.Context.Seed;
using Infra.Context.UnitOfWork;
using Infra.Queries.Base;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IDatabaseConfig, DatabaseConfig>();

builder.Services
    .AddScoped<NotificationManager>()
    .AddScoped<INotificationReader>(sp => sp.GetRequiredService<NotificationManager>())
    .AddScoped<INotificationWriter>(sp => sp.GetRequiredService<NotificationManager>());

builder.Services.AddScoped<ISeed, MySeed>();
builder.Services.AddDbContext<IMyDbContext, MyDbContext>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(Query<>).Assembly));
builder.Services.AddScoped<IDevOpsService, DevOpsService>();

builder.Services
    .AddMediatR(config => config.RegisterServicesFromAssembly(typeof(Command<>).Assembly))
    .AddScoped<IMessageDispatcher, MessageDispatcher>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserAuthProvider, UserAuthProvider>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAnyOrigin", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();