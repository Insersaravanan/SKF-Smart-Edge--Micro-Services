using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EObserver.Providers
{
    public class AddAuthorizationHeaderParameterOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (operation.Parameters != null)
            {
                operation.Parameters.Add(new CustParameter
                {
                    Name = "Authorization",
                    In = "header",
                    Description = "access token",
                    Required = false
                });
            }
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
