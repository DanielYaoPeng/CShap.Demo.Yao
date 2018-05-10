using System.Web.Http;
using WebActivatorEx;
using MongoDBCRUD_YP;
using Swashbuckle.Application;
using System;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace MongoDBCRUD_YP
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                    {
                        
                        c.SingleApiVersion("v1", "MongoDBCRUD_YP");

                      
                        c.IncludeXmlComments(GetXmlCommentsPath(thisAssembly.GetName().Name));

                       
                    })
                .EnableSwaggerUi(c =>
                    {
                    });
           
        }
        protected static string GetXmlCommentsPath(string name)
        {
            return string.Format(@"{0}\bin\{1}.XML", AppDomain.CurrentDomain.BaseDirectory, name);
        }
    }
}
