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
using System.Runtime.CompilerServices;
using System.Windows.Markup;
using System.Windows;

using ICSharpCode.WpfDesign.XamlDom;

namespace ICSharpCode.WpfDesign.Designer.Xaml
{
	sealed class XamlComponentService : IComponentService
	{
		public event EventHandler<DesignItemPropertyChangedEventArgs> PropertyChanged;
		
		#region IdentityEqualityComparer
		sealed class IdentityEqualityComparer : IEqualityComparer<object>
		{
			internal static readonly IdentityEqualityComparer Instance = new IdentityEqualityComparer();
			
			int IEqualityComparer<object>.GetHashCode(object obj)
			{
				return RuntimeHelpers.GetHashCode(obj);
			}
			
			bool IEqualityComparer<object>.Equals(object x, object y)
			{
				return x == y;
			}
		}
		#endregion
		
		readonly XamlDesignContext _context;
		
		public XamlComponentService(XamlDesignContext context)
		{
			this._context = context;
		}
		
		public event EventHandler<DesignItemEventArgs> ComponentRegistered;
		
		// TODO: this must not be a dictionary because there's no way to unregister components
		// however, this isn't critical because our design items will stay alive for the lifetime of the
		// designer anyway if we don't limit the Undo stack.
		Dictionary<object, XamlDesignItem> _sites = new Dictionary<object, XamlDesignItem>(IdentityEqualityComparer.Instance);
		
		public DesignItem GetDesignItem(object component)
		{
			if (component == null)
				throw new ArgumentNullException("component");
			XamlDesignItem site;
			_sites.TryGetValue(component, out site);
			return site;
		}

		public void SetDefaultPropertyValues(DesignItem designItem)
		{
			var values = Metadata.GetDefaultPropertyValues(designItem.ComponentType);
			if (values != null) {
				foreach (var value in values) {
					designItem.Properties[value.Key].SetValue(value.Value);
				}
			}
		}

		public DesignItem RegisterComponentForDesigner(object component)
		{
			if (component == null) {
				component = new NullExtension();
			} else if (component is Type) {
				component = new TypeExtension((Type)component);
			}
			
			XamlDesignItem item = new XamlDesignItem(_context.Document.CreateObject(component), _context);
			if (!(component is string))
				_sites.Add(component, item);
			if (ComponentRegistered != null) {
				ComponentRegistered(this, new DesignItemEventArgs(item));
			}
			return item;
		}
		
		/// <summary>
		/// registers components from an existing XAML tree
		/// </summary>
		internal XamlDesignItem RegisterXamlComponentRecursive(XamlObject obj)
		{
			if (obj == null) return null;
			
			foreach (XamlProperty prop in obj.Properties) {
				RegisterXamlComponentRecursive(prop.PropertyValue as XamlObject);
				foreach (XamlPropertyValue val in prop.CollectionElements) {
					RegisterXamlComponentRecursive(val as XamlObject);
				}
			}
			
			XamlDesignItem site = new XamlDesignItem(obj, _context);
			_sites.Add(site.Component, site);
			if (ComponentRegistered != null) {
				ComponentRegistered(this, new DesignItemEventArgs(site));
			}
			
			if (_context.RootItem != null && !string.IsNullOrEmpty(site.Name)) {
				var nameScope = NameScopeHelper.GetNameScopeFromObject(((XamlDesignItem)_context.RootItem).XamlObject);

				if (nameScope != null) {
					// The object will be a part of the RootItem namescope, remove local namescope if set
					NameScopeHelper.ClearNameScopeProperty(obj.Instance);
					
					string newName = site.Name;
					if (nameScope.FindName(newName) != null) {
						int copyIndex = newName.LastIndexOf("_Copy", StringComparison.Ordinal);
						if (copyIndex < 0) {
							newName += "_Copy";
						}
						else if (!newName.EndsWith("_Copy", StringComparison.Ordinal)) {
							string copyEnd = newName.Substring(copyIndex + "_Copy".Length);
							int copyEndValue;
							if (Int32.TryParse(copyEnd, out copyEndValue))
								newName = newName.Remove(copyIndex + "_Copy".Length);
							else
								newName += "_Copy";
						}
						
						int i = 1;
						string newNameTemplate = newName;
						while (nameScope.FindName(newName) != null) {
							newName = newNameTemplate + i++;
						}
						
						site.Name = newName;
					}

					nameScope.RegisterName(newName, obj.Instance);
				}
			}
			return site;
		}
		
		/// <summary>
		/// raises the Property changed Events
		/// </summary>
		internal void RaisePropertyChanged(XamlModelProperty property)
		{
			var ev = this.PropertyChanged;
			if (ev != null) {
				ev(this, new DesignItemPropertyChangedEventArgs(property.DesignItem, property));
			}
		}
	}
}
