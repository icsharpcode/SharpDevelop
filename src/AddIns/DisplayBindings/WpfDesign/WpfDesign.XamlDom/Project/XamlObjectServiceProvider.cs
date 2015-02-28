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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Markup;
using System.Xaml;

namespace ICSharpCode.WpfDesign.XamlDom
{
	/// <summary>
	/// A service provider that provides the IProvideValueTarget and IXamlTypeResolver services.
	/// No other services (e.g. from the document's service provider) are offered.
	/// </summary>
	public class XamlObjectServiceProvider : IServiceProvider, IXamlNameResolver, IProvideValueTarget, IXamlSchemaContextProvider, IAmbientProvider, IUriContext
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
			if (serviceType == typeof(XamlTypeResolverProvider))
			{
				return Resolver;
			}
			if (serviceType == typeof(IXamlSchemaContextProvider)) {
				return this;
			}
			if (serviceType == typeof(IAmbientProvider)) {
				return this;
			}
			if (serviceType == typeof(IXamlNameResolver))
			{
				return this;
			}
			if (serviceType == typeof(IUriContext))
			{
				return this;
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
		
		#region IUriContext implementation

		/// <inheritdoc/>		
		public virtual Uri BaseUri {
			get {
				return new Uri("pack://application:,,,/");
			}
			set {
				
			}
		}
		
		#endregion

		#region IXamlSchemaContextProvider Members

		private XamlSchemaContext iCsharpXamlSchemaContext;

		//Maybe we new our own XamlSchemaContext?
		//private class ICsharpXamlSchemaContext : XamlSchemaContext
		//{
		//    public override XamlType GetXamlType(Type type)
		//    {
		//        return base.GetXamlType(type);
		//    }
		//}

		/// <inheritdoc/>
		public XamlSchemaContext SchemaContext
		{
			get
			{
				return iCsharpXamlSchemaContext = iCsharpXamlSchemaContext ?? System.Windows.Markup.XamlReader.GetWpfSchemaContext(); // new XamlSchemaContext();
			}
		}

		#endregion

		#region IAmbientProvider Members

		/// <inheritdoc/>
		public AmbientPropertyValue GetFirstAmbientValue(IEnumerable<XamlType> ceilingTypes, params XamlMember[] properties)
		{
			return GetAllAmbientValues(ceilingTypes, properties).FirstOrDefault();
		}

		/// <inheritdoc/>
		public object GetFirstAmbientValue(params XamlType[] types)
		{
			return null;
		}

		/// <inheritdoc/>
		public IEnumerable<AmbientPropertyValue> GetAllAmbientValues(IEnumerable<XamlType> ceilingTypes, params XamlMember[] properties)
		{
			var obj = this.XamlObject.ParentObject;

			while (obj != null)
			{
				if (ceilingTypes.Any(x => obj.SystemXamlTypeForProperty.CanAssignTo(x)))
				{
					foreach (var pr in obj.Properties)
					{
						if (properties.Any(x => x.Name == pr.PropertyName))
						{
							yield return new AmbientPropertyValue(pr.SystemXamlMemberForProperty, pr.ValueOnInstance);
						}
					}
				}

				obj = obj.ParentObject;
			}
		}

		/// <inheritdoc/>
		public IEnumerable<object> GetAllAmbientValues(params XamlType[] types)
		{
			return new List<object>();
		}

		/// <inheritdoc/>
		public IEnumerable<AmbientPropertyValue> GetAllAmbientValues(IEnumerable<XamlType> ceilingTypes, bool searchLiveStackOnly, IEnumerable<XamlType> types, params XamlMember[] properties)
		{
			return new List<AmbientPropertyValue>();
		}

		#endregion

		#region IXamlNameResolver

		/// <inheritdoc/>
		public object Resolve(string name)
		{
			INameScope ns = null;
			var xamlObj = this.XamlObject;
			while (xamlObj != null)
			{
				ns = NameScopeHelper.GetNameScopeFromObject(xamlObj);

				if (ns != null) {
					var obj = ns.FindName(name);
					if (obj != null)
						return obj;
				}

				xamlObj = xamlObj.ParentObject;
			}
			
			return null;
		}

		/// <inheritdoc/>
		public object Resolve(string name, out bool isFullyInitialized)
		{
			var ret = Resolve(name);
			isFullyInitialized = ret != null;
			return ret;
		}

		/// <inheritdoc/>
		public object GetFixupToken(IEnumerable<string> names)
		{
			return null;
		}

		/// <inheritdoc/>
		public object GetFixupToken(IEnumerable<string> names, bool canAssignDirectly)
		{
			return null;
		}

		/// <inheritdoc/>
		public IEnumerable<KeyValuePair<string, object>> GetAllNamesAndValuesInScope()
		{
			return null;
		}

		/// <inheritdoc/>
		public bool IsFixupTokenAvailable
		{
			get { return false; }
		}

#pragma warning disable 0067 // Required by interface implementation, disable Warning CS0067: The event is never used
		/// <inheritdoc/>
		public event EventHandler OnNameScopeInitializationComplete;
#pragma warning restore 0067

		#endregion
	}
}
