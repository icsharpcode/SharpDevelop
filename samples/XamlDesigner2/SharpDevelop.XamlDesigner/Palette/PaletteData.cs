using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Markup;
using System.Windows;
using System.Windows.Data;
using System.ComponentModel;

namespace SharpDevelop.XamlDesigner.Palette
{
	[ContentProperty("Assemblies")]
	public class PaletteData : ViewModel
	{
		public PaletteData()
		{
			Assemblies = new PaletteAssemblyCollection(this);
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PaletteAssemblyCollection Assemblies { get; private set; }

		bool showAll;

		[DefaultValue(false)]
		public bool ShowAll
		{
			get
			{
				return showAll;
			}
			set
			{
				if (showAll != value) {
					showAll = value;
					if (showAll) {
						ShowAllNodes();
					}
					else {
						ShowIncludedNodes();
					}
					RaisePropertyChanged("ShowAll");
				}
			}
		}

		void ShowAllNodes()
		{
			foreach (var assembly in Assemblies) {
				assembly.LoadItems(false);
			}
		}

		void ShowIncludedNodes()
		{
			foreach (var assembly in Assemblies) {
				foreach (var item in assembly.Items.ToArray()) {
					if (!item.IsIncluded) {
						assembly.Items.Remove(item);
					}
				}
			}
		}

		public void Include(IEnumerable<PaletteNode> nodes)
		{
			foreach (var item in nodes.OfType<PaletteItem>().ToArray()) {
				item.IsIncluded = true;
			}
		}

		public void Exclude(IEnumerable<PaletteNode> nodes)
		{
			foreach (var item in nodes.OfType<PaletteItem>().ToArray()) {
				item.IsIncluded = false;
				if (!ShowAll) {
					item.ParentAssembly.Items.Remove(item);
				}
			}
		}

		public void Remove(IEnumerable<PaletteNode> assemblies)
		{
			foreach (PaletteAssembly assembly in assemblies.ToArray()) {
				Assemblies.Remove(assembly);
			}			
		}

		public void AddAssembly(string path)
		{
			var newAssembly = new PaletteAssembly() { Path = path };
			Assemblies.Add(newAssembly);
			newAssembly.LoadItems(true);
		}
	}
}
