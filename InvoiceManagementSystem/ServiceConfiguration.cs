using InvoiceMgmt.BAL.IService;
using InvoiceMgmt.BAL.Service;
using InvoiceMgmt.DAL.GenericRepository;
using InvoiceMgmt.DAL.IRepo;
using InvoiceMgmt.DAL.Repo;

namespace InvoiceMgmt.API
{
    public static class ServiceConfiguration
    {
        public static void RegisterRepos(this IServiceCollection collection, ConfigurationManager configuration)
        {
            // Register the generic repository with DI
            collection.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            // Register other services
            collection.AddScoped<ICustomerService, CustomerService>();
            collection.AddScoped<ICustomerRepo, CustomerRepo>();

            collection.AddScoped<IProductService, ProductService>();
            collection.AddScoped<IProductRepo, ProductRepo>();

            collection.AddScoped<IInvoiceItemService, InvoiceItemService>();
            collection.AddScoped<IInvoiceItemRepo, InvoiceItemRepo>();

            collection.AddScoped<IInvoiceService, InvoiceService>();
            collection.AddScoped<IInvoiceRepo, InvoiceRepo>();

            collection.AddScoped<IInvoiceService, InvoiceService>();
            collection.AddScoped<IInvoiceRepo, InvoiceRepo>();

            collection.AddScoped<IAuditTrailService, AuditTrailService>();
            collection.AddScoped<IAuditTrailRepo, AuditTrailRepo>();

            collection.AddScoped<IUserService, UserService>();
            collection.AddScoped<IUserRepo, UserRepo>();

            collection.AddScoped<IRoleMasterService, RoleMasterService>();
            collection.AddScoped<IRoleMasterRepo, RoleMasterRepo>();

            collection.AddScoped<JwtTokenService>();
        }


        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });
        }

        public static void RegisterLogging(this IServiceCollection collection)
        {
            //Register logging
        }

        public static void RegisterAuth(this IServiceCollection collection)
        {
            //Register authentication services.
        }
    }
}
