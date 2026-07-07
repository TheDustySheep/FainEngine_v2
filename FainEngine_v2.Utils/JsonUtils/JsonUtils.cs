using System.Reflection;
using System.Text;

namespace FainEngine_v2.Utils.JsonUtils;

public static class JsonUtils
{
    public static string ToJsonLikeString(object obj)
    {
        if (obj == null) return "{ }";

        var type = obj.GetType();
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);

        var sb = new StringBuilder();
        sb.Append("{ ");

        bool first = true;
        foreach (var f in fields)
        {
            var v = f.GetValue(obj);

            if (v is null)
                continue;

            if (!first)
                sb.Append(", ");

            string formatted = v is string s ? $"\"{s}\"" : v.ToString();

            sb.Append($"\"{f.Name}\": {formatted}");
            first = false;
        }

        sb.Append(" }");
        return sb.ToString();
    }
}
