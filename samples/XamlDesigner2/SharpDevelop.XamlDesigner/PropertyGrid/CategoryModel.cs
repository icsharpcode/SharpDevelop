using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SharpDevelop.XamlDesigner.PropertyGrid
{
	/// <summary>
	/// View-Model class for a property grid category.
	/// </summary>
	public class CategoryModel : ViewModel
	{
		public CategoryModel(string name)
		{
			Name = name;
			Properties = new PropertyNodeCollection();
		}

		public string Name { get; private set; }
		public PropertyNodeCollection Properties { get; private set; }

		bool isVisible;

		public bool IsVisible
		{
			get
			{
				return isVisible;
			}
			set
			{
				isVisible = value;
				RaisePropertyChanged("IsVisible");
			}
		}
	}
}
