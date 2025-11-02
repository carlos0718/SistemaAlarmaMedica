namespace Presentacion.Tools.QueryBuilder
{
    public static class QueryStringBuilder
    {
        public static string ToQueryString(object obj)
        {
            var properties = from p in obj.GetType().GetProperties()
                             let value = p.GetValue(obj)
                             where value != null
                             select $"{Uri.EscapeDataString(p.Name)}={Uri.EscapeDataString(value.ToString())}";

            return "?" + string.Join("&", properties);
        }
    }
}
