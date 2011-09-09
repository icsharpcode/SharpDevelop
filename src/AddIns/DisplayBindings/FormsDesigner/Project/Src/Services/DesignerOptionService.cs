// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using System.Windows.Forms.Design;

namespace ICSharpCode.FormsDesigner.Services
{
	[Serializable]
	public class SharpDevelopDesignerOptions
	{
		// Gets or sets a value that enables or disables in-place editing for ToolStrip controls.
		public bool EnableInSituEditing { get; set; }
		
		// Obtains and shows whether smart tags are automatically opened.
		public bool ObjectBoundSmartTagAutoShow { get; set; }
		
		// The component cache is a performance enhancement that is incompatible with certain designers.
		// You can disable it with this property
		public bool UseOptimizedCodeGeneration { get; set; }

		// Obtains and shows the size of the standard design-mode grid square.
		public Size GridSize { get; set; }
		
		// Obtains and shows whether the design mode surface grid is enabled.
		public bool ShowGrid { get; set; }
		
		// Obtains and shows whether components should be aligned with the surface grid.
		public bool SnapToGrid { get; set; }
		
		// Gets or sets a value that enables or disables snaplines in the designer.
		public bool UseSnapLines { get; set; }
		
		// Gets or sets a value that enables or disables smart tags in the designer.
		public bool UseSmartTags { get; set; }
		
		// Whether to include an underscore in the event handler name
		public string EventHandlerNameFormat { get; set; }
		
		public bool PropertyGridSortAlphabetical { get; set; }
	}

	public class SharpDevelopDesignerOptionService : WindowsFormsDesignerOptionService
	{
		public SharpDevelopDesignerOptionService(SharpDevelopDesignerOptions options)
		{
			this.Options.Properties.Find("GridSize", true).SetValue(this, options.GridSize);
			this.Options.Properties.Find("EnableInSituEditing", true).SetValue(this, options.EnableInSituEditing);
			this.Options.Properties.Find("ObjectBoundSmartTagAutoShow", true).SetValue(this, options.ObjectBoundSmartTagAutoShow);
			this.Options.Properties.Find("UseOptimizedCodeGeneration", true).SetValue(this, options.UseOptimizedCodeGeneration);
			this.Options.Properties.Find("UseSmartTags", true).SetValue(this, options.UseSmartTags);
			this.Options.Properties.Find("UseSnapLines", true).SetValue(this, options.UseSnapLines);
			
			if (options.UseSnapLines) {
				this.Options.Properties.Find("ShowGrid", true).SetValue(this, false);
				this.Options.Properties.Find("SnapToGrid", true).SetValue(this, false);
			} else {
				this.Options.Properties.Find("ShowGrid", true).SetValue(this, options.ShowGrid);
				this.Options.Properties.Find("SnapToGrid", true).SetValue(this, options.SnapToGrid);
			}
		}
	}
}
