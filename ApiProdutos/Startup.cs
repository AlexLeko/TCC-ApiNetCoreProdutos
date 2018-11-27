using ApiProdutos.Data;
using ApiProdutos.Data.Interface;
using ApiProdutos.Repository;
using ApiProdutos.Repository.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ApiProdutos
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IApiProdutoContext, ApiProdutosContext>();

            services.AddScoped<IProdutoRepository, ProdutoRepository>();


            services.Configure<ConfigContext>(
                options =>
                {
                    options.ConnectionString = Configuration.GetSection("MongoDb:ConnectionString").Value;
                    options.Database = Configuration.GetSection("MongoDb:Database").Value;
                });

            services.AddMvc()
                //.AddJsonOptions(options => {

                //var settings = options.SerializerSettings;

                //settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                //settings.MissingMemberHandling = MissingMemberHandling.Ignore;
                //settings.Formatting = Formatting.Indented;
                //settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

                ////options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                ////options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Include;
                ////options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                //})
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
