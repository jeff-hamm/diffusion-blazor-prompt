using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButtsBlazor.Shared.Services
{
    public static class Int32Extensions
    {
        public static string ToBase64StringNoPadding(this int @this) =>
            Convert.ToBase64String(BitConverter.GetBytes(@this))[..^2];
        public static string ToBase64String(this int? @this) =>
            @this.HasValue ? Convert.ToBase64String(BitConverter.GetBytes(@this.Value)) : "";

        public static int FromBase64ToInt(this string @this) =>
            BitConverter.ToInt32(Convert.FromBase64String(@this));
    }
}
