using Microsoft.EntityFrameworkCore;
using SocialMediaMini.DataAccess;
using SocialMediaMini.DataAccess.Infrastructure;
using SocialMediaMini.DataAccess.Repositories;

var builder = WebApplication.CreateBuilder(args);

//Add DbContext
builder.Services.AddDbContext<SocialMediaMiniContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyDbConnectString")));

// Đăng ký DbFactory và UnitOfWork
builder.Services.AddScoped<IDbFactory, DbFactory>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Đăng ký Repository
builder.Services.AddScoped<IAppUserRepository, AppUserRepository>();
builder.Services.AddScoped<IChatRoomRepository, ChatRoomRepository>();
builder.Services.AddScoped<ICommentHistoryRepository, CommentHistoryRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IPostHistoryRepository, PostHistoryRepository>();
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<IUser_ChatRoomRepository, User_ChatRoomRepository>();

//Đăng ký service



// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
