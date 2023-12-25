using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ColorPicker.utils
{
    public static class ColorUtils
    {
        /// <summary>
        /// 尝试转换颜色
        /// </summary>
        /// <param name="input"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static bool TryParseColor(string input, out Color color)
        {
            color = Colors.Transparent;

            try
            {
                // 尝试解析为十六进制
                if (input.StartsWith("#"))
                {
                    color = (Color)ColorConverter.ConvertFromString(input);
                }
                // 尝试解析为0-255或0-1
                else
                {
                    color = (Color)ColorConverter.ConvertFromString($"#{input}");
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
