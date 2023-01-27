using WebApplication1.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//-------------------------------------------
// Get the configuration 
IConfigurationRoot? configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .Build();

builder.Services.AddTransient<ICustomerRepository>((svc) =>
{
    string sqlConnectionString = configuration.GetConnectionString("CustomersDBConnection");
    return new CustomerRepository(sqlConnectionString);
});

builder.Services.AddTransient<IEmployeeRepository>((svc) =>
{
    string sqlConnectionString = configuration.GetConnectionString("EmployeesDBConnection");
    return new EmployeeRepository(sqlConnectionString);
});
//-------------------------------------------

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
