// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace ICSharpCode.WpfDesign.PropertyEditor
{
	/// <summary>
	/// Implements IPropertyEditorDataSource accessing the properties on a DesignItem.
	/// </summary>
	public sealed class DesignItemDataSource : IPropertyEditorDataSource
	{
		readonly DesignItem item;
		readonly List<IPropertyEditorDataProperty> properties = new List<IPropertyEditorDataProperty>();
		readonly List<IPropertyEditorDataEvent> events = new List<IPropertyEditorDataEvent>();
		
		#region Available properties
		// cache properties available for a type - retrieving this list takes ~100ms on my machine, so
		// we cannot do this for every selected component, but must reuse the list
		static readonly Dictionary<Type, string[]> availableProperties = new Dictionary<Type, string[]>();
		
		static string[] GetAvailableProperties(Type forType)
		{
			Debug.Assert(forType != null);
			string[] result;
			lock (availableProperties) {
				if (availableProperties.TryGetValue(forType, out result))
					return result;
			}
			List<string> names = new List<string>();
			foreach (PropertyDescriptor p in TypeDescriptor.GetProperties(forType)) {
				if (!p.IsBrowsable) continue;
				if (p.IsReadOnly) continue;
				if (p.Name.Contains(".")) continue;
				names.Add(p.Name);
			}
			result = names.ToArray();
			lock (availableProperties) {
				availableProperties[forType] = result;
			}
			return result;
		}
		#endregion
		
		#region Available events
		// cache events available for a type
		static readonly Dictionary<Type, string[]> availableEvents = new Dictionary<Type, string[]>();
		
		static string[] GetAvailableEvents(Type forType)
		{
			Debug.Assert(forType != null);
			string[] result;
			lock (availableEvents) {
				if (availableEvents.TryGetValue(forType, out result))
					return result;
			}
			List<string> names = new List<string>();
			foreach (EventDescriptor p in TypeDescriptor.GetEvents(forType)) {
				if (!p.IsBrowsable) continue;
				if (p.Name.Contains(".")) continue;
				names.Add(p.Name);
			}
			result = names.ToArray();
			lock (availableEvents) {
				availableEvents[forType] = result;
			}
			return result;
		}
		#endregion
		
		/// <summary>
		/// Constructs a new DesignItemDataSource for the specified design item.
		/// </summary>
		public DesignItemDataSource(DesignItem item)
		{
			if (item == null)
				throw new ArgumentNullException("item");
			this.item = item;
			
			List<DesignItemProperty> designItemProperties = new List<DesignItemProperty>();
			foreach (string name in GetAvailableProperties(item.ComponentType)) {
				designItemProperties.Add(item.Properties[name]);
			}
			foreach (string name in GetAvailableEvents(item.ComponentType)) {
				designItemProperties.Add(item.Properties[name]);
			}
			designItemProperties.AddRange(item.Properties);
			
			foreach (DesignItemProperty p in designItemProperties.Distinct()) {
				if (p.IsEvent) {
					events.Add(new DesignItemDataEvent(this, p));
				} else {
					properties.Add(new DesignItemDataProperty(this, p));
				}
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
		/// Gets a data source for the specified design item.
		/// This tries to get an existing data source instance (as behavior), or constructs a new
		/// DesignItemDataSource instance if that fails.
		/// </summary>
		public static IPropertyEditorDataSource GetDataSourceForDesignItems(ICollection<DesignItem> items)
		{
			IPropertyEditorDataSource[] sources = new IPropertyEditorDataSource[items.Count];
			DesignContext context = null;
			int i = 0;
			foreach (DesignItem item in items) {
				if (context == null)
					context = item.Context;
				else if (context != item.Context)
					throw new DesignerException("All specified items must use the same design context!");
				
				sources[i++] = GetDataSourceForDesignItem(item);
			}
			return MultipleSelectionDataSource.CreateDataSource(context != null ? context.Services : null, sources);
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
				return item.ComponentType.Name;
			}
		}
		
		/// <summary>See <see cref="IPropertyEditorDataSource"/></summary>
		public ImageSource Icon {
			get { return null; }
		}
		
		/// <summary>See <see cref="IPropertyEditorDataSource"/></summary>
		public ICollection<IPropertyEditorDataProperty> Properties {
			get { return properties.AsReadOnly(); }
		}
		
		/// <summary>See <see cref="IPropertyEditorDataSource"/></summary>
		public ICollection<IPropertyEditorDataEvent> Events {
			get { return events.AsReadOnly(); }
		}
		
		/// <summary>See <see cref="IPropertyEditorDataSource"/></summary>
		public bool CanAddAttachedProperties {
			get {
				return item.Component is DependencyObject;
			}
		}
		
		/// <summary>See <see cref="IPropertyEditorDataSource"/></summary>
		public ServiceContainer Services {
			get { return item.Services; }
		}
		
		/// <summary>See <see cref="IPropertyEditorDataSource"/></summary>
		public Brush CreateThumbnailBrush()
		{
			if (item.View != null) {
				VisualBrush b = new VisualBrush(item.View);
				b.AutoLayoutContent = false;
				return b;
			} else {
				return null;
			}
		}
	}
}
