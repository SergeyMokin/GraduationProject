using System;
using System.Text;

namespace GraduationProjectModels
{
    public static class Defines
    {
        #region Get decoded string.
        public static string GetDecodedString(string str)
        {
            var result = Encoding
                .UTF8
                .GetString(Convert.FromBase64String(Swap(str)));
            
            for (var i = 0; i < 3; i++)
            {
                result = Encoding
                    .UTF8
                    .GetString(Convert.FromBase64String(Swap(result)));
            }

            return Swap(result);
        }

        private static string Swap(string str)
        {
            var res = "";
            for (var i = 0; i < str.Length; i++)
            {
                if (i < str.Length - 2)
                {
                    res += $"{str[i + 1]}{str[i]}";
                    i++;
                }
                else
                {
                    res += str[i];
                }
            }
            return res;
        }
        #endregion
    }
}
