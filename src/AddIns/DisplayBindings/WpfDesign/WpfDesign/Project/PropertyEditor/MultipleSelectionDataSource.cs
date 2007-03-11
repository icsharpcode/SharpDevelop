// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace ICSharpCode.WpfDesign.PropertyEditor
{
	/// <summary>
	/// Implements IPropertyEditorDataSource by combining the information from multiple data sources.
	/// </summary>
	public sealed class MultipleSelectionDataSource : IPropertyEditorDataSource
	{
		/// <summary>
		/// Creates a data source for a collection of data sources.
		/// </summary>
		/// <param name="services">The service container to use.</param>
		/// <param name="sources">The collection of data sources.</param>
		/// <returns>
		/// <c>null</c>, if sources is <c>null</c> or empty.
		/// <c>sources[0]</c>, if sources.Count == 1.
		/// A new MultiSelectionDataSource, if sources.Count >= 2.
		/// </returns>
		public static IPropertyEditorDataSource CreateDataSource(ServiceContainer services, ICollection<IPropertyEditorDataSource> sources)
		{
			if (sources == null || sources.Count == 0) {
				return null;
			} else if (sources.Count == 1) {
				foreach (IPropertyEditorDataSource s in sources) {
					return s;
				}
				throw new InvalidOperationException();
			} else {
				return new MultipleSelectionDataSource(services, sources);
			}
		}
		
		
		readonly ServiceContainer services;
		readonly IPropertyEditorDataSource[] data;
		readonly List<IPropertyEditorDataProperty> properties = new List<IPropertyEditorDataProperty>();
		
		/// <summary>
		/// Creates a new MultiSelectionDataSource instance.
		/// </summary>
		public MultipleSelectionDataSource(ServiceContainer services, ICollection<IPropertyEditorDataSource> sources)
		{
			this.services = services;
			if (sources == null)
				throw new ArgumentNullException("sources");
			data = Func.ToArray(sources);
			if (data.Length < 2)
				throw new ArgumentException("The collection must have at least 2 items!");
			
			foreach (IPropertyEditorDataProperty property in data[0].Properties) {
				IPropertyEditorDataProperty[] properties = new IPropertyEditorDataProperty[data.Length];
				properties[0] = property;
				for (int i = 1; i < data.Length; i++) {
					foreach (IPropertyEditorDataProperty p in data[i].Properties) {
						if (p.Name == property.Name && p.ReturnType == property.ReturnType) {
							properties[i] = p;
							break;
						}
					}
					if (properties[i] == null) {
						properties = null; // could not find the property for all data sources
						break;
					}
				}
				
				if (properties != null) {
					this.properties.Add(new MultipleSelectionDataProperty(this, properties));
				}
			}
		}
		
		/// <summary>
		/// Is never raised because Name cannot change.
		/// </summary>
		public event EventHandler NameChanged {
			add { } remove { }
		}
		
		/// <summary>
		/// Always returns null. The setter throws an exception.
		/// </summary>
		public string Name {
			get { return null; }
			set { throw new NotSupportedException(); }
		}
		
		/// <summary>
		/// Gets the type of the selected objects.
		/// </summary>
		public string Type {
			get { return "multiple selection"; }
		}
		
		/// <summary>
		/// Gets the icon of the selected objects.
		/// </summary>
		public System.Windows.Media.ImageSource Icon {
			get { return null; }
		}
		
		/// <summary>
		/// Gets the collection of properties.
		/// </summary>
		public ICollection<IPropertyEditorDataProperty> Properties {
			get { return properties.AsReadOnly(); }
		}
		
		/// <summary>
		/// Gets if adding attached properties is supported.
		/// </summary>
		public bool CanAddAttachedProperties {
			get { return false; }
		}
		
		/// <summary>
		/// Gets the service container used by this data source.
		/// </summary>
		public ServiceContainer Services {
			get { return services; }
		}
		
		/// <summary>See <see cref="IPropertyEditorDataSource"/></summary>
		public Brush CreateThumbnailBrush()
		{
			return null;
		}
	}
}
