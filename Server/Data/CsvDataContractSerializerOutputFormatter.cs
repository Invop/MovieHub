using System.Text;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace MovieHub.Server.Data;

public class CsvDataContractSerializerOutputFormatter : TextOutputFormatter
{
    public CsvDataContractSerializerOutputFormatter()
    {
        SupportedMediaTypes.Add("text/csv");
        SupportedEncodings.Add(Encoding.Unicode);
    }

    public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
    {
        var query = (IQueryable)context.Object;

        var queryString = context.HttpContext.Request.QueryString;
        var columns = queryString.Value.Contains("$select")
            ? OutputFormatter.GetPropertiesFromSelect(queryString.Value, query.ElementType)
            : OutputFormatter.GetProperties(query.ElementType);

        var sb = new StringBuilder();

        foreach (var item in query)
        {
            var row = new List<string>();

            foreach (var column in columns)
            {
                var value = OutputFormatter.GetValue(item, column.Key);

                row.Add($"{value}".Trim());
            }

            sb.AppendLine(string.Join(",", row.ToArray()));
        }

        return context.HttpContext.Response.WriteAsync(
            $"{string.Join(",", columns.Select(c => c.Key))}{Environment.NewLine}{sb}",
            selectedEncoding,
            context.HttpContext.RequestAborted);
    }
}