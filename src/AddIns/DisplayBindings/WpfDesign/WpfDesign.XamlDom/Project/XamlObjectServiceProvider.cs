// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;

namespace ICSharpCode.WpfDesign.XamlDom
{
	/// <summary>
	/// A service provider that provides the IProvideValueTarget and IXamlTypeResolver services.
	/// No other services (e.g. from the document's service provider) are offered.
	/// </summary>
	public class XamlObjectServiceProvider : IServiceProvider, IProvideValueTarget
	{
		/// <summary>
		/// Creates a new XamlObjectServiceProvider instance.
		/// </summary>
		public XamlObjectServiceProvider(XamlObject obj)
		{
			if (obj == null)
				throw new ArgumentNullException("obj");
			XamlObject = obj;
			Resolver = new XamlTypeResolverProvider(obj);
		}

		/// <summary>
		/// Gets the XamlObject that owns this service provider (e.g. the XamlObject that represents a markup extension).
		/// </summary>
		public XamlObject XamlObject { get; private set; }
		internal XamlTypeResolverProvider Resolver { get; private set; }

		#region IServiceProvider Members

		/// <summary>
		/// Retrieves the service of the specified type.
		/// </summary>
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

		/// <summary>
		/// Gets the target object (the DependencyObject instance on which a property should be set)
		/// </summary>
		public object TargetObject {
			get {
				var parentProperty = XamlObject.ParentProperty;

				if (parentProperty == null) {
					return null;
				}

				if (parentProperty.IsCollection) {
					return parentProperty.ValueOnInstance;
				}

				return parentProperty.ParentObject.Instance;
			}
		}

		/// <summary>
		/// Gets the target dependency property.
		/// </summary>
		public object TargetProperty {
			get {
				var parentProperty = XamlObject.ParentProperty;

				if (parentProperty == null) {
					return null;
				}

				return parentProperty.DependencyProperty;
			}
		}

		#endregion
	}
}
