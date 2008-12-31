using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.ComponentModel;
using System.Reflection;
using SharpDevelop.XamlDesigner.Dom;

namespace SharpDevelop.XamlDesigner.Extensibility
{
	//NOTE: Works like Inherited = true, AllowMultiple = true
	public static class MetadataStore
	{
		static MetadataStore()
		{
			DefaultMetadata.Register();
		}

		static Dictionary<Type, TypeMetadata> store = new Dictionary<Type, TypeMetadata>();

		public static void AddAttribute(Type type, Attribute attribute)
		{
			EnsureTypeMetadata(type).Attributes.Add(attribute);
		}

		public static void AddAttribute(Type type, string memberName, Attribute attribute)
		{
			EnsureMemberMetadata(type, memberName).Attributes.Add(attribute);
		}

		public static void AddAttribute(DependencyProperty dp, Attribute attribute)
		{
			AddAttribute(dp.OwnerType, dp.Name, attribute);
		}

		public static IEnumerable<T> GetAttributes<T>(Type type) where T : Attribute
		{
			foreach (var current in GetTypeChain(type)) {
				TypeMetadata typeMetadata;
				if (store.TryGetValue(current, out typeMetadata)) {
					foreach (var attribute in typeMetadata.GetMergedAttributes().OfType<T>()) {
						yield return attribute;
					}
				}
			}
		}

		static IEnumerable<Type> GetTypeChain(Type type)
		{
			var current = type;
			while (current != null) {
				yield return current;
				current = current.BaseType;
			}
			foreach (var item in type.GetInterfaces()) {
				yield return item;
			}
		}

		public static IEnumerable<T> GetAttributes<T>(Type type, string memberName) where T : Attribute
		{
			TypeMetadata typeMetadata;
			if (store.TryGetValue(type, out typeMetadata)) {
				if (typeMetadata.Members != null) {
					MemberMetadata memberMetadata;
					if (typeMetadata.Members.TryGetValue(memberName, out memberMetadata)) {
						foreach (var attribute in memberMetadata.GetMergedAttributes().OfType<T>()) {
							yield return attribute;
						}
					}
				}				
			}
		}

		public static IEnumerable<T> GetAttributes<T>(DependencyProperty dp) where T : Attribute
		{
			return GetAttributes<T>(dp.OwnerType, dp.Name);
		}

		public static IEnumerable<T> GetAttributes<T>(DesignProperty property) where T : Attribute
		{
			return GetAttributes<T>(property.MemberId);
		}

		public static IEnumerable<T> GetAttributes<T>(MemberId member) where T : Attribute
		{
			return GetAttributes<T>(member.OwnerType, member.Name);
		}

		public static IEnumerable<T> GetAttributes<T>(MemberDescriptor descriptor) where T : Attribute
		{
			return GetAttributes<T>(descriptor.GetComponentType(), descriptor.Name);
		}

		static TypeMetadata EnsureTypeMetadata(Type type)
		{
			TypeMetadata result;
			if (!store.TryGetValue(type, out result)) {
				result = new TypeMetadata();
				result.Type = type;
				store[type] = result;
			}
			return result;
		}

		static MemberMetadata EnsureMemberMetadata(Type type, string memberName)
		{
			var typeMetadata = EnsureTypeMetadata(type);
			if (typeMetadata.Members == null) {
				typeMetadata.Members = new Dictionary<string, MemberMetadata>();
			}
			MemberMetadata result;
			if (!typeMetadata.Members.TryGetValue(memberName, out result)) {
				result = new MemberMetadata();
				result.MemberInfo = type.GetMember(memberName)[0];
				typeMetadata.Members[memberName] = result;
			}
			return result;
		}

		class TypeMetadata
		{
			public Type Type;
			public List<Attribute> Attributes = new List<Attribute>();
			public Dictionary<string, MemberMetadata> Members;

			public IEnumerable<Attribute> GetMergedAttributes()
			{
				return Attribute.GetCustomAttributes(Type).Concat(Attributes);
			}
		}

		class MemberMetadata
		{
			public MemberInfo MemberInfo;
			public List<Attribute> Attributes = new List<Attribute>();

			public IEnumerable<Attribute> GetMergedAttributes()
			{
				return Attribute.GetCustomAttributes(MemberInfo).Concat(Attributes);
			}
		}
	}
}
