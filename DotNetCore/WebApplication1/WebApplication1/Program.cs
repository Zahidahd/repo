using WebApplication1.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//-------------------------------------------
// Get the configuration 
IConfigurationRoot? configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .Build();

string ECommerceDBConnectionString = configuration.GetConnectionString("ECommerceDBConnection");

builder.Services.AddTransient<ICustomerRepository> ((svc) =>
{
    return new CustomerRepository(ECommerceDBConnectionString);
});

builder.Services.AddTransient<IDoctorRepository>((svc) =>
{
    return new DoctorRepository(ECommerceDBConnectionString);
});

builder.Services.AddTransient<IProductRepository>((svc) =>
{
    return new ProductRepository(ECommerceDBConnectionString);
});

builder.Services.AddTransient<IOrderRepository>((svc) =>
{
    return new OrderRepository(ECommerceDBConnectionString);
});

builder.Services.AddTransient<IEmployeeRepository>((svc) =>
{
    string sqlConnectionString = configuration.GetConnectionString("EmployeesDBConnection");
    return new EmployeeRepository(sqlConnectionString);
});

builder.Services.AddTransient<ITeacherRepository>((svc) =>
{
    string sqlConnectionString = configuration.GetConnectionString("TeachersDBConnection");
    return new TeacherRepository(sqlConnectionString);
});

//------------------------------------------

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
