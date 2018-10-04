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
            
            for (var i = 0; !(i >= 0b11); i++)
            {
                result = Encoding
                    .UTF8
                    .GetString(Convert.FromBase64String(Swap(result)));
            }

            return Swap(result);
        }

        private static string Swap(string str)
        {
            var res = new StringBuilder();
            for (var i = 0; i < str.Length; i++)
            {
                if (i < str.Length - 0b10)
                {
                    res.Append($"{str[i + 0b1]}{str[i]}");
                    i++;
                }
                else
                {
                    res.Append(str[i]);
                }
            }
            return res.ToString();
        }
        #endregion
    }
}
