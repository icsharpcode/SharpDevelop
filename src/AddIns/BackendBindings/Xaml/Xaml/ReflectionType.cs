using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Reflection;
using System.Collections;
using System.ComponentModel;
using System.Windows;

namespace ICSharpCode.Xaml
{
	public class ReflectionType : XamlType
	{
		internal ReflectionType(Type type)
		{
			this.type = type;
			this.propertyDescriptors = TypeDescriptor.GetProperties(type);
		}

		Type type;
		PropertyDescriptorCollection propertyDescriptors;

		public Type Type
		{
			get { return type; }
		}

		public override string Name
		{
			get { return type.Name; }
		}

		public override bool IsDefaultConstructible
		{
			get { return type.GetConstructor(Type.EmptyTypes) != null; }
		}

		public override bool IsNullable
		{
			get { return !type.IsValueType; }
		}

		public override IEnumerable<XamlMember> Members
		{
			get { throw new NotImplementedException(); }
		}

		public override XamlMember ContentProperty
		{
			get
			{
				foreach (ContentPropertyAttribute a in type.GetCustomAttributes(typeof(ContentPropertyAttribute), true)) {
					return Member(a.Name);
				}
				return null;
			}
		}

		public override XamlMember DictionaryKeyProperty
		{
			get { throw new NotImplementedException(); }
		}

		public override XamlMember NameProperty
		{
			get
			{
				foreach (RuntimeNamePropertyAttribute a in type.GetCustomAttributes(typeof(RuntimeNamePropertyAttribute), true)) {
					return Member(a.Name);
				}
				return null;
			}
		}

		public override XamlMember XmlLangProperty
		{
			get { throw new NotImplementedException(); }
		}

		public override bool TrimSurroundingWhitespace
		{
			get
			{
				foreach (var a in type.GetCustomAttributes(typeof(TrimSurroundingWhitespaceAttribute), true)) {
					return true;
				}
				return false;
			}
		}

		public override bool IsWhitespaceSignificantCollection
		{
			get
			{
				foreach (var a in type.GetCustomAttributes(typeof(WhitespaceSignificantCollectionAttribute), true)) {
					return true;
				}
				return false;
			}
		}

		public override bool IsCollection
		{
			get { return typeof(ICollection).IsAssignableFrom(type); }
		}

		public override bool IsDictionary
		{
			get { return typeof(IDictionary).IsAssignableFrom(type); }
		}

		public override IEnumerable<XamlType> AllowedTypes
		{
			get { throw new NotImplementedException(); }
		}

		public override IEnumerable<XamlType> AllowedKeyTypes
		{
			get { throw new NotImplementedException(); }
		}

		public override bool IsXData
		{
			get { throw new NotImplementedException(); }
		}

		public override bool IsNameScope
		{
			get { throw new NotImplementedException(); }
		}

		public override IEnumerable<Constructor> Constructors
		{
			get
			{
				Dictionary<int, Constructor> ctors = new Dictionary<int, Constructor>();
				foreach (var ctorInfo in type.GetConstructors()) {
					var parameters = ctorInfo.GetParameters();
					if (parameters.Length > 0 && !ctors.ContainsKey(parameters.Length)) {
						var ctor = new Constructor();
						ctor.Arguments = parameters
							.Select(p => ReflectionMapper.GetXamlType(p.ParameterType)).ToArray();

						List<XamlMember> members = new List<XamlMember>();
						foreach (var p in parameters) {
							var member = MemberCaseInsensetive(p.Name);
							if (member == null) {
								members = null;
								break;
							}
							members.Add(member);
						}
						if (members != null) {
							ctor.CorrespondingMembers = members.ToArray();
						}

						ctors[parameters.Length] = ctor;
					}
				}
				return ctors.Values;
			}
		}

		public override XamlType ReturnValueType
		{
			get { throw new NotImplementedException(); }
		}

		public override string Namespace
		{
			get { return type.Namespace; }
		}

		public override XamlAssembly Assembly
		{
			get { return ReflectionMapper.GetXamlAssembly(type.Assembly); }
		}

		public override XamlMember Member(string name)
		{
			ReflectionMemberInfo info = null;

			var pd = propertyDescriptors[name];
			if (pd != null) {
				info = new ReflectionPropertyInfo(pd);
			}
			else {
				var eventInfo = type.GetEvent(name, BindingFlags.Public | BindingFlags.Instance);
				if (eventInfo != null) {
					info = new ReflectionEventInfo(eventInfo);
				}
				else {
					// attached dp better than getter/setter cause we have DependencyObject.ClearValue(dp)
					var field = type.GetField(name + "Property", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
					if (field != null) {
						var dp = (DependencyProperty)field.GetValue(null);
						if (dp != null) {
							info = new DependencyPropertyInfo(dp);
						}
					}
					if (info == null) {
						var getter = type.GetMethod("Get" + name, BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
						var setter = type.GetMethod("Set" + name, BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
						if (setter != null) {
							info = new ReflectionAttachedPropertyInfo(getter, setter);
						}
					}
				}
			}
			if (info != null) {
				return ReflectionMapper.GetXamlMember(info);
			}
			return null;
		}

		XamlMember MemberCaseInsensetive(string name)
		{
			foreach (PropertyDescriptor pd in propertyDescriptors) {
				if (pd.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)) {
					return ReflectionMapper.GetXamlMember(new ReflectionPropertyInfo(pd));
				}
			}
			return null;
		}

		public override IEnumerable<XamlType> ContentWrappers
		{
			get
			{
				foreach (ContentWrapperAttribute a in type.GetCustomAttributes(typeof(ContentWrapperAttribute), true)) {
					yield return ReflectionMapper.GetXamlType(a.ContentWrapper);
				}
			}
		}

		public override bool IsAssignableFrom(XamlType other)
		{
			var r = other as ReflectionType;
			if (r != null) {
				return type.IsAssignableFrom(r.Type);
			}
			return false;
		}

		public override bool HasTextSyntax
		{
			get { return Runtime.GetValueSerializer(this) != null; }
		}

		public override Type SystemType
		{
			get { return type; }
		}

		public override T GetAttribute<T>()
		{
			var usageAttribute = (AttributeUsageAttribute)typeof(T).GetCustomAttributes(typeof(AttributeUsageAttribute), true)[0];
			foreach (T attr in type.GetCustomAttributes(typeof(T), usageAttribute.Inherited)) {
				return attr;
			}
			return null;
		}
	}
}
