using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace QLPMDA_Project.Extensions
{
    public static class Utils
    {
        public const int SystemId = 1;

        public const string TenantField = "t";

        public const string Pixel = "px";

        public const string FeatureField = "f";

        public const string QuestionMark = "?";

        public const string Amp = "&";

        public const string ApplicationJson = "application/json";

        public const string Authorization = "Authorization";

        public const int SelfVendorId = 65;

        public const string AutoSaveReason = "Tự động cập nhật";

        public const string IdField = "Id";

        public const string NewLine = "\r\n";

        public const string Indent = "\t";

        public const string Dot = ".";

        public const string Comma = ",";

        public const string Semicolon = ";";

        public const string Space = " ";

        public const int ComponentId = 20;

        public const int ComponentGroupId = 30;

        public const int GridPolicyId = 2077;

        public const int HistoryId = 4199;

        public const string InsertedBy = "InsertedBy";

        public const string OwnerId = "OwnerId";

        public const string Balancer = "Balancer";

        public const string GOOGLE_MAP = "https://maps.googleapis.com/maps/api/js?key=AIzaSyCr_2PaKJplCyvwN4q78lBkX3UBpfZ_HsY";

        public const string GOOGLE_MAP_PLACES = "https://maps.googleapis.com/maps/api/js?key=AIzaSyBfVrTUFatsZTyqaCKwRzbj09DD72VxSwc&libraries=places";

        public const string GOOGLE_MAP_GEOMETRY = "https://maps.googleapis.com/maps/api/js?key=AIzaSyBfVrTUFatsZTyqaCKwRzbj09DD72VxSwc&libraries=geometry";

        public const string GOOGLE_MAP_WEEKLY = "https://maps.googleapis.com/maps/api/js?key=AIzaSyBfVrTUFatsZTyqaCKwRzbj09DD72VxSwc&libraries=&v=weekly";

        public const string GOOGLE_MAP_GEO_REQUEST = "https://maps.googleapis.com/maps/api/geocode/json?key=AIzaSyBfVrTUFatsZTyqaCKwRzbj09DD72VxSwc";

        public static Dictionary<string, string> SpecialSQLChar = new Dictionary<string, string>
    {
        { "$", "" },
        { " eq ", " = " },
        { " gt ", " > " },
        { " ge ", " >= " },
        { " lt ", " < " },
        { " le ", " <= " },
        { " eq false", " = 0" },
        { " = false", " = 0" },
        { " = true", " = 1" },
        { " eq true", " = 1" },
        { " eq null", " is null" },
        { " ne null", " is not null" }
    };

        public static Dictionary<string, string> PatternSQLChar = new Dictionary<string, string> { { " in (\\(.+?\\)) = 0", " not in $1" } };

        public static Dictionary<char, string> SpecialChar = new Dictionary<char, string>
    {
        { '+', "%2B" },
        { '/', "%2F" },
        { '?', "%3F" },
        { '#', "%23" },
        { '&', "%26" }
    };

        public static Dictionary<string, char> ReverseSpecialChar = SpecialChar.ToDictionary((KeyValuePair<char, string> x) => x.Value, (KeyValuePair<char, string> x) => x.Key);

        public static Func<string, string> NullFormatHandler = (string x) => "null";

        public static Func<string, string> NotFoundHandler = (string x) => "{" + x + "}";

        public static Func<string, string> EmptyFormat = (string x) => string.Empty;

        public static string EncodeSpecialChar(this string str)
        {
            if (str == null)
            {
                return null;
            }

            char[] array = str.ToCharArray();
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < array.Length; i++)
            {
                if (SpecialChar.ContainsKey(array[i]))
                {
                    stringBuilder.Append(SpecialChar[array[i]]);
                }
                else
                {
                    stringBuilder.Append(array[i]);
                }
            }

            return stringBuilder.ToString();
        }

        public static string EncodeSQLSpecialChar(this string str)
        {
            if (str == null)
            {
                return null;
            }

            foreach (KeyValuePair<string, string> item in SpecialSQLChar)
            {
                str = str.Replace(item.Key, item.Value);
            }

            foreach (KeyValuePair<string, string> item2 in PatternSQLChar)
            {
                str = Regex.Replace(str, item2.Key, item2.Value);
            }

            return str;
        }

        public static object EncodeProperties(this object value)
        {
            PropertyInfo[] array = value?.GetType().GetProperties();
            PropertyInfo[] array2 = array;
            foreach (PropertyInfo propertyInfo in array2)
            {
                if (propertyInfo.PropertyType == typeof(string) && propertyInfo.CanWrite)
                {
                    string str = propertyInfo.GetValue(value, null)?.ToString();
                    propertyInfo.SetValue(value, str.EncodeSpecialChar(), null);
                }
            }

            return value;
        }

        public static object DecodeProperties(this object value)
        {
            PropertyInfo[] array = value?.GetType().GetProperties();
            PropertyInfo[] array2 = array;
            foreach (PropertyInfo propertyInfo in array2)
            {
                if (propertyInfo.PropertyType == typeof(string) && propertyInfo.CanWrite)
                {
                    string str = propertyInfo.GetValue(value, null)?.ToString();
                    propertyInfo.SetValue(value, str.DecodeSpecialChar(), null);
                }
            }

            return value;
        }

        public static string DecodeSpecialChar(this string str)
        {
            if (str == null)
            {
                return null;
            }

            char[] array = str.ToCharArray();
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == '%' && i + 3 <= array.Length && ReverseSpecialChar.ContainsKey(str.Substring(i, 3)))
                {
                    stringBuilder.Append(ReverseSpecialChar[str.Substring(i, 3)]);
                    i += 2;
                }
                else
                {
                    stringBuilder.Append(array[i]);
                }
            }

            return stringBuilder.ToString();
        }

        public static object GetComplexPropValue(this object obj, string propName)
        {
            if (obj == null || string.IsNullOrWhiteSpace(propName))
            {
                return null;
            }

            string[] array = propName.Split('.');
            object obj2 = obj;
            string[] array2 = array;
            foreach (string propName2 in array2)
            {
                if (obj2 == null)
                {
                    return null;
                }

                obj2 = obj2.GetPropValue(propName2);
            }

            return obj2;
        }

        public static string FormatEntity(string format, object source)
        {
            if (!format.HasAnyChar())
            {
                return string.Empty;
            }

            if (source == null)
            {
                return format;
            }

            return FormatEntity(format, null, source);
        }

        public static string FormatEntity(string format, IFormatProvider provider, object source, Func<string, string> nullHandler = null, Func<string, string> notFoundHandler = null)
        {
            if (format == null)
            {
                return null;
            }

            if (source == null)
            {
                return format;
            }

            if (nullHandler == null)
            {
                nullHandler = NullFormatHandler;
            }

            if (notFoundHandler == null)
            {
                notFoundHandler = NotFoundHandler;
            }

            StringBuilder stringBuilder = new StringBuilder();
            int num = 0;
            bool flag = false;
            bool flag2 = false;
            StringBuilder stringBuilder2 = new StringBuilder();
            List<object> list = new List<object>();
            for (int i = 0; i < format.Length; i++)
            {
                char c = format[i];
                switch (c)
                {
                    case '{':
                        flag = true;
                        flag2 = true;
                        stringBuilder.Append(c.ToString() + num);
                        break;
                    case ':':
                        if (flag && !flag2)
                        {
                            stringBuilder.Append(c);
                        }
                        else if (flag)
                        {
                            flag2 = false;
                            stringBuilder.Append(c);
                            GetValues(source, nullHandler, notFoundHandler, stringBuilder2.ToString(), list);
                            stringBuilder2.Clear();
                        }
                        else
                        {
                            stringBuilder.Append(c);
                        }

                        break;
                    case '}':
                        if (flag)
                        {
                            flag = false;
                            stringBuilder.Append(c);
                            num++;
                            GetValues(source, nullHandler, notFoundHandler, stringBuilder2.ToString(), list);
                            stringBuilder2.Clear();
                        }

                        break;
                    default:
                        if (flag && flag2)
                        {
                            stringBuilder2.Append(c);
                        }
                        else
                        {
                            stringBuilder.Append(c);
                        }

                        break;
                }
            }

            return string.Format(provider, stringBuilder.ToString(), list.ToArray());
        }

        private static void GetValues(object source, Func<string, string> nullHandler, Func<string, string> notFoundHandler, string field, List<object> objList)
        {
            if (!field.IsNullOrEmpty())
            {
                var (flag, obj) = source.GetComplexProp(field);
                if (obj == null && flag)
                {
                    obj = nullHandler(field);
                }
                else if (obj == null && !flag)
                {
                    obj = notFoundHandler(field);
                }

                objList.Add((field == "0") ? source : obj);
            }
        }

        public static string GenerateRandomToken(int? maxLength = 32)
        {
            StringBuilder stringBuilder = new StringBuilder();
            Random random = new Random();
            for (int i = 0; i < maxLength; i++)
            {
                char value = Convert.ToChar(Convert.ToInt32(Math.Floor(26.0 * random.NextDouble() + 65.0)));
                stringBuilder.Append(value);
            }

            return stringBuilder.ToString();
        }

        public static bool CheckDataPath(this string path, int parentId)
        {
            return path.Contains("\\" + parentId + "\\");
        }

        public static int UniqueNegativeId(object data)
        {
            return -Math.Abs(data.GetHashCode());
        }

        public static bool CompareIdField(object entity, object cellData)
        {
            object propValue = entity.GetPropValue("Id");
            return (propValue is long num && num == (long)cellData) || (propValue is int num2 && num2 == (int)cellData);
        }

        public static string GetFormattedRow(string exp, object row)
        {
            return FormatEntity(exp, null, row, EmptyFormat, EmptyFormat);
        }

        public static DateTime LastDayOfMonth(DateTime? time = null)
        {
            DateTime dateTime = time ?? DateTime.Now;
            return new DateTime(dateTime.Year, dateTime.Month, DateTime.DaysInMonth(dateTime.Year, dateTime.Month));
        }

        public static int? TryParseInt(this string value)
        {
            if (int.TryParse(value, out var result))
            {
                return result;
            }

            return null;
        }

        public static bool? TryParseBool(this string value)
        {
            if (bool.TryParse(value, out var result))
            {
                return result;
            }

            return null;
        }

        public static decimal? TryParseDecimal(this string value)
        {
            if (value.IsNullOrWhiteSpace())
            {
                return null;
            }

            if (decimal.TryParse(value, out var result))
            {
                return result;
            }

            return null;
        }

        public static T? TryParse<T>(this string value) where T : struct
        {
            if (value.IsNullOrWhiteSpace())
            {
                return null;
            }

            Type typeFromHandle = typeof(T);
            T result;
            if (typeFromHandle.IsEnum)
            {
                return Enum.TryParse<T>(value, out result) ? new T?(result) : null;
            }

            if (typeFromHandle == typeof(int) || typeFromHandle == typeof(int?))
            {
                return Convert.ToInt32(value) as T?;
            }

            if (typeFromHandle == typeof(long) || typeFromHandle == typeof(long?))
            {
                return Convert.ToInt64(value) as T?;
            }

            if (typeFromHandle == typeof(short) || typeFromHandle == typeof(short?))
            {
                return Convert.ToInt16(value) as T?;
            }

            if (typeFromHandle == typeof(bool) || typeFromHandle == typeof(bool?))
            {
                return Convert.ToBoolean(value) as T?;
            }

            TimeSpan result2;
            if (typeFromHandle == typeof(TimeSpan) || typeFromHandle == typeof(TimeSpan?))
            {
                return TimeSpan.TryParse(value, out result2) ? (result2 as T?) : null;
            }

            DateTime result3;
            if (typeFromHandle == typeof(DateTime) || typeFromHandle == typeof(DateTime?))
            {
                return DateTime.TryParse(value, out result3) ? (result3 as T?) : null;
            }

            DateTimeOffset result4;
            if (typeFromHandle == typeof(DateTimeOffset) || typeFromHandle == typeof(DateTimeOffset?))
            {
                return DateTimeOffset.TryParse(value, out result4) ? (result4 as T?) : null;
            }

            if (typeFromHandle == typeof(byte) || typeFromHandle == typeof(byte?))
            {
                return Convert.ToByte(value) as T?;
            }

            if (typeFromHandle == typeof(char) || typeFromHandle == typeof(char?))
            {
                return Convert.ToChar(value) as T?;
            }

            if (typeFromHandle == typeof(decimal) || typeFromHandle == typeof(decimal?))
            {
                return Convert.ToDecimal(value) as T?;
            }

            if (typeFromHandle == typeof(double) || typeFromHandle == typeof(double?))
            {
                return Convert.ToDouble(value) as T?;
            }

            float result5;
            if (typeFromHandle == typeof(float) || typeFromHandle == typeof(float?))
            {
                return float.TryParse(value, out result5) ? (result5 as T?) : null;
            }

            return null;
        }

        public static object ChangeType(this string value, Type type)
        {
            if (value.IsNullOrWhiteSpace())
            {
                return null;
            }

            int result;
            if (type == typeof(int) || type == typeof(int?))
            {
                return int.TryParse(value, out result) ? new int?(result) : ((type == typeof(int)) ? new int?(0) : null);
            }

            long result2;
            if (type == typeof(long) || type == typeof(long?))
            {
                return long.TryParse(value, out result2) ? new long?(result2) : ((type == typeof(long)) ? new long?(0L) : null);
            }

            short result3;
            if (type == typeof(short) || type == typeof(short?))
            {
                return short.TryParse(value, out result3) ? new short?(result3) : ((type == typeof(short)) ? new short?(0) : null);
            }

            if (type == typeof(bool) || type == typeof(bool?))
            {
                return Convert.ToBoolean(value);
            }

            TimeSpan result4;
            if (type == typeof(TimeSpan) || type == typeof(TimeSpan?))
            {
                return TimeSpan.TryParse(value, out result4) ? new TimeSpan?(result4) : null;
            }

            DateTime result5;
            if (type == typeof(DateTime) || type == typeof(DateTime?))
            {
                return DateTime.TryParse(value, out result5) ? new DateTime?(result5) : null;
            }

            DateTimeOffset result6;
            if (type == typeof(DateTimeOffset) || type == typeof(DateTimeOffset?))
            {
                return DateTimeOffset.TryParse(value, out result6) ? new DateTimeOffset?(result6) : null;
            }

            if (type == typeof(byte) || type == typeof(byte?))
            {
                return Convert.ToByte(value);
            }

            if (type == typeof(char) || type == typeof(char?))
            {
                return Convert.ToChar(value);
            }

            decimal result7;
            if (type == typeof(decimal) || type == typeof(decimal?))
            {
                return decimal.TryParse(value, out result7) ? new decimal?(result7) : ((type == typeof(decimal)) ? new decimal?(default(decimal)) : null);
            }

            float result8;
            if (type == typeof(double) || type == typeof(double?))
            {
                return float.TryParse(value, out result8) ? new double?(result8) : ((type == typeof(double)) ? new double?(0.0) : null);
            }

            float result9;
            if (type == typeof(float) || type == typeof(float?))
            {
                return float.TryParse(value, out result9) ? new float?(result9) : ((type == typeof(float)) ? new float?(0f) : null);
            }

            return null;
        }

        public static string GetMimeType(string extension)
        {
            if (extension == null)
            {
                throw new ArgumentNullException("extension");
            }

            if (extension.StartsWith("."))
            {
                extension = extension.Substring(1);
            }

            extension = extension.ToLower();
            return extension switch
            {
                "323" => "text/h323",
                "3g2" => "video/3gpp2",
                "3gp" => "video/3gpp",
                "3gp2" => "video/3gpp2",
                "3gpp" => "video/3gpp",
                "7z" => "application/x-7z-compressed",
                "aa" => "audio/audible",
                "aac" => "audio/aac",
                "aaf" => "application/octet-stream",
                "aax" => "audio/vnd.audible.aax",
                "ac3" => "audio/ac3",
                "aca" => "application/octet-stream",
                "accda" => "application/msaccess.addin",
                "accdb" => "application/msaccess",
                "accdc" => "application/msaccess.cab",
                "accde" => "application/msaccess",
                "accdr" => "application/msaccess.runtime",
                "accdt" => "application/msaccess",
                "accdw" => "application/msaccess.webapplication",
                "accft" => "application/msaccess.ftemplate",
                "acx" => "application/internet-property-stream",
                "addin" => "text/xml",
                "ade" => "application/msaccess",
                "adobebridge" => "application/x-bridge-url",
                "adp" => "application/msaccess",
                "adt" => "audio/vnd.dlna.adts",
                "adts" => "audio/aac",
                "afm" => "application/octet-stream",
                "ai" => "application/postscript",
                "aif" => "audio/x-aiff",
                "aifc" => "audio/aiff",
                "aiff" => "audio/aiff",
                "air" => "application/vnd.adobe.air-application-installer-package+zip",
                "amc" => "application/x-mpeg",
                "application" => "application/x-ms-application",
                "art" => "image/x-jg",
                "asa" => "application/xml",
                "asax" => "application/xml",
                "ascx" => "application/xml",
                "asd" => "application/octet-stream",
                "asf" => "video/x-ms-asf",
                "ashx" => "application/xml",
                "asi" => "application/octet-stream",
                "asm" => "text/plain",
                "asmx" => "application/xml",
                "aspx" => "application/xml",
                "asr" => "video/x-ms-asf",
                "asx" => "video/x-ms-asf",
                "atom" => "application/atom+xml",
                "au" => "audio/basic",
                "avi" => "video/x-msvideo",
                "axs" => "application/olescript",
                "bas" => "text/plain",
                "bcpio" => "application/x-bcpio",
                "bin" => "application/octet-stream",
                "bmp" => "image/bmp",
                "c" => "text/plain",
                "cab" => "application/octet-stream",
                "caf" => "audio/x-caf",
                "calx" => "application/vnd.ms-office.calx",
                "cat" => "application/vnd.ms-pki.seccat",
                "cc" => "text/plain",
                "cd" => "text/plain",
                "cdda" => "audio/aiff",
                "cdf" => "application/x-cdf",
                "cer" => "application/x-x509-ca-cert",
                "chm" => "application/octet-stream",
                "class" => "application/x-java-applet",
                "clp" => "application/x-msclip",
                "cmx" => "image/x-cmx",
                "cnf" => "text/plain",
                "cod" => "image/cis-cod",
                "config" => "application/xml",
                "contact" => "text/x-ms-contact",
                "coverage" => "application/xml",
                "cpio" => "application/x-cpio",
                "cpp" => "text/plain",
                "crd" => "application/x-mscardfile",
                "crl" => "application/pkix-crl",
                "crt" => "application/x-x509-ca-cert",
                "cs" => "text/plain",
                "csdproj" => "text/plain",
                "csh" => "application/x-csh",
                "csproj" => "text/plain",
                "css" => "text/css",
                "csv" => "text/csv",
                "cur" => "application/octet-stream",
                "cxx" => "text/plain",
                "dat" => "application/octet-stream",
                "datasource" => "application/xml",
                "dbproj" => "text/plain",
                "dcr" => "application/x-director",
                "def" => "text/plain",
                "deploy" => "application/octet-stream",
                "der" => "application/x-x509-ca-cert",
                "dgml" => "application/xml",
                "dib" => "image/bmp",
                "dif" => "video/x-dv",
                "dir" => "application/x-director",
                "disco" => "text/xml",
                "dll" => "application/x-msdownload",
                "dll.config" => "text/xml",
                "dlm" => "text/dlm",
                "doc" => "application/msword",
                "docm" => "application/vnd.ms-word.document.macroenabled.12",
                "docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                "dot" => "application/msword",
                "dotm" => "application/vnd.ms-word.template.macroenabled.12",
                "dotx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.template",
                "dsp" => "application/octet-stream",
                "dsw" => "text/plain",
                "dtd" => "text/xml",
                "dtsconfig" => "text/xml",
                "dv" => "video/x-dv",
                "dvi" => "application/x-dvi",
                "dwf" => "drawing/x-dwf",
                "dwp" => "application/octet-stream",
                "dxr" => "application/x-director",
                "eml" => "message/rfc822",
                "emz" => "application/octet-stream",
                "eot" => "application/octet-stream",
                "eps" => "application/postscript",
                "etl" => "application/etl",
                "etx" => "text/x-setext",
                "evy" => "application/envoy",
                "exe" => "application/octet-stream",
                "exe.config" => "text/xml",
                "fdf" => "application/vnd.fdf",
                "fif" => "application/fractals",
                "filters" => "application/xml",
                "fla" => "application/octet-stream",
                "flr" => "x-world/x-vrml",
                "flv" => "video/x-flv",
                "fsscript" => "application/fsharp-script",
                "fsx" => "application/fsharp-script",
                "generictest" => "application/xml",
                "gif" => "image/gif",
                "group" => "text/x-ms-group",
                "gsm" => "audio/x-gsm",
                "gtar" => "application/x-gtar",
                "gz" => "application/x-gzip",
                "h" => "text/plain",
                "hdf" => "application/x-hdf",
                "hdml" => "text/x-hdml",
                "hhc" => "application/x-oleobject",
                "hhk" => "application/octet-stream",
                "hhp" => "application/octet-stream",
                "hlp" => "application/winhlp",
                "hpp" => "text/plain",
                "hqx" => "application/mac-binhex40",
                "hta" => "application/hta",
                "htc" => "text/x-component",
                "htm" => "text/html",
                "html" => "text/html",
                "htt" => "text/webviewhtml",
                "hxa" => "application/xml",
                "hxc" => "application/xml",
                "hxd" => "application/octet-stream",
                "hxe" => "application/xml",
                "hxf" => "application/xml",
                "hxh" => "application/octet-stream",
                "hxi" => "application/octet-stream",
                "hxk" => "application/xml",
                "hxq" => "application/octet-stream",
                "hxr" => "application/octet-stream",
                "hxs" => "application/octet-stream",
                "hxt" => "text/html",
                "hxv" => "application/xml",
                "hxw" => "application/octet-stream",
                "hxx" => "text/plain",
                "i" => "text/plain",
                "ico" => "image/x-icon",
                "ics" => "application/octet-stream",
                "idl" => "text/plain",
                "ief" => "image/ief",
                "iii" => "application/x-iphone",
                "inc" => "text/plain",
                "inf" => "application/octet-stream",
                "inl" => "text/plain",
                "ins" => "application/x-internet-signup",
                "ipa" => "application/x-itunes-ipa",
                "ipg" => "application/x-itunes-ipg",
                "ipproj" => "text/plain",
                "ipsw" => "application/x-itunes-ipsw",
                "iqy" => "text/x-ms-iqy",
                "isp" => "application/x-internet-signup",
                "ite" => "application/x-itunes-ite",
                "itlp" => "application/x-itunes-itlp",
                "itms" => "application/x-itunes-itms",
                "itpc" => "application/x-itunes-itpc",
                "ivf" => "video/x-ivf",
                "jar" => "application/java-archive",
                "java" => "application/octet-stream",
                "jck" => "application/liquidmotion",
                "jcz" => "application/liquidmotion",
                "jfif" => "image/pjpeg",
                "jnlp" => "application/x-java-jnlp-file",
                "jpb" => "application/octet-stream",
                "jpe" => "image/jpeg",
                "jpeg" => "image/jpeg",
                "jpg" => "image/jpeg",
                "js" => "application/x-javascript",
                "jsx" => "text/jscript",
                "jsxbin" => "text/plain",
                "latex" => "application/x-latex",
                "library-ms" => "application/windows-library+xml",
                "lit" => "application/x-ms-reader",
                "loadtest" => "application/xml",
                "lpk" => "application/octet-stream",
                "lsf" => "video/x-la-asf",
                "lst" => "text/plain",
                "lsx" => "video/x-la-asf",
                "lzh" => "application/octet-stream",
                "m13" => "application/x-msmediaview",
                "m14" => "application/x-msmediaview",
                "m1v" => "video/mpeg",
                "m2t" => "video/vnd.dlna.mpeg-tts",
                "m2ts" => "video/vnd.dlna.mpeg-tts",
                "m2v" => "video/mpeg",
                "m3u" => "audio/x-mpegurl",
                "m3u8" => "audio/x-mpegurl",
                "m4a" => "audio/m4a",
                "m4b" => "audio/m4b",
                "m4p" => "audio/m4p",
                "m4r" => "audio/x-m4r",
                "m4v" => "video/x-m4v",
                "mac" => "image/x-macpaint",
                "mak" => "text/plain",
                "man" => "application/x-troff-man",
                "manifest" => "application/x-ms-manifest",
                "map" => "text/plain",
                "master" => "application/xml",
                "mda" => "application/msaccess",
                "mdb" => "application/x-msaccess",
                "mde" => "application/msaccess",
                "mdp" => "application/octet-stream",
                "me" => "application/x-troff-me",
                "mfp" => "application/x-shockwave-flash",
                "mht" => "message/rfc822",
                "mhtml" => "message/rfc822",
                "mid" => "audio/mid",
                "midi" => "audio/mid",
                "mix" => "application/octet-stream",
                "mk" => "text/plain",
                "mmf" => "application/x-smaf",
                "mno" => "text/xml",
                "mny" => "application/x-msmoney",
                "mod" => "video/mpeg",
                "mov" => "video/quicktime",
                "movie" => "video/x-sgi-movie",
                "mp2" => "video/mpeg",
                "mp2v" => "video/mpeg",
                "mp3" => "audio/mpeg",
                "mp4" => "video/mp4",
                "mp4v" => "video/mp4",
                "mpa" => "video/mpeg",
                "mpe" => "video/mpeg",
                "mpeg" => "video/mpeg",
                "mpf" => "application/vnd.ms-mediapackage",
                "mpg" => "video/mpeg",
                "mpp" => "application/vnd.ms-project",
                "mpv2" => "video/mpeg",
                "mqv" => "video/quicktime",
                "ms" => "application/x-troff-ms",
                "msi" => "application/octet-stream",
                "mso" => "application/octet-stream",
                "mts" => "video/vnd.dlna.mpeg-tts",
                "mtx" => "application/xml",
                "mvb" => "application/x-msmediaview",
                "mvc" => "application/x-miva-compiled",
                "mxp" => "application/x-mmxp",
                "nc" => "application/x-netcdf",
                "nsc" => "video/x-ms-asf",
                "nws" => "message/rfc822",
                "ocx" => "application/octet-stream",
                "oda" => "application/oda",
                "odc" => "text/x-ms-odc",
                "odh" => "text/plain",
                "odl" => "text/plain",
                "odp" => "application/vnd.oasis.opendocument.presentation",
                "ods" => "application/oleobject",
                "odt" => "application/vnd.oasis.opendocument.text",
                "one" => "application/onenote",
                "onea" => "application/onenote",
                "onepkg" => "application/onenote",
                "onetmp" => "application/onenote",
                "onetoc" => "application/onenote",
                "onetoc2" => "application/onenote",
                "orderedtest" => "application/xml",
                "osdx" => "application/opensearchdescription+xml",
                "p10" => "application/pkcs10",
                "p12" => "application/x-pkcs12",
                "p7b" => "application/x-pkcs7-certificates",
                "p7c" => "application/pkcs7-mime",
                "p7m" => "application/pkcs7-mime",
                "p7r" => "application/x-pkcs7-certreqresp",
                "p7s" => "application/pkcs7-signature",
                "pbm" => "image/x-portable-bitmap",
                "pcast" => "application/x-podcast",
                "pct" => "image/pict",
                "pcx" => "application/octet-stream",
                "pcz" => "application/octet-stream",
                "pdf" => "application/pdf",
                "pfb" => "application/octet-stream",
                "pfm" => "application/octet-stream",
                "pfx" => "application/x-pkcs12",
                "pgm" => "image/x-portable-graymap",
                "pic" => "image/pict",
                "pict" => "image/pict",
                "pkgdef" => "text/plain",
                "pkgundef" => "text/plain",
                "pko" => "application/vnd.ms-pki.pko",
                "pls" => "audio/scpls",
                "pma" => "application/x-perfmon",
                "pmc" => "application/x-perfmon",
                "pml" => "application/x-perfmon",
                "pmr" => "application/x-perfmon",
                "pmw" => "application/x-perfmon",
                "png" => "image/png",
                "pnm" => "image/x-portable-anymap",
                "pnt" => "image/x-macpaint",
                "pntg" => "image/x-macpaint",
                "pnz" => "image/png",
                "pot" => "application/vnd.ms-powerpoint",
                "potm" => "application/vnd.ms-powerpoint.template.macroenabled.12",
                "potx" => "application/vnd.openxmlformats-officedocument.presentationml.template",
                "ppa" => "application/vnd.ms-powerpoint",
                "ppam" => "application/vnd.ms-powerpoint.addin.macroenabled.12",
                "ppm" => "image/x-portable-pixmap",
                "pps" => "application/vnd.ms-powerpoint",
                "ppsm" => "application/vnd.ms-powerpoint.slideshow.macroenabled.12",
                "ppsx" => "application/vnd.openxmlformats-officedocument.presentationml.slideshow",
                "ppt" => "application/vnd.ms-powerpoint",
                "pptm" => "application/vnd.ms-powerpoint.presentation.macroenabled.12",
                "pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
                "prf" => "application/pics-rules",
                "prm" => "application/octet-stream",
                "prx" => "application/octet-stream",
                "ps" => "application/postscript",
                "psc1" => "application/powershell",
                "psd" => "application/octet-stream",
                "psess" => "application/xml",
                "psm" => "application/octet-stream",
                "psp" => "application/octet-stream",
                "pub" => "application/x-mspublisher",
                "pwz" => "application/vnd.ms-powerpoint",
                "qht" => "text/x-html-insertion",
                "qhtm" => "text/x-html-insertion",
                "qt" => "video/quicktime",
                "qti" => "image/x-quicktime",
                "qtif" => "image/x-quicktime",
                "qtl" => "application/x-quicktimeplayer",
                "qxd" => "application/octet-stream",
                "ra" => "audio/x-pn-realaudio",
                "ram" => "audio/x-pn-realaudio",
                "rar" => "application/octet-stream",
                "ras" => "image/x-cmu-raster",
                "rat" => "application/rat-file",
                "rc" => "text/plain",
                "rc2" => "text/plain",
                "rct" => "text/plain",
                "rdlc" => "application/xml",
                "resx" => "application/xml",
                "rf" => "image/vnd.rn-realflash",
                "rgb" => "image/x-rgb",
                "rgs" => "text/plain",
                "rm" => "application/vnd.rn-realmedia",
                "rmi" => "audio/mid",
                "rmp" => "application/vnd.rn-rn_music_package",
                "roff" => "application/x-troff",
                "rpm" => "audio/x-pn-realaudio-plugin",
                "rqy" => "text/x-ms-rqy",
                "rtf" => "application/rtf",
                "rtx" => "text/richtext",
                "ruleset" => "application/xml",
                "s" => "text/plain",
                "safariextz" => "application/x-safari-safariextz",
                "scd" => "application/x-msschedule",
                "sct" => "text/scriptlet",
                "sd2" => "audio/x-sd2",
                "sdp" => "application/sdp",
                "sea" => "application/octet-stream",
                "searchconnector-ms" => "application/windows-search-connector+xml",
                "setpay" => "application/set-payment-initiation",
                "setreg" => "application/set-registration-initiation",
                "settings" => "application/xml",
                "sgimb" => "application/x-sgimb",
                "sgml" => "text/sgml",
                "sh" => "application/x-sh",
                "shar" => "application/x-shar",
                "shtml" => "text/html",
                "sit" => "application/x-stuffit",
                "sitemap" => "application/xml",
                "skin" => "application/xml",
                "sldm" => "application/vnd.ms-powerpoint.slide.macroenabled.12",
                "sldx" => "application/vnd.openxmlformats-officedocument.presentationml.slide",
                "slk" => "application/vnd.ms-excel",
                "sln" => "text/plain",
                "slupkg-ms" => "application/x-ms-license",
                "smd" => "audio/x-smd",
                "smi" => "application/octet-stream",
                "smx" => "audio/x-smd",
                "smz" => "audio/x-smd",
                "snd" => "audio/basic",
                "snippet" => "application/xml",
                "snp" => "application/octet-stream",
                "sol" => "text/plain",
                "sor" => "text/plain",
                "spc" => "application/x-pkcs7-certificates",
                "spl" => "application/futuresplash",
                "src" => "application/x-wais-source",
                "srf" => "text/plain",
                "ssisdeploymentmanifest" => "text/xml",
                "ssm" => "application/streamingmedia",
                "sst" => "application/vnd.ms-pki.certstore",
                "stl" => "application/vnd.ms-pki.stl",
                "sv4cpio" => "application/x-sv4cpio",
                "sv4crc" => "application/x-sv4crc",
                "svc" => "application/xml",
                "swf" => "application/x-shockwave-flash",
                "t" => "application/x-troff",
                "tar" => "application/x-tar",
                "tcl" => "application/x-tcl",
                "testrunconfig" => "application/xml",
                "testsettings" => "application/xml",
                "tex" => "application/x-tex",
                "texi" => "application/x-texinfo",
                "texinfo" => "application/x-texinfo",
                "tgz" => "application/x-compressed",
                "thmx" => "application/vnd.ms-officetheme",
                "thn" => "application/octet-stream",
                "tif" => "image/tiff",
                "tiff" => "image/tiff",
                "tlh" => "text/plain",
                "tli" => "text/plain",
                "toc" => "application/octet-stream",
                "tr" => "application/x-troff",
                "trm" => "application/x-msterminal",
                "trx" => "application/xml",
                "ts" => "video/vnd.dlna.mpeg-tts",
                "tsv" => "text/tab-separated-values",
                "ttf" => "application/octet-stream",
                "tts" => "video/vnd.dlna.mpeg-tts",
                "txt" => "text/plain",
                "u32" => "application/octet-stream",
                "uls" => "text/iuls",
                "user" => "text/plain",
                "ustar" => "application/x-ustar",
                "vb" => "text/plain",
                "vbdproj" => "text/plain",
                "vbk" => "video/mpeg",
                "vbproj" => "text/plain",
                "vbs" => "text/vbscript",
                "vcf" => "text/x-vcard",
                "vcproj" => "application/xml",
                "vcs" => "text/plain",
                "vcxproj" => "application/xml",
                "vddproj" => "text/plain",
                "vdp" => "text/plain",
                "vdproj" => "text/plain",
                "vdx" => "application/vnd.ms-visio.viewer",
                "vml" => "text/xml",
                "vscontent" => "application/xml",
                "vsct" => "text/xml",
                "vsd" => "application/vnd.visio",
                "vsi" => "application/ms-vsi",
                "vsix" => "application/vsix",
                "vsixlangpack" => "text/xml",
                "vsixmanifest" => "text/xml",
                "vsmdi" => "application/xml",
                "vspscc" => "text/plain",
                "vss" => "application/vnd.visio",
                "vsscc" => "text/plain",
                "vssettings" => "text/xml",
                "vssscc" => "text/plain",
                "vst" => "application/vnd.visio",
                "vstemplate" => "text/xml",
                "vsto" => "application/x-ms-vsto",
                "vsw" => "application/vnd.visio",
                "vsx" => "application/vnd.visio",
                "vtx" => "application/vnd.visio",
                "wav" => "audio/wav",
                "wave" => "audio/wav",
                "wax" => "audio/x-ms-wax",
                "wbk" => "application/msword",
                "wbmp" => "image/vnd.wap.wbmp",
                "wcm" => "application/vnd.ms-works",
                "wdb" => "application/vnd.ms-works",
                "wdp" => "image/vnd.ms-photo",
                "webarchive" => "application/x-safari-webarchive",
                "webtest" => "application/xml",
                "wiq" => "application/xml",
                "wiz" => "application/msword",
                "wks" => "application/vnd.ms-works",
                "wlmp" => "application/wlmoviemaker",
                "wlpginstall" => "application/x-wlpg-detect",
                "wlpginstall3" => "application/x-wlpg3-detect",
                "wm" => "video/x-ms-wm",
                "wma" => "audio/x-ms-wma",
                "wmd" => "application/x-ms-wmd",
                "wmf" => "application/x-msmetafile",
                "wml" => "text/vnd.wap.wml",
                "wmlc" => "application/vnd.wap.wmlc",
                "wmls" => "text/vnd.wap.wmlscript",
                "wmlsc" => "application/vnd.wap.wmlscriptc",
                "wmp" => "video/x-ms-wmp",
                "wmv" => "video/x-ms-wmv",
                "wmx" => "video/x-ms-wmx",
                "wmz" => "application/x-ms-wmz",
                "wpl" => "application/vnd.ms-wpl",
                "wps" => "application/vnd.ms-works",
                "wri" => "application/x-mswrite",
                "wrl" => "x-world/x-vrml",
                "wrz" => "x-world/x-vrml",
                "wsc" => "text/scriptlet",
                "wsdl" => "text/xml",
                "wvx" => "video/x-ms-wvx",
                "x" => "application/directx",
                "xaf" => "x-world/x-vrml",
                "xaml" => "application/xaml+xml",
                "xap" => "application/x-silverlight-app",
                "xbap" => "application/x-ms-xbap",
                "xbm" => "image/x-xbitmap",
                "xdr" => "text/plain",
                "xht" => "application/xhtml+xml",
                "xhtml" => "application/xhtml+xml",
                "xla" => "application/vnd.ms-excel",
                "xlam" => "application/vnd.ms-excel.addin.macroenabled.12",
                "xlc" => "application/vnd.ms-excel",
                "xld" => "application/vnd.ms-excel",
                "xlk" => "application/vnd.ms-excel",
                "xll" => "application/vnd.ms-excel",
                "xlm" => "application/vnd.ms-excel",
                "xls" => "application/vnd.ms-excel",
                "xlsb" => "application/vnd.ms-excel.sheet.binary.macroenabled.12",
                "xlsm" => "application/vnd.ms-excel.sheet.macroenabled.12",
                "xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "xlt" => "application/vnd.ms-excel",
                "xltm" => "application/vnd.ms-excel.template.macroenabled.12",
                "xltx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.template",
                "xlw" => "application/vnd.ms-excel",
                "xml" => "text/xml",
                "xmta" => "application/xml",
                "xof" => "x-world/x-vrml",
                "xoml" => "text/plain",
                "xpm" => "image/x-xpixmap",
                "xps" => "application/vnd.ms-xpsdocument",
                "xrm-ms" => "text/xml",
                "xsc" => "application/xml",
                "xsd" => "text/xml",
                "xsf" => "text/xml",
                "xsl" => "text/xml",
                "xslt" => "text/xml",
                "xsn" => "application/octet-stream",
                "xss" => "application/xml",
                "xtp" => "application/octet-stream",
                "xwd" => "image/x-xwindowdump",
                "z" => "application/x-compress",
                "zip" => "application/x-zip-compressed",
                _ => "application/octet-stream",
            };
        }
    }

}
