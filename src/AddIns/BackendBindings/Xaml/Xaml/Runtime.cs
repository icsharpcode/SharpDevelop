using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.ComponentModel;
using System.Windows.Markup;

namespace ICSharpCode.Xaml
{
	public static class Runtime
	{
		public static object UnsetValue = new object();

		static Dictionary<Type, Type> typeReplacers = new Dictionary<Type, Type>();

		public static void AddTypeReplacer(Type type, Type replacer)
		{
			lock (typeReplacers) {
				typeReplacers[type] = replacer;
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

		public static object CreateInstance(XamlType type, object[] args)
		{
			if (type.SystemType != null) {
				var finalType = GetTypeReplacer(type.SystemType) ?? type.SystemType;
				return Activator.CreateInstance(finalType, args);
			}
			return null;
		}

		public static object GetValue(MemberNode memberNode)
		{
			return GetValue(memberNode.ParentObject.Instance, memberNode.Member);
		}

		public static void SetValue(MemberNode memberNode, object value)
		{
			SetValue(memberNode.ParentObject.Instance, memberNode.Member, value);
		}

		public static void ResetValue(MemberNode memberNode)
		{
			ResetValue(memberNode.ParentObject.Instance, memberNode.Member);
		}

		public static object GetValue(object instance, XamlMember member)
		{
			var info = GetFinalMemberInfo(instance, member);
			if (info != null) {
				return info.GetValue(instance);
			}
			return Runtime.UnsetValue;
		}

		public static void SetValue(object instance, XamlMember member, object value)
		{
			var info = GetFinalMemberInfo(instance, member);
			if (info != null) {
				info.SetValue(instance, value);
			}
		}

		public static void ResetValue(object instance, XamlMember member)
		{
			var info = GetFinalMemberInfo(instance, member);
			if (info != null) {
				info.ResetValue(instance);
			}
		}

		static ReflectionMemberInfo GetFinalMemberInfo(object instance, XamlMember member)
		{
			var result = member as ReflectionMember;
			if (result != null) {
				if (!result.IsAttachable) {
					var designTimeType = instance.GetType();
					var ownerType = member.OwnerType.SystemType;

					if (!ownerType.IsAssignableFrom(designTimeType)) {
						result = ReflectionMapper.GetXamlType(designTimeType)
							.Member(member.Name) as ReflectionMember;
					}
				}
				return result.Info;
			}
			return null;
		}

		public static void Add(object collection, object item)
		{
			IList list = collection as IList;
			list.Add(item);
		}

		public static void Add(object collection, object key, object item)
		{
			IDictionary dict = collection as IDictionary;
			dict.Add(key, item);
		}

		public static void Insert(object collection, int index, object item)
		{
			IList list = collection as IList;
			list.Insert(index, item);
		}

		public static void Remove(object collection, object item)
		{
			IList list = collection as IList;
			if (list != null) {
				list.Remove(item);
			}
			else {
				IDictionary dict = collection as IDictionary;
				object key = null;
				foreach (DictionaryEntry pair in dict) {
					if (pair.Value == item) {
						key = pair.Key;
						break;
					}
				}
				dict.Remove(key);
			}
		}

		public static bool TryConvertToText(XamlContext context, object value, out string text)
		{
			text = null;
			if (value != null) {
				text = ConvertToText(context, value);
			}
			return text != null;
		}

		public static string ConvertToText(XamlContext context, object value)
		{
			var text = StandardValues.GetStandardValueText(value);
			if (text != null) {
				return text;
			}
			var targetType = context.XamlProperty.ValueType.SystemType;
			if (targetType != null) {
				TryConvert(ref value, targetType);
			}
			var valueSerializer = GetValueSerializer(context);
			if (valueSerializer != null && valueSerializer.CanConvertToString(value, context)) {
				return valueSerializer.ConvertToString(value, context);
			}
			return null;
		}

		static void TryConvert(ref object value, Type targetType)
		{
			if (!targetType.IsAssignableFrom(value.GetType())) {
				if (value is IConvertible && typeof(IConvertible).IsAssignableFrom(targetType)) {
					value = Convert.ChangeType(value, targetType);
				}
			}
		}

		public static object ConvertFromText(XamlContext context, string text)
		{
			var valueSerializer = GetValueSerializer(context);
			if (valueSerializer != null && valueSerializer.CanConvertFromString(text, context)) {
				return valueSerializer.ConvertFromString(text, context);
			}
			return text;
		}

		public static object ConvertFromText(XamlContext context, XamlType targetType, string text)
		{
			var valueSerializer = GetValueSerializer(targetType);
			if (valueSerializer != null && valueSerializer.CanConvertFromString(text, context)) {
				return valueSerializer.ConvertFromString(text, context);
			}
			return text;
		}

		public static ValueSerializer GetValueSerializer(XamlContext context)
		{
			var valueSerializer = GetValueSerializer(context.XamlProperty.Member);
			if (valueSerializer == null) {
				valueSerializer = GetValueSerializer(context.XamlProperty.ValueType);
			}
			return valueSerializer;
		}

		public static ValueSerializer GetValueSerializer(XamlType type)
		{
			if (type.SystemType != null) {
				return ValueSerializer.GetSerializerFor(type.SystemType);
			}
			return null;
		}

		public static ValueSerializer GetValueSerializer(XamlMember member)
		{
			var reflectionMember = member as ReflectionMember;
			if (reflectionMember != null && reflectionMember.Info.PropertyDescriptor != null) {
				return ValueSerializer.GetSerializerFor(reflectionMember.Info.PropertyDescriptor);
			}
			return null;
		}

		public static XamlType GetWrapperTypeForInitializationText(object value)
		{
			var type = value.GetType();
			Type prev = null;

			while (true) {
				if (type == null || ValueSerializer.GetSerializerFor(type) == null) {
					break;
				}
				prev = type;
				type = type.BaseType;
			}

			if (prev != null) {
				return ReflectionMapper.GetXamlType(prev);
			}

			throw new XamlException("ValueSerializer not found");
		}
	}
}
