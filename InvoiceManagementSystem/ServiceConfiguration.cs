using InvoiceMgmt.BAL.IService;
using InvoiceMgmt.BAL.IService.BulkUploads;
using InvoiceMgmt.BAL.Service;
using InvoiceMgmt.BAL.Service.BulkUploads;
using InvoiceMgmt.DAL.GenericRepository;
using InvoiceMgmt.DAL.IRepo;
using InvoiceMgmt.DAL.IRepo.BulkUploads;
using InvoiceMgmt.DAL.Repo;
using InvoiceMgmt.DAL.Repo.BulkUploads;
using Microsoft.OpenApi.Models;

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

            collection.AddScoped<IBulkUploadsCustomersService, BulkUploadsCustomersService>();
            collection.AddScoped<IUploadBulkCustomersRepo, UploadBulkCustomersRepo>();


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
        public static void ConfigureServices(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Invoice API", Version = "v1" });
                c.OperationFilter<SwaggerFileUploadOperationFilter>(); // Add the custom filter here
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
