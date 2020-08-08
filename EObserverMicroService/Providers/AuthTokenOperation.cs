using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EObserver.Providers
{
    public class AuthTokenOperation : IDocumentFilter
    {
        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            swaggerDoc.Paths.Add("/api/token", new PathItem
            {
                Post = new Operation
                {
                    Tags = new List<string> { "Auth" },
                    Consumes = new List<string>
                    {
                        "application/x-www-form-urlencoded"
                    },
                    Parameters = new List<IParameter>
                    {
                        new CustParameter
                        {
                            Name = "grant_type",
                            Required = true,
                            In = "formData"
                        },

                        new CustParameter
                        {
                            Name = "username",
                            Required = false,
                            In = "formData"
                        },
                         new CustParameter
                        {
                            Name = "password",
                            Required = true,
                            In = "formData"
                        },

                    },
                    Responses = new Dictionary<string, Response>()
                    {
                        { "200", new Response() }
                    }
                }
            });

        }

        public class CustParameter : IParameter
        {
            public string Name { get; set; }
            public string In { get; set; }
            public string Description { get; set; }
            public bool Required { get; set; }

            public Dictionary<string, object> Extensions { get; }
        }
    }
}
