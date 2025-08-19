using Microsoft.AspNetCore.Mvc.Formatters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRYLibrary.Core.APIServer.Formatter
{
    public class ByteArrayInputFormatter : InputFormatter
    {
        public ByteArrayInputFormatter()
        {
            SupportedMediaTypes.Add(Microsoft.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/octet-stream"));
        }

        protected override bool CanReadType(Type type)
        {
            return type == typeof(byte[]);
        }

        public override Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            var stream = new MemoryStream();
            context.HttpContext.Request.Body.CopyToAsync(stream).Wait();
            return InputFormatterResult.SuccessAsync(stream.ToArray());
        }
    }
}