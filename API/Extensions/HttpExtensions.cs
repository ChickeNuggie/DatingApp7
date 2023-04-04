using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using API.Helpers;

namespace API.Extensions
{   // Create HTTP extension method that extends HTTP response object or class, making it easier to use.
    public static class HttpExtensions
    {
        public static void AddPaginationHeader(this HttpResponse response, PaginationHeader header)
        {
            // Convert header into JSON format from C# object
            var jsonOptions = new JsonSerializerOptions{PropertyNamingPolicy = JsonNamingPolicy.CamelCase};
            //access response and header and add json serializer on the response header.x
            response.Headers.Add("Pagination", JsonSerializer.Serialize(header, jsonOptions));
            //access information inside of the header
            response.Headers.Add("Access-Control-Exposed-Headers", "Pagination");
            //
        }
    }
}