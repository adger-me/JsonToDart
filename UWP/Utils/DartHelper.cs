﻿using FlutterCandiesJsonToDart.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FlutterCandiesJsonToDart.Utils
{
    public class DartHelper
    {
        public const String ReplaceSymbol = "#";
        public const String ClassHeader = "class {0} {{";
        public const String ClassFooter = "}";

        public const String FromJsonHeader = "  factory {0}.fromJson(Map<String, dynamic> jsonRes)=>jsonRes == null? null:{0}(";
        public const String FromJsonHeader1 = "  factory {0}.fromJson(Map<String, dynamic> jsonRes){{ if(jsonRes == null){{return null;}}\n";
        public const String FromJsonFooter = ");";
        public const String FromJsonFooter1 = "return {0}({1});}}";
        public const String ToJsonHeader = "  Map<String, dynamic> toJson() => <String, dynamic>{";
        public const String ToJsonFooter = "};";
        public const String ToJsonSetString = "        '{0}': {1},";
        public const String JsonImport = "import 'dart:convert' show json;";
        public const String DebugPrintImport = "import 'package:flutter/foundation.dart';";
        public const String PropertyString = "  {0} {1};";
        public const String PropertyStringFinal = "  final {0} {1};";
        public const String PropertyStringGet = "  {0} _{2};\n  {0} get {1} => _{2};";
        public const String PropertyStringGetSet = "  {0} _{2};\n  {0} get {1} => _{2};\n  set {1}(value)  {{\n    _{2} = value;\n  }}\n";

        public static String SetProperty(String setName, ExtendedProperty item, String className)
        {
            //if (ConfigHelper.Instance.Config.EnableDataProtection)
            //{
            //    return $"    {setName} : convertValueByType(jsonRes['{item.Key}'],{GetDartTypeString(item.Type)},stack:\"{className}-{item.Key}\"),";
            //}
            //else
            //return $"    {setName} : asT<{GetDartTypeString(item.Type)}>(jsonRes['{item.Key}']),";

            return $"{ setName } : { String.Format(UseAsT, GetDartTypeString(item.Type), $"jsonRes['{item.Key}']")},";
        }


        public const String SetObjectProperty = "    {0} : {2}.fromJson(asT<Map<String, dynamic>>(jsonRes['{1}'])),";
        public static String PropertyS(PropertyAccessorType type)
        {
            switch (type)
            {
                case PropertyAccessorType.None:
                    return PropertyString;
                case PropertyAccessorType.Final:
                    return PropertyStringFinal;
                case PropertyAccessorType.Get:
                    return PropertyStringGet;
                case PropertyAccessorType.GetSet:
                    return PropertyStringGetSet;
            }
            return "";
        }

        public static String GetSetPropertyString(ExtendedProperty property)
        {
            var name = property.Name;
            switch (property.PropertyAccessorType)
            {
                case PropertyAccessorType.None:
                case PropertyAccessorType.Final:
                    return name;
                case PropertyAccessorType.Get:
                case PropertyAccessorType.GetSet:
                    var lowName = name.Substring(0, 1).ToLower() + name.Substring(1);
                    return "_" + lowName;
            }

            return name;

        }

        public static String GetDartTypeString(DartType dartType)
        {
            switch (dartType)
            {
                case DartType.String:
                    return "String";
                case DartType.Int:
                    return "int";

                case DartType.Object:
                    return "Object";

                case DartType.Bool:
                    return "bool";

                case DartType.Double:
                    return "double";
                default:
                    return "";
            }

        }

        //public static String GetDefaultValue(DartType dartType)
        //{
        //    switch (dartType)
        //    {
        //        case DartType.String:
        //            return "''";
        //        case DartType.Int:
        //            return "0";

        //        case DartType.Object:
        //            return "null";

        //        case DartType.Bool:
        //            return "false";

        //        case DartType.Double:
        //            return "0.0";
        //        default:
        //            return "null";
        //    }
        //}

        public const String FactoryStringHeader = "    {0}({{";
        public const String FactoryStringFooter = "    });\n";

        public static String FactorySetString(PropertyAccessorType type)
        {
            switch (type)
            {
                case PropertyAccessorType.None:
                case PropertyAccessorType.Final:
                    return "this.{0},";
                case PropertyAccessorType.Get:
                case PropertyAccessorType.GetSet:
                    return "{0} {1},";

            }
            return "this.{0},";

        }

        public const String ClassToString = "  @override\nString  toString() {\n    return json.encode(this);\n  }";

        public static DartType ConverDartType(JTokenType type)
        {
            switch (type)
            {
                case JTokenType.None:
                    break;
                case JTokenType.Object:
                    return DartType.Object;
                case JTokenType.Array:
                    break;
                case JTokenType.Constructor:
                    break;
                case JTokenType.Property:
                    break;
                case JTokenType.Comment:
                    return DartType.String;
                case JTokenType.Integer:
                    return DartType.Int;
                case JTokenType.Float:
                    return DartType.Double;
                case JTokenType.String:
                    return DartType.String;
                case JTokenType.Boolean:
                    return DartType.Bool;
                case JTokenType.Null:
                    break;
                case JTokenType.Undefined:
                    break;
                case JTokenType.Date:
                    return DartType.String;
                case JTokenType.Raw:
                    break;
                case JTokenType.Bytes:
                    return DartType.String;
                case JTokenType.Guid:
                    return DartType.String;
                case JTokenType.Uri:
                    return DartType.String;
                case JTokenType.TimeSpan:
                    return DartType.String;
                default:
                    break;
            }
            return DartType.Object;
        }



        public const String ConvertMethod = @"dynamic convertValueByType(value, Type type, {String stack: """"}) {
  if (value == null) {"
    + "debugPrint('$stack : value is null');" +
        @"
    if (type == String) {
      return """";
    } else if (type == int) {
      return 0;
    } else if (type == double) {
      return 0.0;
    } else if (type == bool) {
      return false;
    }
    return null;
  }

  if (value.runtimeType == type) {
    return value;
  }
var valueS = value.toString();
" + "debugPrint('$stack : ${value.runtimeType} is not $type type');" + @"
  if (type == String) {
    return valueS;
  } else if (type == int) {
    return int.tryParse(valueS);
  } else if (type == double) {
    return double.tryParse(valueS);
  } else if (type == bool) {
    valueS = valueS.toLowerCase();
    var intValue = int.tryParse(valueS);
    if (intValue != null) {
      return intValue == 1;
    }
   " + " return valueS == \"true\";" + @"
  }
}";
        public const String TryCatchMethod = @"void tryCatch(Function f) {
  try {
    f?.call();
  } catch (e, stack) {
   " + " debugPrint('$e'); \n  debugPrint('$stack');" + @"
    }
}";


        public const String AsTMethod = @"
T asT<T>(dynamic value) {
  if (value is T) {
    return value;
  }
  return null;
}     
 ";
        public const String AsTMethodWithDataProtection = @"
T asT<T>(dynamic value) {
  if (value is T) {
    return value;
  }
 if (value != null) {
    final String valueS = value.toString();
    if (0 is T) {
      return int.tryParse(valueS) as T;
    } else if (0.0 is T) {
      return double.tryParse(valueS) as T;
    } else if ('' is T) {
      return valueS as T;
    } else if (false is T) {
      if (valueS == '0' || valueS == '1') {
        return (valueS == '1') as T;
      }
      return bool.fromEnvironment(value.toString()) as T;
    }
  }
  return null;
}     
 ";

        public const String UseAsT = "asT<{0}>({1})";

        public static async Task<String> FormatCode(String value)
        {
#if WINDOWS_UWP || WPF
            var dic = new Dictionary<String, String>();
            dic.Add("source", value);
            string str = JsonConvert.SerializeObject(dic);
            HttpContent content = new StringContent(str, Encoding.UTF8, "application/json");
            HttpClient client = new HttpClient();
            try
            {
                HttpResponseMessage response = await client.PostAsync(new Uri("https://dart-services.dartpad.cn/api/dartservices/v2/format"), content);

                if (response.IsSuccessStatusCode)
                {
                    var resultOject = JObject.Parse(await response.Content.ReadAsStringAsync());
                    return resultOject["newString"].ToString();
                }

            }
            catch (Exception)
            {
                try
                {
                    HttpResponseMessage response = await client.PostAsync(new Uri("https://dart-services.appspot.com/api/dartservices/v2/format"), content);

                    if (response.IsSuccessStatusCode)
                    {
                        var resultOject = JObject.Parse(await response.Content.ReadAsStringAsync());
                        return resultOject["newString"].ToString();
                    }
                }
                catch (Exception)
                {

          
                }
            }

            return value;
        }
#endif
    }



}
