
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Quan_ly_ban_hang.Models;
using Quan_ly_ban_hang.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
	options.LoginPath = "/Home/Login"; // Chỉ định trang đăng nhập mới
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache(); // Đăng kí dịch vụ lưu cache trong bộ nhớ(Session sẽ sử dụng nó)
builder.Services.AddSession(options =>   // Đăng ký dịch vụ Session
{
    options.Cookie.Name = "MinhNguyen"; // Đặt tên cookie cho session
    options.IdleTimeout = TimeSpan.FromMinutes(20); // Thời gian tồn tại của Session
    options.Cookie.HttpOnly = true; // Cookie chỉ có thể được truy cập từ HTTP
    options.Cookie.IsEssential = true;
});
// Configure DbContext
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("qlbanhangConnection"));
});
// Register AccountService
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddScoped<ISessionCartService, SessionCartService>();
builder.Services.AddScoped<ICartService, CartService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=home}/{action=index}/{id?}");

app.Run();

