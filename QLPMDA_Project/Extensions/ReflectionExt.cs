using System.Collections;
using System.ComponentModel;
using System.Reflection;

namespace QLPMDA_Project.Extensions
{
    public static class ReflectionExt
    {
        public const string IdField = "Id";

        public const string ParentCopyId = "ParentCopyId";

        public const string StatusIdField = "StatusId";

        public const string FreightStateId = "FreightStateId";

        public static bool IsSimple(this Type type)
        {
            if ((object)type == null)
            {
                throw new ArgumentNullException("type is null");
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return type.GetGenericArguments()[0].IsSimple();
            }

            return type.IsPrimitive || type.IsEnum || type.Equals(typeof(string)) || type.Equals(typeof(DateTime)) || type.Equals(typeof(DateTimeOffset)) || type.Equals(typeof(TimeSpan)) || type.Equals(typeof(decimal));
        }

        public static bool IsDate(this Type type)
        {
            return type.Equals(typeof(DateTime)) || type.Equals(typeof(DateTime?)) || type.Equals(typeof(DateTimeOffset)) || type.Equals(typeof(DateTimeOffset?));
        }

        public static bool IsNumber(this Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return type.GetGenericArguments()[0].IsNumber();
            }

            return type == typeof(sbyte) || type == typeof(byte) || type == typeof(short) || type == typeof(ushort) || type == typeof(int) || type == typeof(uint) || type == typeof(int) || type == typeof(ulong) || type == typeof(float) || type == typeof(double) || type == typeof(decimal);
        }

        public static bool IsDecimal(this Type type)
        {
            return type == typeof(decimal) || type == typeof(decimal?);
        }

        public static bool IsInt32(this Type type)
        {
            return type == typeof(int) || type == typeof(int?);
        }

        public static bool IsBool(this Type type)
        {
            return type == typeof(bool) || type == typeof(bool?);
        }

        public static void CopyPropFrom(this object target, object source, params string[] ignoreFields)
        {
            target.CopyPropFromInternal(source, new HashSet<int>(), 0, 0, ignoreFields);
        }

        public static void CopyPropFromAct(this object target, object source)
        {
            if (source == null || target == null)
            {
                return;
            }

            Type type = target.GetType();
            if (type.IsSimple())
            {
                return;
            }

            IEnumerable<PropertyInfo> enumerable = from x in type.GetProperties()
                                                   where x.CanRead && x.CanWrite
                                                   select x;
            foreach (PropertyInfo item in enumerable)
            {
                object propValue = source.GetPropValue(item.Name);
                if (propValue != null)
                {
                    if (item.PropertyType.IsDecimal() && propValue != null && !propValue.GetType().IsDecimal())
                    {
                        target.SetPropValue(item.Name, Convert.ToDecimal(propValue));
                    }
                    else if (item.PropertyType.IsDate() && propValue != null && !propValue.GetType().IsDate())
                    {
                        target.SetPropValue(item.Name, Convert.ToDateTime(propValue));
                    }
                    else if (item.PropertyType.IsSimple())
                    {
                        target.SetPropValue(item.Name, propValue);
                    }
                    else
                    {
                        target.GetPropValue(item.Name).CopyPropFromAct(propValue);
                    }
                }
            }
        }

        public static void CopyPropFrom(this object target, object source, int maxLevel = 0, params string[] ignoreFields)
        {
            target.CopyPropFromInternal(source, new HashSet<int>(), 0, maxLevel, ignoreFields);
        }

        public static void CopyPropFromInternal(this object target, object source, HashSet<int> visited, int currentLevel = 0, int maxLevel = 0, params string[] ignoreFields)
        {
            bool flag = maxLevel == 0 || currentLevel < maxLevel;
            if (source == null || target == null || !flag)
            {
                return;
            }

            Type type = target.GetType();
            if (type.IsSimple())
            {
                return;
            }

            int hashCode = source.GetHashCode();
            if (visited.Contains(hashCode))
            {
                return;
            }

            visited.Add(hashCode);
            if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                IEnumerator enumerator = (source as IEnumerable).GetEnumerator();
                IEnumerator enumerator2 = (target as IEnumerable).GetEnumerator();
                while (enumerator.MoveNext() && enumerator2.MoveNext())
                {
                    enumerator2.Current.CopyPropFromInternal(enumerator.Current, visited, currentLevel + 1, maxLevel, ignoreFields);
                }

                return;
            }

            IEnumerable<PropertyInfo> enumerable = from x in type.GetProperties()
                                                   where x.CanRead && x.CanWrite
                                                   select x;
            foreach (PropertyInfo item in enumerable)
            {
                if (ignoreFields == null || !ignoreFields.Contains(item.Name))
                {
                    object propValue = source.GetPropValue(item.Name);
                    if (item.PropertyType.IsDecimal() && propValue != null && !propValue.GetType().IsDecimal())
                    {
                        target.SetPropValue(item.Name, Convert.ToDecimal(propValue));
                    }
                    else if (item.PropertyType.IsDate() && propValue != null && !propValue.GetType().IsDate())
                    {
                        target.SetPropValue(item.Name, Convert.ToDateTime(propValue));
                    }
                    else if (item.PropertyType.IsSimple())
                    {
                        target.SetPropValue(item.Name, propValue);
                    }
                    else
                    {
                        target.GetPropValue(item.Name).CopyPropFromInternal(propValue, visited, currentLevel + 1, maxLevel, ignoreFields);
                    }
                }
            }
        }

        public static void ClearReferences(this object obj, bool clearInnerEnum = false, HashSet<object> visited = null)
        {
            if (visited == null)
            {
                visited = new HashSet<object>();
            }

            if (obj == null || visited.Contains(obj) || obj.GetType().IsSimple())
            {
                return;
            }

            visited.Add(obj);
            if (obj is IEnumerable enumerable)
            {
                {
                    foreach (object item in enumerable)
                    {
                        item.ClearReferences(clearInnerEnum, visited);
                    }

                    return;
                }
            }

            ClearRefInternal(obj, clearInnerEnum, visited);
        }

        private static void ClearRefInternal(object obj, bool clearInnerEnum, HashSet<object> visited)
        {
            IEnumerable<PropertyInfo> enumerable = from x in obj.GetType().GetProperties()
                                                   where x.CanWrite && x.CanWrite
                                                   select x;
            foreach (PropertyInfo item in enumerable)
            {
                string text = item.Name.SubStrIndex(0, item.Name.Length - 2);
                IEnumerable enumerable2 = item.GetValue(obj) as IEnumerable;
                if (!item.PropertyType.IsSimple() && enumerable2 == null)
                {
                    item.SetValue(obj, null);
                }

                if (!(item.PropertyType != typeof(string)) || enumerable2 == null)
                {
                    continue;
                }

                if (clearInnerEnum)
                {
                    item.SetValue(obj, null);
                    continue;
                }

                foreach (object item2 in enumerable2)
                {
                    item2.ClearReferences(clearInnerEnum, visited);
                }
            }
        }

        public static string GetEnumDescription(this Enum value, Type enumType = null)
        {
            enumType = enumType ?? value.GetType();
            FieldInfo field = enumType.GetField(value.ToString());
            if ((object)field == null)
            {
                return string.Empty;
            }

            if (field.GetCustomAttributes(typeof(DescriptionAttribute), inherit: false) is DescriptionAttribute[] source && source.Any())
            {
                return source.First().Description;
            }

            return value.ToString();
        }

        public static object DeepCopy(object obj, string path = null)
        {
            if (obj == null)
            {
                return null;
            }

            Type type = obj.GetType();
            object propValue = obj.GetPropValue("ModelName");
            if (propValue != null)
            {
                type = Type.GetType(path + propValue);
            }

            if (type.IsSimple())
            {
                return obj;
            }

            if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                object obj2 = Activator.CreateInstance(type);
                Type type2 = type.GetGenericArguments()[0];
                IEnumerator enumerator = (obj as IEnumerable).GetEnumerator();
                MethodInfo method = obj2.GetType().GetMethod("Add", BindingFlags.Public);
                while (enumerator.MoveNext())
                {
                    method.MakeGenericMethod(type2)?.Invoke(obj2, new object[1] { DeepCopy(enumerator.Current) });
                }

                return obj2;
            }

            if (type.IsClass)
            {
                object obj3 = new object();
                obj3 = ((propValue == null) ? Activator.CreateInstance(obj.GetType()) : Activator.CreateInstance(Type.GetType(path + propValue)));
                FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                FieldInfo[] array = fields;
                foreach (FieldInfo fieldInfo in array)
                {
                    object value = fieldInfo.GetValue(obj);
                    if (value != null)
                    {
                        fieldInfo.SetValue(obj3, DeepCopy(value));
                    }
                }

                return obj3;
            }

            return null;
        }

        public static List<T> CopyRowWithoutId<T>(List<T> selectedRows, string path = null)
        {
            return selectedRows.Select(delegate (T x)
            {
                T val = (T)DeepCopy(x, path);
                val.SetPropValue("Id", -Math.Abs(val.GetHashCode()));
                val.SetPropValue("ParentCopyId", x.GetPropValue("Id"));
                ProcessObjectRecursive(val, delegate (object obj)
                {
                    int? num = obj.GetPropValue("Id") as int?;
                    if (num.HasValue && num.Value > 0)
                    {
                        obj.SetPropValue("Id", 0);
                    }

                    int? num2 = obj.GetPropValue("StatusId") as int?;
                    if (num2.HasValue)
                    {
                        obj.SetPropValue("StatusId", 2);
                    }

                    int? num3 = obj.GetPropValue("FreightStateId") as int?;
                    if (num2.HasValue)
                    {
                        obj.SetPropValue("FreightStateId", 1);
                    }
                });
                val.ClearReferences();
                return val;
            }).ToList();
        }

        public static void ProcessObjectRecursive(object obj, Action<object> action, HashSet<object> visited = null)
        {
            if (obj == null || obj.GetType().IsSimple())
            {
                return;
            }

            if (visited == null)
            {
                visited = new HashSet<object>();
            }

            if (visited.Contains(obj))
            {
                return;
            }

            visited.Add(obj);
            if (obj is IEnumerable enumerable)
            {
                try
                {
                    IEnumerator enumerator = enumerable.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        ProcessObjectRecursive(enumerator.Current, action, visited);
                    }

                    return;
                }
                catch
                {
                    return;
                }
            }

            action(obj);
            IEnumerable<PropertyInfo> enumerable2 = from x in obj.GetType().GetProperties()
                                                    where !x.PropertyType.IsSimple() && typeof(IEnumerable).IsAssignableFrom(x.PropertyType)
                                                    select x;
            foreach (PropertyInfo item in enumerable2)
            {
                ProcessObjectRecursive(item.GetValue(obj), action, visited);
            }
        }

        public static Dictionary<Key, T> ToDictionaryDistinct<T, Key>(this IEnumerable<T> source, Func<T, Key> keySelector)
        {
            return (from g in source?.GroupBy(keySelector)
                    select g.First()).ToDictionary(keySelector);
        }
    }
}
