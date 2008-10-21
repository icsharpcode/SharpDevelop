using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Windows;
using System.ComponentModel;
using System.Diagnostics;

namespace ICSharpCode.WpfDesign
{
	public static class Metadata
	{
		public static string GetFullName(this DependencyProperty p)
		{
			return p.OwnerType.FullName + "." + p.Name;
		}

		//static Dictionary<Type, List<object>> standardValues = new Dictionary<Type, List<object>>();

		//public static void AddStandardValues(Type type, Type valuesContainer)
		//{
		//    AddStandardValues(type, valuesContainer
		//        .GetProperties(BindingFlags.Public | BindingFlags.Static)
		//        .Select(p => p.GetValue(null, null)));
		//}

		//public static void AddStandardValues<T>(Type type, IEnumerable<T> values)
		//{			
		//    List<object> list;
		//    lock (standardValues) {
		//        if (!standardValues.TryGetValue(type, out list)) {
		//            list = new List<object>();
		//            standardValues[type] = list;
		//        }
		//        foreach (var v in values) {
		//            list.Add(v);
		//            if (StandardValueAdded != null) {
		//                StandardValueAdded(null, new StandardValueEventArgs() { StandardValue = v });
		//            }
		//        }
		//    }
		//}

		//public static IEnumerable GetStandardValues(Type type)
		//{
		//    if (type.IsEnum) {
		//        return Enum.GetValues(type);
		//    }
		//    List<object> values;
		//    lock (standardValues) {
		//        if (standardValues.TryGetValue(type, out values)) {
		//            return values;
		//        }
		//    }
		//    return null;
		//}

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

						if (StandardValueAdded != null) {
							StandardValueAdded(null, new StandardValueEventArgs() {
								Type = type,
								StandardValue = v
							});
						}
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
						yield return value;
					}
				}
			}
		}

		public static StandardValue GetStandardValue(object value)
		{
			lock (standardValueFromInstance) {
				StandardValue standardValue;
				if (standardValueFromInstance.TryGetValue(value, out standardValue)) {
					return standardValue;
				}
			}
			return null;
		}

		//static Dictionary<string, string> categories = new Dictionary<string, string>();

		//public static void AddCategory(DependencyProperty p, string category)
		//{
		//    lock (categories) {
		//        categories[p.GetFullName()] = category;
		//    }
		//}

		//public static void AddCategory(Type type, string property, string category)
		//{
		//    lock (categories) {
		//        categories[type + "." + property] = category;
		//    }
		//}

		//public static string GetCategory(DesignItemProperty p)
		//{
		//    string result;
		//    lock (categories) {
		//        if (categories.TryGetValue(p.DependencyFullName, out result)) {
		//            return result;
		//        }
		//    }
		//    return p.Category;
		//}

		//static HashSet<string> advancedProperties = new HashSet<string>();

		//public static void AddAdvancedProperty(DependencyProperty p)
		//{
		//    lock (advancedProperties) {
		//        advancedProperties.Add(p.GetFullName());
		//    }
		//}

		//public static void AddAdvancedProperty(Type type, string member)
		//{
		//    lock (advancedProperties) {
		//        advancedProperties.Add(type.FullName + "." + member);
		//    }
		//}

		//public static bool IsAdvanced(DesignItemProperty p)
		//{
		//    lock (advancedProperties) {
		//        if (advancedProperties.Contains(p.DependencyFullName)) {
		//            return true;
		//        }
		//    }
		//    return p.IsAdvanced;
		//}

		static HashSet<string> hiddenProperties = new HashSet<string>();

		public static void HideProperty(DependencyProperty p)
		{
			lock (hiddenProperties) {
				hiddenProperties.Add(p.GetFullName());
			}
		}

		public static void HideProperty(Type type, string member)
		{
			lock (hiddenProperties) {
				hiddenProperties.Add(type.FullName + "." + member);
			}
		}

		public static bool IsBrowsable(DesignItemProperty p)
		{
			lock (hiddenProperties) {
				if (hiddenProperties.Contains(p.DependencyFullName)) {
					return false;
				}
			}
			return true;
		}

		//public static string[] CategoryOrder { get; set; }

		static HashSet<string> popularProperties = new HashSet<string>();

		public static void AddPopularProperty(DependencyProperty p)
		{
			lock (popularProperties) {
				popularProperties.Add(p.GetFullName());
			}
		}

		public static void AddPopularProperty(Type type, string member)
		{
			lock (popularProperties) {
				popularProperties.Add(type.FullName + "." + member);
			}
		}

		public static bool IsPopularProperty(DesignItemProperty p)
		{
			lock (popularProperties) {
				if (popularProperties.Contains(p.DependencyFullName)) {
					return true;
				}
			}
			return false;
		}

		//static HashSet<Type> popularControls = new HashSet<Type>();

		//public static void AddPopularControl(Type t)
		//{
		//    lock (popularControls) {
		//        popularControls.Add(t);
		//    }
		//}

		//public static IEnumerable<Type> GetPopularControls()
		//{
		//    lock (popularControls) {
		//        foreach (var t in popularControls) {
		//            yield return t;
		//        }
		//    }
		//}

		//public static bool IsPopularControl(Type t)
		//{
		//    lock (popularControls) {
		//        return popularControls.Contains(t);
		//    }
		//}

		static Dictionary<string, NumberRange> ranges = new Dictionary<string, NumberRange>();

		public static void AddValueRange(DependencyProperty p, double min, double max)
		{
			lock (ranges) {
				ranges[p.GetFullName()] = new NumberRange() { Min = min, Max = max };
			}
		}

		public static NumberRange GetValueRange(DesignItemProperty p)
		{
			NumberRange r;
			lock (ranges) {
				if (ranges.TryGetValue(p.DependencyFullName, out r)) {
					return r;
				}
			}
			return null;
		}

		static HashSet<Type> placementDisabled = new HashSet<Type>();

		public static void DisablePlacement(Type type)
		{
			lock (placementDisabled) {
				placementDisabled.Add(type);
			}
		}

		public static bool IsPlacementDisabled(Type type)
		{
			lock (placementDisabled) {
				return placementDisabled.Contains(type);
			}
		}

		static Dictionary<Type, Size> defaultSizes = new Dictionary<Type, Size>();

		public static void AddDefaultSize(Type t, Size s)
		{
			lock (defaultSizes) {
				defaultSizes[t] = s;
			}
		}

		public static Size GetDefaultSize(Type t)
		{
			Size s;
			lock (defaultSizes) {
				while (t != null) {
					if (defaultSizes.TryGetValue(t, out s)) {
						return s;
					}
					t = t.BaseType;
				}
			}
			return new Size(double.NaN, double.NaN);
		}

		static Dictionary<Type, Type> typeReplacers = new Dictionary<Type, Type>();

		public static void AddTypeReplacer(Type type, Type replacer)
		{
			lock (typeReplacers) {
				typeReplacers[type] = replacer;
				if (TypeReplacerAdded != null) {
					TypeReplacerAdded(null, new TypeEventArgs() { Type = type });
				}
			}
		}

		public static Type GetTypeReplacer(Type type)
		{
			Type replacer;
			lock (typeReplacers) {
				typeReplacers.TryGetValue(type, out replacer);
				return replacer;
			}
		}

		static HashSet<Assembly> registeredAssemblies = new HashSet<Assembly>();

		public static IEnumerable<Assembly> RegisteredAssemblies
		{
			get { return registeredAssemblies; }
		}

		public static void RegisterAssembly(Assembly assembly)
		{
			if (!registeredAssemblies.Contains(assembly)) {

				ICSharpCode.WpfDesign.PropertyGrid.EditorManager.RegisterAssembly(assembly);

				foreach (var t in assembly.GetTypes()) {
					if (t.GetInterface("IRegisterMetadata") == typeof(IRegisterMetadata)) {
						var m = Activator.CreateInstance(t) as IRegisterMetadata;
						m.Register();
					}
				}
				foreach (var t in assembly.GetTypes()) {
					foreach (ReplacerForAttribute a in t.GetCustomAttributes(typeof(ReplacerForAttribute), false)) {
						AddTypeReplacer(a.Type, t);
					}
				}

				registeredAssemblies.Add(assembly);
			}
		}

		public static event EventHandler<StandardValueEventArgs> StandardValueAdded;
		public static event EventHandler<TypeEventArgs> TypeReplacerAdded;
	}

	public class StandardValueEventArgs : EventArgs
	{
		public Type Type;
		public StandardValue StandardValue;
	}

	public class TypeEventArgs : EventArgs
	{
		public Type Type;
	}

	public class StandardValue
	{
		public object Instance;
		public string Text;
	}

	public class NumberRange
	{
		public double Min;
		public double Max;
	}

	public class ReplacerForAttribute : Attribute
	{
		public ReplacerForAttribute(Type type)
		{
			this.Type = type;
		}

		public Type Type { get; private set; }
	}

	public interface IRegisterMetadata
	{
		void Register();
	}
}
