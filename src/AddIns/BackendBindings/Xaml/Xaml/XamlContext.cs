using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Markup;

namespace ICSharpCode.Xaml
{
	public class XamlContext : IServiceProvider, ITypeDescriptorContext, IXamlTypeResolver, IUriContext, IProvideValueTarget, IValueSerializerContext
	{
		internal XamlContext(XamlProperty property)
		{
			this.property = property;
		}

		XamlProperty property;

		public XamlProperty XamlProperty
		{
			get { return property; }
		}

		#region IServiceProvider Members

		public object GetService(Type serviceType)
		{
			if (serviceType == typeof(ITypeDescriptorContext)) return this;
			if (serviceType == typeof(IXamlTypeResolver)) return this;
			if (serviceType == typeof(IUriContext)) return this;
			if (serviceType == typeof(IProvideValueTarget)) return this;
			if (serviceType == typeof(IValueSerializerContext)) return this;
			return null;
		}

		#endregion

		#region ITypeDescriptorContext Members

		public IContainer Container
		{
			get { throw new NotImplementedException(); }
		}

		public object Instance
		{
			get { throw new NotImplementedException(); }
		}

		public void OnComponentChanged()
		{
			throw new NotImplementedException();
		}

		public bool OnComponentChanging()
		{
			throw new NotImplementedException();
		}

		public PropertyDescriptor PropertyDescriptor
		{
			get { throw new NotImplementedException(); }
		}

		#endregion

		#region IXamlTypeResolver Members

		public Type Resolve(string qualifiedTypeName)
		{
			var namespaceProvider = XmlTracker.GetNamespaceProvider(property.Object);
			var typeName = XamlParser.GetTypeName(qualifiedTypeName, namespaceProvider);
			var type = property.Object.Document.Project.TypeFinder.FindType(typeName);
			return type.SystemType;
		}

		#endregion

		#region IUriContext Members

		public Uri BaseUri
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		#endregion

		#region IProvideValueTarget Members

		public object TargetObject
		{
			get { return property.Object.Instance; }
		}

		public object TargetProperty
		{
			get
			{
				var reflectionMember = property.Member as ReflectionMember;
				if (reflectionMember != null) {
					return reflectionMember.Info.DependencyProperty;
				}
				return null;
			}
		}

		#endregion

		#region IValueSerializerContext Members

		public ValueSerializer GetValueSerializerFor(PropertyDescriptor descriptor)
		{
			return null;
		}

		public ValueSerializer GetValueSerializerFor(Type type)
		{
			return null;
		}

		#endregion
	}
}
