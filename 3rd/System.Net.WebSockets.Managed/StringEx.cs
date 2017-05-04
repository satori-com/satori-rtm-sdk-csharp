using System.Collections.Generic;
using System.Text;

namespace System
{
    public static class StringEx
    {
        public static bool IsNullOrWhitespace(String value)
        {
            #if NET_4_5_COMPAT

            if (value == null) return true;

            for (int i = 0; i < value.Length; i++)
            {
                if (!Char.IsWhiteSpace(value[i])) return false;
            }

            return true;

            #else
            return string.IsNullOrWhitespace(value);
            #endif
        }

        #if NET_4_5_COMPAT
        public static StringBuilder Clear(this StringBuilder sb) 
        {
            sb.Length = 0;
            return sb;
        }
        #endif

        public static string Join(string separator, params object[] items)
        {
            #if NET_4_5_COMPAT
            return Join(separator, items ?? new object[0]);
            #else
            return string.Join(separator, items ?? new object[0]);
            #endif
        }

        public static string Join<T>(string separator, IEnumerable<T> items)
        {
            #if NET_4_5_COMPAT

            var sb = new StringBuilder();
            bool first = true;
            foreach (var item in items) 
            {
                if (!first)
                {
                    sb.Append(separator);
                }
                first = false;
                sb.Append(item?.ToString());
            }

            return sb.ToString();

#else
            return string.Join(separator, items);
#endif
        }
    }
}
