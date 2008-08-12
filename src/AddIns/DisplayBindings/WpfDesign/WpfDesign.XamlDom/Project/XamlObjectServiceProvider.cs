using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;

namespace ICSharpCode.WpfDesign.XamlDom
{
	public class XamlObjectServiceProvider : IServiceProvider, IProvideValueTarget
	{
		public XamlObjectServiceProvider(XamlObject obj)
		{
			XamlObject = obj;
			Resolver = new XamlTypeResolverProvider(obj);
		}

		public XamlObject XamlObject { get; private set; }
		internal XamlTypeResolverProvider Resolver { get; private set; }

		#region IServiceProvider Members

		public object GetService(Type serviceType)
		{
			if (serviceType == typeof(IProvideValueTarget)) {
				return this;
			}
			if (serviceType == typeof(IXamlTypeResolver)) {
				return Resolver;
			}
			return null;
		}

		#endregion

		#region IProvideValueTarget Members

		public object TargetObject {
			get { return XamlObject.ParentProperty.ParentObject.Instance; }
		}

		public object TargetProperty {
			get { return XamlObject.ParentProperty.DependencyProperty; }
		}

		#endregion
	}
}
