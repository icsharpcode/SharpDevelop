// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Collections.Generic;

namespace ICSharpCode.WpfDesign.PropertyEditor
{
	/// <summary>
	/// Implements IPropertyEditorDataSource accessing the properties on a DesignItem.
	/// </summary>
	public sealed class DesignItemDataSource : IPropertyEditorDataSource
	{
		readonly DesignItem item;
		readonly List<IPropertyEditorDataProperty> properties = new List<IPropertyEditorDataProperty>();
		
		/// <summary>
		/// Constructs a new DesignItemDataSource for the specified design item.
		/// </summary>
		public DesignItemDataSource(DesignItem item)
		{
			if (item == null)
				throw new ArgumentNullException("item");
			this.item = item;
			
			List<DesignItemProperty> designItemProperties = new List<DesignItemProperty>();
			foreach (PropertyDescriptor p in TypeDescriptor.GetProperties(item.Component)) {
				if (!p.IsBrowsable) continue;
				if (p.IsReadOnly) continue;
				if (p.Name.Contains(".")) continue;
				designItemProperties.Add(item.Properties[p.Name]);
			}
			designItemProperties.AddRange(item.Properties);
			
			foreach (DesignItemProperty p in Linq.Distinct(designItemProperties)) {
				properties.Add(new DesignItemDataProperty(this, p));
			}
		}
		
		/// <summary>
		/// Gets a data source for the specified design item.
		/// This tries to get an existing data source instance (as behavior), or constructs a new
		/// DesignItemDataSource instance if that fails.
		/// </summary>
		public static IPropertyEditorDataSource GetDataSourceForDesignItem(DesignItem item)
		{
			return item.GetBehavior<IPropertyEditorDataSource>() ?? new DesignItemDataSource(item);
		}
		
		/// <summary>
		/// Gets the design item for which this DesignItemDataSource was created.
		/// </summary>
		public DesignItem DesignItem {
			get { return item; }
		}
		
		/// <summary>See <see cref="IPropertyEditorDataSource"/></summary>
		public string Name {
			get { return item.Name ?? ""; }
			set { item.Name = value; }
		}
		
		/// <summary>
		/// Is raised when the name of the design item changes.
		/// </summary>
		public event EventHandler NameChanged {
			add    { item.NameChanged += value; }
			remove { item.NameChanged -= value; }
		}
		
		/// <summary>See <see cref="IPropertyEditorDataSource"/></summary>
		public string Type {
			get {
				return item.Component.GetType().Name;
			}
		}
		
		/// <summary>See <see cref="IPropertyEditorDataSource"/></summary>
		public ImageSource Icon {
			get { return null; }
		}
		
		/// <summary>See <see cref="IPropertyEditorDataSource"/></summary>
		public ICollection<IPropertyEditorDataProperty> Properties {
			get { return properties; }
		}
		
		/// <summary>See <see cref="IPropertyEditorDataSource"/></summary>
		public bool CanAddAttachedProperties {
			get {
				return item.Component is DependencyObject;
			}
		}
	}
}
