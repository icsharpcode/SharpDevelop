// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
