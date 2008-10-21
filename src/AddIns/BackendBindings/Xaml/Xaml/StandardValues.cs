using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Reflection;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using System.Windows.Documents;
using System.ComponentModel;

namespace ICSharpCode.Xaml
{
	public static class StandardValues
	{
		static Dictionary<Type, List<StandardValue>> standardValues = new Dictionary<Type, List<StandardValue>>();
		static Dictionary<object, StandardValue> standardValueFromInstance = new Dictionary<object, StandardValue>();

		public static void AddStandardValues(Type type, Type valuesContainer)
		{
			AddStandardValues(type, valuesContainer
				.GetProperties(BindingFlags.Public | BindingFlags.Static)
				.Select(p => new StandardValue() {
					Instance = p.GetValue(null, null),
					Text = p.Name
				}));
		}

		public static void AddStandardValue(Type type, string text, object value)
		{
			AddStandardValues(type, new[] { new StandardValue() { Text = text, Instance = value } });
		}

		public static void AddStandardValues(Type type, IEnumerable<StandardValue> values)
		{
			List<StandardValue> list;
			lock (standardValues) {
				lock (standardValueFromInstance) {
					if (!standardValues.TryGetValue(type, out list)) {
						list = new List<StandardValue>();
						standardValues[type] = list;
					}
					foreach (var v in values) {
						list.Add(v);
						standardValueFromInstance[v.Instance] = v;
					}
				}
			}
		}

		public static IEnumerable<StandardValue> GetStandardValues(Type type)
		{
			if (type.IsEnum) {
				var enumValues = Enum.GetValues(type);
				var enumNames = Enum.GetNames(type);

				for (int i = 0; i < enumValues.Length; i++) {
					yield return new StandardValue() {
						Instance = enumValues.GetValue(i),
						Text = enumNames[i],
					};
				}
			}
			List<StandardValue> values;
			lock (standardValues) {
				if (standardValues.TryGetValue(type, out values)) {
					foreach (var value in values) {
						if (value.Text == null) {
							if (Runtime.TryConvertToText(null, value.Instance, out value.Text)) {
								yield return value;
							}
						}
						else {
							yield return value;
						}
					}
				}
			}
			var converter = TypeDescriptor.GetConverter(type);
			if (converter.GetStandardValuesSupported()) {
				foreach (var value in converter.GetStandardValues()) {
					string text;
					if (Runtime.TryConvertToText(null, value, out text)) {
						yield return new StandardValue() {
							Instance = value,
							Text = text
						};
					}
				}
			}
			yield break;
		}

		public static string GetStandardValueText(object value)
		{
			lock (standardValueFromInstance) {
				StandardValue standardValue;
				if (standardValueFromInstance.TryGetValue(value, out standardValue)) {
					return standardValue.Text;
				}
			}
			return null;
		}

		//static Dictionary<Type, Type> designTimeTypes = new Dictionary<Type, Type>();

		//public static void AddDesignTimeType(Type type, Type designTimeType)
		//{
		//    lock (designTimeTypes)
		//    {
		//        designTimeTypes[type] = designTimeType;
		//    }
		//}

		//public static Type GetDesignTimeType(Type type)
		//{
		//    lock (designTimeTypes)
		//    {
		//        Type result;
		//        if (designTimeTypes.TryGetValue(type, out result))
		//        {
		//            return result;
		//        }
		//    }
		//    return type;
		//}
	}

	public class StandardValue
	{
		public object Instance;
		public string Text;
	}
}
