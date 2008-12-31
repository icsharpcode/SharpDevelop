using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xaml.Schema;
using System.Windows.Controls;
using System.Xaml;
using System.Windows.Input;
using SharpDevelop.XamlDesigner.Extensibility;
using System.Windows;
using System.Windows.Markup;

namespace SharpDevelop.XamlDesigner.Dom
{
	public abstract class MemberId
	{
		public abstract string Name { get; }
		public abstract Type OwnerType { get; }
		public abstract Type ValueType { get; }
		public abstract bool IsReadOnly { get; }
		public abstract ValueSerializer ValueSerializer { get; }
		public bool IsAttachable { get; internal set; }

		public string DisplayName
		{
			get { return IsAttachable ? OwnerType.Name + "." + Name : Name; }
		}

		public string Documentation
		{
			get { return DesignEnvironment.Instance.GetDocumentation(this); }
		}

		public bool IsBrowsable
		{
			get
			{
				var a = MetadataStore.GetAttributes<BrowsableAttribute>(this).FirstOrDefault();
				return a != null ? a.Browsable : true;
			}
		}

		static Dictionary<Type, Dictionary<string, MemberId>> map =
			new Dictionary<Type, Dictionary<string, MemberId>>();

		public override string ToString()
		{
			return OwnerType.Name + "." + Name;
		}

		public static MemberId GetMember(Type type, string name)
		{
			EnsureType(type);

			foreach (var currentType in type.GetChain()) {
				MemberId result;
				if (map[currentType].TryGetValue(name, out result)) {
					return result;
				}
			}

			throw new Exception();
		}

		public static IEnumerable<MemberId> GetMembers(Type type)
		{
			EnsureType(type);

			foreach (var currentType in type.GetChain()) {
				foreach (var member in map[currentType].Values) {
					yield return member;
				}
			}
		}

		static void EnsureType(Type type)
		{
			Dictionary<string, MemberId> result;
			if (!map.TryGetValue(type, out result)) {
				result = new Dictionary<string, MemberId>();

				foreach (PropertyDescriptor d in TypeDescriptor.GetProperties(type)) {
					if (d.ComponentType == type) {
						result[d.Name] = new PropertyId(d);
					}
				}
				foreach (EventDescriptor d in TypeDescriptor.GetEvents(type)) {
					if (d.ComponentType == type) {
						result[d.Name] = new EventId(d);
					}
				}
				foreach (PropertyDescriptor d in GetAttachableProperties(type)) {
					result[d.Name] = new PropertyId(d) { IsAttachable = true };
				}

				map[type] = result;
			}
			if (type.BaseType != null) {
				EnsureType(type.BaseType);
			}
		}

		static IEnumerable<PropertyDescriptor> GetAttachableProperties(Type type)
		{
			var schemaType = XamlSchemaTypeResolver.Default.Resolve(
					XamlSchemaTypeResolver.Default.GetTypeReference(type));

			foreach (var group in schemaType.AttachableMembers) {
				foreach (var property in group.Members.OfType<SchemaProperty>()) {
					var d = XamlClrProperties.GetPropertyDescriptor(property);
					if (d != null) {
						yield return d;
					}
				}
			}
		}
	}
}
