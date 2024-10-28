using Microsoft.EntityFrameworkCore;

namespace Quan_ly_ban_hang.Models
{
	public class DataContext : DbContext
	{
		// Constructor nhận đối tượng DbContextOptions và truyền nó cho constructor của lớp cơ sở (DbContext).
		public DataContext(DbContextOptions<DataContext> options) : base(options) { } //DbContextOptions chứa các cấu hình cần thiết để Entity FWC kết nối csdl
		public DbSet<User> Users { get; set; } //Dbset<T> tham số T là 1 loại thực thể
		public DbSet<Role> Roles { get; set; }
		public DbSet<Customer> Customers { get; set; }
		public DbSet<Product> Products { get; set; }
		public DbSet<Category> Categories { get; set; }
		public DbSet<ProductCategory> ProductCategories { get; set; }
		public DbSet<ProductDiscount> ProductDiscounts { get; set; }
		public DbSet<Order> Orders { get; set; }
		public DbSet<OrderDetail> OrderDetails { get; set; }
		public DbSet<ShoppingCart> ShoppingCarts { get; set; }
		public DbSet<CartItem> CartItems { get; set; }
		public DbSet<Review> Reviews { get; set; }
		public DbSet<Payment> Payments { get; set; }
		public DbSet<Discount> Discounts { get; set; }
		public DbSet<Brand> Brands { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<ProductCategory>()
				.HasKey(pc => new { pc.ProductId, pc.CategoryId }); // Cấu hình khóa chính ghép
			modelBuilder.Entity<ProductDiscount>()
	            .HasKey(pd => new { pd.ProductId, pd.DiscountId }); // Cấu hình khóa chính ghép
		}
		/*  protected override void OnModelCreating(ModelBuilder modelBuilder)
		  {
			  base.OnModelCreating(modelBuilder);
			  // Cấu hình mối quan hệ nhiều-nhiều giữa Product và Discount
			  modelBuilder.Entity<ProductImage>()
				   .HasOne(pi => pi.Product)
				   .WithMany(p => p.ProductImages)
				   .HasForeignKey(pi => pi.ProductId);

			  modelBuilder.Entity<Product>()
				  .HasOne(p => p.Brand)
				  .WithMany(b => b.Products)
				  .HasForeignKey(p => p.BrandId);

			  modelBuilder.Entity<ProductDiscount>()
				  .HasKey(pd => new { pd.ProductId, pd.DiscountId });

			  modelBuilder.Entity<ProductDiscount>()
				  .HasOne(pd => pd.Product)
				  .WithMany(p => p.ProductDiscounts)
				  .HasForeignKey(pd => pd.ProductId);

			  modelBuilder.Entity<ProductDiscount>()
				  .HasOne(pd => pd.Discount)
				  .WithMany(d => d.ProductDiscounts)
				  .HasForeignKey(pd => pd.DiscountId);

			  modelBuilder.Entity<ProductCategory>()
			  .HasKey(pc => new { pc.ProductId, pc.CategoryId });

			  modelBuilder.Entity<ProductCategory>()
				  .HasOne(pc => pc.Product)
				  .WithMany(p => p.ProductCategories)
				  .HasForeignKey(pc => pc.ProductId);

			  modelBuilder.Entity<ProductCategory>()
				  .HasOne(pc => pc.Category)
				  .WithMany(c => c.ProductCategories)
				  .HasForeignKey(pc => pc.CategoryId);

			  // One-to-many relationships
			  modelBuilder.Entity<User>()
				  .HasOne(u => u.Role)
				  .WithMany(r => r.Users)
				  .HasForeignKey(u => u.RoleId);

			  modelBuilder.Entity<Customer>()
				  .HasOne(c => c.User)
				  .WithOne(u => u.Customer)
				  .HasForeignKey<Customer>(c => c.UserId);

			  modelBuilder.Entity<Order>()
				  .HasOne(o => o.Customer)
				  .WithMany(c => c.Order)
				  .HasForeignKey(o => o.CustomerId);

			  modelBuilder.Entity<OrderDetail>()
				  .HasOne(od => od.Order)
				  .WithMany(o => o.OrderDetails)
				  .HasForeignKey(od => od.OrderId);

			  modelBuilder.Entity<OrderDetail>()
				  .HasOne(od => od.Product)
				  .WithMany(p => p.OrderDetails)
				  .HasForeignKey(od => od.ProductId);

			  modelBuilder.Entity<Review>()
				  .HasOne(r => r.Product)
				  .WithMany(p => p.Reviews)
				  .HasForeignKey(r => r.ProductId);

			  modelBuilder.Entity<Review>()
				  .HasOne(r => r.Customer)
				  .WithMany(c => c.Reviews)
				  .HasForeignKey(r => r.CustomerId);

			  modelBuilder.Entity<ShoppingCart>()
				  .HasOne(sc => sc.Customer)
				  .WithOne(c => c.ShoppingCart)
				  .HasForeignKey<ShoppingCart>(sc => sc.CustomerId);

			  modelBuilder.Entity<CartItem>()
				  .HasOne(ci => ci.Shopping_Cart)
				  .WithMany(sc => sc.CartItems)
				  .HasForeignKey(ci => ci.CartId);

			  modelBuilder.Entity<CartItem>()
				  .HasOne(ci => ci.Product)
				  .WithMany(p => p.CartItems)
				  .HasForeignKey(ci => ci.ProductId);

			  modelBuilder.Entity<Payment>()
				  .HasOne(p => p.Order)
				  .WithOne(o => o.Payment)
				  .HasForeignKey<Payment>(p => p.OrderId);

			  modelBuilder.Entity<Order>()
				  .Property(o => o.TotalAmount)
				  .HasColumnType("decimal(18, 3)");

			  modelBuilder.Entity<OrderDetail>()
				  .Property(od => od.UnitPrice)
				  .HasColumnType("decimal(18, 3)");

			  modelBuilder.Entity<Payment>()
				  .Property(p => p.Amount)
				  .HasColumnType("decimal(18, 3)");

			  modelBuilder.Entity<Product>()
				  .Property(p => p.Price)
				  .HasColumnType("decimal(18, 3)");

			  *//*
			   * 
			  // Seed data(data ban đầu)
			  var adminRoleId = Guid.NewGuid();
			  var customerRoleId = Guid.NewGuid();
			  var adminUserId = Guid.NewGuid();
			  var customerUserId = Guid.NewGuid();
			  var categoryId = Guid.NewGuid();
			  var productId = Guid.NewGuid();
			  var customerId = Guid.NewGuid();
			  var adminId = Guid.NewGuid();
			  var orderId = Guid.NewGuid();
			  var reviewId = Guid.NewGuid();
			  var paymentId = Guid.NewGuid();
			  var cartId = Guid.NewGuid();
			  var cartItemId = Guid.NewGuid();
						  modelBuilder.Entity<Role>().HasData(
							  new Role { RoleId = adminRoleId, RoleName = "Admin" },
							  new Role { RoleId = customerRoleId, RoleName = "Customer" }
						  );

						  modelBuilder.Entity<User>().HasData(
							  new User
							  {
								  UserId = adminUserId,
								  RoleId = adminRoleId,
								  UserName = "admin1",
								  Password = "adminpassword",
								  EmailAddress = "admin1@gmail.com",
								  Phone = "0925979999",
								  FullName = "Administrator"
							  },
							  new User
							  {
								  UserId = customerUserId,
								  RoleId = customerRoleId,
								  UserName = "customer",
								  Password = "customerpassword",
								  EmailAddress = "customer@gmail.com",
								  Phone = "0987654321",
								  FullName = "Nguyen Anh Duc"
							  }
						  );

						  modelBuilder.Entity<Category>().HasData(
							  new Category
							  {
								  CategoryId = categoryId,
								  CategoryName = "Default Category",
								  Description = "This is a default category"
							  }
						  );

						  modelBuilder.Entity<Product>().HasData(
							  new Product
							  {
								  ProductId = productId,
								  Name = "Default Product",
								  Description = "This is a default product",
								  Price = 100.0m,
								  Stock = 10
							  }
						  );

						  modelBuilder.Entity<Customer>().HasData(
							  new Customer
							  {
								  CustomerId = customerId,
								  UserId = customerUserId,
								  Address = "123 Customer St.",
								  RegistrationDate = DateTime.Now
							  }
						  );

						  modelBuilder.Entity<Order>().HasData(
							  new Order
							  {
								  OrderId = orderId,
								  CustomerId = customerId,
								  OrderDate = DateTime.Now,
								  TotalAmount = 100.0m,
								  Status = "Pending"
							  }
						  );

						  modelBuilder.Entity<OrderDetail>().HasData(
							  new OrderDetail
							  {
								  OrderDetailId = Guid.NewGuid(),
								  OrderId = orderId,
								  ProductId = productId,
								  Quantity = 1,
								  UnitPrice = 100.0m
							  }
						  );

						  modelBuilder.Entity<Review>().HasData(
							  new Review
							  {
								  ReviewId = reviewId,
								  ProductId = productId,
								  CustomerId = customerId,
								  Rating = 5,
								  Comment = "Great product!",
								  ReviewDate = DateTime.Now
							  }
						  );

						  modelBuilder.Entity<Payment>().HasData(
							  new Payment
							  {
								  PaymentId = paymentId,
								  OrderId = orderId,
								  PaymentDate = DateTime.Now,
								  Amount = 100.0m,
								  PaymentMethod = "Credit Card"
							  }
						  );

						  modelBuilder.Entity<ShoppingCart>().HasData(
							  new ShoppingCart
							  {
								  CardId = cartId,
								  CustomerId = customerId,
								  CreatedDate = DateTime.Now
							  }
						  );

						  modelBuilder.Entity<CartItem>().HasData(
							  new CartItem
							  {
								  CartitemId = cartItemId,
								  CartId = cartId,
								  ProductId = productId,
								  Quantity = 1
							  }
						  );

						  modelBuilder.Entity<ProductCategory>().HasData(
							  new ProductCategory
							  {
								  ProductId = productId,
								  CategoryId = categoryId
							  }
						  );
					  *//*
		  }*/
	}
}
