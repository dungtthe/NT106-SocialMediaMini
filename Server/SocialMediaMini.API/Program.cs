using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SocialMediaMini.DataAccess;
using SocialMediaMini.DataAccess.Infrastructure;
using SocialMediaMini.DataAccess.Repositories;
using SocialMediaMini.Service;
using System.Diagnostics;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//Add DbContext
builder.Services.AddDbContext<SocialMediaMiniContext>(options =>
{
    options.UseLazyLoadingProxies();
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyDbConnectString"));

    //loggin sql to console
    options.EnableSensitiveDataLogging();//kiểu mấy cái tham số á
    options.LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information);
});


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
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IChatRoomService, ChatRoomService>();

//add jwt
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, // Yêu cầu Kiểm tra Issuer
        ValidateAudience = false, // Không cần Kiểm tra Audience
        ValidateLifetime = true, // Yêu cầu Kiểm tra thời hạn của token
        ClockSkew = TimeSpan.Zero, // Loại bỏ thời gian lệch,check thời hạn thêm chính xác
        ValidateIssuerSigningKey = true, // Yêu cầu Kiểm tra Signature
        ValidIssuer = builder.Configuration["Jwt:Issuer"], // Cấu hình Issuer
                                                           // ValidAudience = builder.Configuration["Jwt:Audience"], // Cấu hình Audience
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            {
                //context.Response.Headers.Add("Token-Expired", "true");
            }
            return Task.CompletedTask;
        }
    };
});


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(e => e.Value.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );
            var customResponse = new
            {
                type = "modelState",
                errors
            };

            return new BadRequestObjectResult(customResponse);
        };
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



// Cấu hình CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin() // Cho phép bất kỳ nguồn gốc nào
               .AllowAnyHeader() // Cho phép bất kỳ header nào
               .AllowAnyMethod(); // Cho phép bất kỳ phương thức HTTP nào
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseCors(); // Áp dụng CORS

app.UseAuthentication();
app.UseAuthorization();


//Area
app.MapAreaControllerRoute(
    name: "AdminArea",
    areaName: "Admin",
    pattern: "admin/{controller=Home}/{action=Index}/{id?}"
);
app.MapAreaControllerRoute(
    name: "UserArea",
    areaName: "User",
    pattern: "user/{controller=Home}/{action=Index}/{id?}"
);
// Route mặc định
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
