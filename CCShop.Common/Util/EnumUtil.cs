using System;
using System.ComponentModel;
using System.Reflection;

namespace CCShop.Common.Util
{
    public class EnumUtil
    {
        /// <summary>
        /// 根据枚举实体获取对应的描述信息
        /// </summary>
        /// <param name="obj">枚举实体</param>
        /// <returns>描述信息</returns>
        public static string GetDescription(Enum obj)
        {
            string objName = obj.ToString();
            Type t = obj.GetType();
            FieldInfo fi = t.GetField(objName);
            DescriptionAttribute[] arrDesc = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return arrDesc[0].Description;
        }

        /// <summary>
        /// 根据枚举值获取对应的枚举对象
        /// </summary>
        /// <typeparam name="T">枚举对象</typeparam>
        /// <param name="valueObj">枚举值</param>
        /// <returns>枚举对象</returns>
        public static T GetEnum<T>(string valueObj)
        {
            return (T)Enum.Parse(typeof(T), valueObj);
        }
    }
}
