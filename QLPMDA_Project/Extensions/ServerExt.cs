using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace QLPMDA_Project.Extensions
{
    public static class ServerExt
    {
        public static object GetPropValue(this object obj, string propName)
        {
            return obj?.GetType().GetProperty(propName)?.GetValue(obj);
        }

        public static void SetPropValue(this object instance, string propertyName, object value)
        {
            Type type = instance.GetType();
            PropertyInfo property = type.GetProperty(propertyName);
            if (property != null)
            {
                property.SetValue(instance, value, null);
            }
        }

        public static void SetReadonlyPropValue(this object instance, string propertyName, object value)
        {
            Type type = instance.GetType();
            PropertyInfo property = type.BaseType.GetProperty(propertyName);
            if (property != null)
            {
                property.SetValue(instance, value, null);
            }
        }

        public static string ToJson(this object value)
        {
            return JsonConvert.SerializeObject(value, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
        }

        public static (bool, object) GetComplexProp(this object obj, string propName)
        {
            if (obj == null || string.IsNullOrWhiteSpace(propName))
            {
                return (false, null);
            }

            string[] array = propName.Split('.');
            if (array.Length == 0)
            {
                return (false, null);
            }

            if (array.Length == 1)
            {
                return (obj.GetType().GetProperty(propName) != null, obj.GetPropValue(propName));
            }

            string text = array.LastOrDefault();
            array = array.Take(array.Length - 1).ToArray();
            object obj2 = obj;
            string[] array2 = array;
            foreach (string propName2 in array2)
            {
                if (obj2 == null)
                {
                    return (false, null);
                }

                obj2 = obj2.GetPropValue(propName2);
            }

            return (obj2 != null && obj2.GetType().GetProperty(text) != null, obj2.GetPropValue(text));
        }
    }
}
