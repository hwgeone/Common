using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public static class ObjectHelper
    {
        /// <summary>
        /// 数组去重
        /// </summary>
        /// <param name="myData"></param>
        /// <returns></returns>
        public static String[] RemoveDup(String[] myData)
        {
            return myData.Distinct().ToArray();
        }

        public static string IsHasSameElements(String[] fData, String[] sData)
        {
            string res = "";
            HashSet<String> same = new HashSet<String>();  //用来存放两个数组中相同的元素  
            HashSet<String> temp = new HashSet<String>();  //用来存放数组a中的元素  

            for (int i = 0; i < fData.Count(); i++)
            {
                temp.Add(fData[i]);   //把数组a中的元素放到Set中，可以去除重复的元素  
            }

            for (int j = 0; j < sData.Count(); j++)
            {
                //把数组b中的元素添加到temp中  
                //如果temp中已存在相同的元素，则temp.add（b[j]）返回false  
                if (!temp.Add(sData[j]))
                {
                    res = sData[j];
                    break;
                }
            }
            return res;
        }

        /// <summary>
        /// 返回枚举项的描述信息。
        /// </summary>
        /// <param name="value">要获取描述信息的枚举项。</param>
        /// <returns>枚举想的描述信息。</returns>
        public static string GetDescription(int relation,Type type)
        {

            //value : enum value
            foreach (int item in Enum.GetValues(type))
            {
                if (item == relation)
                {
                    //Type enumType = item.GetType();
                    // 获取枚举常数名称。
                    string name = Enum.GetName(type, item);
                    if (name != null)
                    {
                        // 获取枚举字段。
                        FieldInfo fieldInfo = type.GetField(name);
                        if (fieldInfo != null)
                        {
                            // 获取描述的属性。
                            DescriptionAttribute attr = Attribute.GetCustomAttribute(fieldInfo,
                                typeof(DescriptionAttribute), false) as DescriptionAttribute;
                            if (attr != null)
                            {
                                return attr.Description;
                            }
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Perform a deep Copy of the object, using Json as a serialisation method. NOTE: Private members are not cloned using this method.
        /// </summary>
        /// <typeparam name="T">The type of object being copied.</typeparam>
        /// <param name="source">The object instance to copy.</param>
        /// <returns>The copied object.</returns>
        public static T CloneJson<T>(this T source)
        {
            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            // initialize inner objects individually
            // for example in default constructor some list property initialized with some values,
            // but in 'source' these items are cleaned -
            // without ObjectCreationHandling.Replace default constructor values will be added to result
            var deserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };

            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source), deserializeSettings);
        }

        public static T CopyTo<T>(this object source) where T : class, new()
        {
            var result = new T();
            source.CopyTo(result);
            return result;
        }

        public static void CopyTo<T>(this object source, T target)
            where T : class,new()
        {
            if (source == null)
                return;

            if (target == null)
            {
                target = new T();
            }

            foreach (var property in target.GetType().GetProperties())
            {
                var propertyValue = source.GetType().GetProperty(property.Name).GetValue(source, null);
                if (propertyValue != null)
                {
                    if (propertyValue.GetType().IsClass)
                    {

                    }
                    target.GetType().InvokeMember(property.Name, BindingFlags.SetProperty, null, target, new object[] { propertyValue });
                }

            }

            foreach (var field in target.GetType().GetFields())
            {
                var fieldValue = source.GetType().GetField(field.Name).GetValue(source);
                if (fieldValue != null)
                {
                    target.GetType().InvokeMember(field.Name, BindingFlags.SetField, null, target, new object[] { fieldValue });
                }
            }
        }
    }
}
