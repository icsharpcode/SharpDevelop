using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.ComponentModel;

namespace SharpDevelop.XamlDesigner.Palette
{
	public class PaletteItem : PaletteNode
	{
		public Type Type { get; internal set; }
		public string TypeName { get; set; }
		public PaletteAssembly ParentAssembly { get; private set; }

		public string Name
		{
			get { return Type != null ? Type.Name : null; }
		}

		public object Icon
		{
			get 
			{
				if (Type != null) {
					var stream = DesignResources.GetStream("Palette/Icons/" + Type.Name + ".png");
					if (stream != null) {
						return BitmapFrame.Create(stream);
					}
					return "Icons/ElementGray.png";
				}
				return null;
			}
		}

		bool isIncluded = true;

		[DefaultValue(true)]
		public bool IsIncluded
		{
			get
			{
				return isIncluded;
			}
			set
			{
				if (isIncluded != value) {
					isIncluded = value;
					RaisePropertyChanged("IsIncluded");
				}
			}
		}

		CreateTool createTool;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public CreateTool CreateTool
		{
			get 
			{
				if (createTool == null) {
					createTool = new CreateTool(Type);
				}
				return createTool; 
			}
		}

		internal void SetParent(PaletteAssembly parent)
		{
			ParentAssembly = parent;
			if (Type == null && ParentAssembly.Assembly != null) {
				Type = ParentAssembly.Assembly.GetType(TypeName);
			}
		}
	}
}
