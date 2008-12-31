using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows.Markup;
using System.ComponentModel;
using System.Windows.Data;
using System.Diagnostics;
using System.Windows;

namespace SharpDevelop.XamlDesigner.Palette
{
	[ContentProperty("Items")]
	public class PaletteAssembly : PaletteNode
	{
		public PaletteAssembly()
		{
			Items = new PaletteItemCollection(this);
			ItemsView = CollectionViewSource.GetDefaultView(Items);
			ItemsView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
		}

		public Assembly Assembly { get; private set; }		
		
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PaletteItemCollection Items { get; private set; }

		public ICollectionView ItemsView { get; private set; }
		public PaletteData ParentData { get; private set; }

		public string ShortName
		{
			get 
			{
				if (Assembly != null) {
					return Assembly.GetName().Name;
				}
				return null;
			}
		}

		string name;

		[DefaultValue(null)]
		public string Name
		{
			get { return name; }
			set
			{
				name = value;
				try {
					Assembly = Assembly.Load(name);
				}
				catch {
					//TODO
				}
			}
		}

		string path;

		[DefaultValue(null)]
		public string Path
		{
			get { return path; }
			set
			{
				path = value;
				try {
					var fullPath = System.IO.Path.GetFullPath(path);
					Assembly = Assembly.LoadFile(fullPath);
				}
				catch {
					//TODO
				}
			}
		}

		public string ToolTip
		{
			get { return Path ?? Name; }
		}

		internal void SetParent(PaletteData parent)
		{
			ParentData = parent;
		}

		public void LoadItems(bool includeNewItems)
		{
			HashSet<Type> existingTypes = new HashSet<Type>(Items.Select(t => t.Type));
			foreach (var type in Assembly.GetExportedTypes()) {
				if (typeof(FrameworkElement).IsAssignableFrom(type) && !type.IsAbstract) {
					if (!existingTypes.Contains(type)) {
						Items.Add(new PaletteItem() {
							TypeName = type.FullName,
							IsIncluded = includeNewItems
						});
					}
				}
			}
		}
	}
}
