using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Reflection;
using System.Collections;

namespace ControlTravelAgencySystem.Common
{
    public class Utils
    {
        /**
         * Хэширование строки 
         */
        public static string md5(string input)
        {
            MD5 md5 = MD5.Create();
            byte[] hash = md5.ComputeHash(Encoding.ASCII.GetBytes(input));

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Конвертация timestamp в тип Datetime
        /// </summary>
        /// <param name="unixTimeStamp"></param>
        /// <returns></returns>
        public static DateTime tsToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        /// <summary>
        /// Конвертация datetime в таймштам (обратно)
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static int dtToTimestamp(DateTime dt)
        {
            return (int)(dt.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }

        public static string toJson(object obj)
        {
            return Regex.Replace(new HtmlString(JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                StringEscapeHandling = StringEscapeHandling.EscapeHtml,
                MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                MaxDepth = 10

            })).ToString(), "(\"(?:[^\"\\\\]|\\\\.)*\")|\\s+", "$1");
        }

        /// <summary>
        /// Сериализация объекта по требуемым полям в формат JSON 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public static string toJsonByCustomProperties(object obj, List<object> properties)
        {
            return toJson(serializeToDictionary(obj, properties));
        }

        /// <summary>
        /// Получения словаря с объекта по требуемым полям
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public static Dictionary<string, object> serializeToDictionary(object obj, List<object> properties)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();

            foreach (var property in properties)
            {
                if (property.GetType() == typeof(string))
                {
                    string key = (string)property;
                    var value = obj.GetType().GetProperty(key).GetValue(obj, null);
                    result.Add(key, value);
                }
                else
                {
                    Type instance = property.GetType();

                    string key = (string)instance
                        .GetProperty("key")
                        .GetValue(property);

                    object childObject = obj.GetType()
                            .GetProperty(key)
                            .GetValue(obj);

                    List<object> childProperties = (List<object>)instance
                            .GetProperty("properties")
                            .GetValue(property);
                    
                    if (childObject != null)
                    {
                        PropertyInfo propertyInfo = instance.GetProperty("type");
                        string type = propertyInfo != null ? (string)propertyInfo.GetValue(property, null) : null;
                        if (type == null || type == "object")
                        {
                            result.Add(key, serializeToDictionary(childObject, childProperties));
                        }
                        else
                        {
                            List<object> list = new List<object>();
                            foreach (var item in (IEnumerable)childObject)
                            {
                                list.Add(serializeToDictionary(item, childProperties));
                            }
                            
                            result.Add(key, list);
                        }
                    }
                    else
                    {
                        result.Add(key, null);
                    }
                }
            }

            return result;
        }
    }
}