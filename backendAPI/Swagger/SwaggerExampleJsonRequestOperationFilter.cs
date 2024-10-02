
using log4net;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace WebAPITemplate.Swagger
{
    public class SwaggerExampleJsonRequestOperationFilter : IOperationFilter
    {
        
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            SwaggerExampleJsonRequestAttribute requestAttribute = context.MethodInfo.GetCustomAttributes(true)
               .SingleOrDefault((attribute) => attribute is SwaggerExampleJsonRequestAttribute) as SwaggerExampleJsonRequestAttribute;
            if (requestAttribute != null)
            {
                string json = "";
                if (!string.IsNullOrEmpty(requestAttribute.FileName))
                {
                    //string[] f = Directory.GetFileSystemEntries("/app", "*", SearchOption.AllDirectories);
                    //var csvs = Array.FindAll(f, a => a.Contains(".csv"));
                    //var dlls = Array.FindAll(f, a => a.Contains(".dll"));
                  
                    var filename = Path.GetFullPath(string.Format("Swagger/SwaggerExample/{0}.json", requestAttribute.FileName));                  
                    try
                    {
                        if (File.Exists(filename))
                        {
                            using (StreamReader r = new StreamReader(filename))
                            {
                                json = r.ReadToEnd();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }

                operation.RequestBody = new OpenApiRequestBody() { Required = true };
                var header = "application/json";
                if (!string.IsNullOrEmpty(requestAttribute.RequestHeader)) header = requestAttribute.RequestHeader;
                operation.RequestBody.Content.Add(header, new OpenApiMediaType()
                {
                    Schema = new OpenApiSchema
                    {
                        //temporary disable 
                        //Reference = new OpenApiReference { Type = ReferenceType.Schema, Id = requestAttribute.FileName }
                    },
                    Example = new Microsoft.OpenApi.Any.OpenApiString(json)
                });

            }
        }

    }
}
