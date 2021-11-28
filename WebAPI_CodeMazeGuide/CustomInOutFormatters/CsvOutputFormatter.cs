using Microsoft.AspNetCore.Mvc.Formatters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using Entities.DTOs;

namespace WebAPI_CodeMazeGuide.CustomInOutFormatters
{
    public class CsvOutputFormatter : TextOutputFormatter
    {
        public CsvOutputFormatter()
        {
            this.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/csv"));
            this.SupportedEncodings.Add(Encoding.UTF8);
            this.SupportedEncodings.Add(Encoding.Unicode);
        }

        protected override bool CanWriteType(Type type)
        {
            if(type.IsAssignableTo(typeof(CompanyDTO)) || 
                type.IsAssignableTo(typeof(IEnumerable<CompanyDTO>)))
            {
                return base.CanWriteType(type);
            }

            return false;
        }

        public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            var buffer = new StringBuilder();

            if(context.Object is IEnumerable<CompanyDTO> companies) 
            {
                foreach(var company in companies)
                {
                    FormatCsv(buffer, company);
                }
            }
            else if(context.Object is CompanyDTO company)
            {
                FormatCsv(buffer, company);
            }

            return context.HttpContext.Response.WriteAsync(buffer.ToString(), selectedEncoding);
        }

        private void FormatCsv(StringBuilder buffer, CompanyDTO company)
        {
            buffer.AppendLine($"{company.Id},\"{company.Name},\"{company.FullAddress}\"");
        }
    }
}
