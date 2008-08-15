using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Windows;
using System.ComponentModel;

namespace ICSharpCode.WpfDesign
{
	public static class Metadata
	{
		public static string GetFullName(this DependencyProperty p)
		{
			return p.OwnerType.FullName + "." + p.Name;
		}

		static Dictionary<Type, List<object>> standardValues = new Dictionary<Type, List<object>>();

		public static void AddStandardValues(Type type, Type valuesContainer)
		{
			AddStandardValues(type, valuesContainer
				.GetProperties(BindingFlags.Public | BindingFlags.Static)
				.Select(p => p.GetValue(null, null)));
		}

		public static void AddStandardValues<T>(Type type, IEnumerable<T> values)
		{			
			List<object> list;
			lock (standardValues) {
				if (!standardValues.TryGetValue(type, out list)) {
					list = new List<object>();
					standardValues[type] = list;
				}
				foreach (var v in values) {
					list.Add(v);
				}
			}
		}

		public static IEnumerable GetStandardValues(Type type)
		{
			if (type.IsEnum) {
				return Enum.GetValues(type);
			}
			List<object> values;
			lock (standardValues) {
				if (standardValues.TryGetValue(type, out values)) {
					return values;
				}
			}
			return null;
		}

		static Dictionary<string, string> categories = new Dictionary<string, string>();

		public static void AddCategory(DependencyProperty p, string category)
		{
			lock (categories) {
				categories[p.GetFullName()] = category;
			}
		}

		public static void AddCategory(Type type, string property, string category)
		{
			lock (categories) {
				categories[type + "." + property] = category;
			}
		}

		public static string GetCategory(DesignItemProperty p)
		{
			string result;
			lock (categories) {
				if (categories.TryGetValue(p.DependencyFullName, out result)) {
					return result;
				}
			}
			return p.Category;
		}

		static HashSet<string> advancedProperties = new HashSet<string>();

		public static void AddAdvancedProperty(DependencyProperty p)
		{
			lock (advancedProperties) {
				advancedProperties.Add(p.GetFullName());
			}
		}

		public static void AddAdvancedProperty(Type type, string member)
		{
			lock (advancedProperties) {
				advancedProperties.Add(type.FullName + "." + member);
			}
		}

		public static bool IsAdvanced(DesignItemProperty p)
		{
			lock (advancedProperties) {
				if (advancedProperties.Contains(p.DependencyFullName)) {
					return true;
				}
			}
			return p.IsAdvanced;
		}

		public static string[] CategoryOrder { get; set; }

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
	}

	public class NumberRange
	{
		public double Min;
		public double Max;
	}
}
