// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using System.Windows.Forms.Design;

using ICSharpCode.Core;
using ICSharpCode.FormsDesigner.Gui.OptionPanels;

namespace ICSharpCode.FormsDesigner.Services
{
	public class SharpDevelopDesignerOptions : DesignerOptions
	{
		bool enableInSituEditing;
		bool objectBoundSmartTagAutoShow;
		bool useOptimizedCodeGeneration;

		Size gridSize = Size.Empty;
		bool showGrid;
		bool snapToGrid;
		bool useSnapLines;
		bool useSmartTags;
		
		public SharpDevelopDesignerOptions()
		{
			int w = PropertyService.Get("FormsDesigner.DesignerOptions.GridSizeWidth",  8);
			int h = PropertyService.Get("FormsDesigner.DesignerOptions.GridSizeHeight", 8);
			this.gridSize = new Size(w, h);
			
			this.showGrid   = PropertyService.Get("FormsDesigner.DesignerOptions.ShowGrid", true);
			this.snapToGrid = PropertyService.Get("FormsDesigner.DesignerOptions.SnapToGrid", true);
			
			this.useSmartTags = GeneralOptionsPanel.UseSmartTags;
			this.useSnapLines = PropertyService.Get("FormsDesigner.DesignerOptions.UseSnapLines", true);

			this.enableInSituEditing         = PropertyService.Get("FormsDesigner.DesignerOptions.EnableInSituEditing", true);
			this.objectBoundSmartTagAutoShow = GeneralOptionsPanel.SmartTagAutoShow;
			this.useOptimizedCodeGeneration  = PropertyService.Get("FormsDesigner.DesignerOptions.UseOptimizedCodeGeneration", true);
		}
		
		// Obtains and shows the size of the standard design-mode grid square.
		public override Size GridSize { 
			get { 
				return gridSize;
			}
		}
		
		// Obtains and shows whether the design mode surface grid is enabled.
		public override bool ShowGrid {
			get { 
				return showGrid;
			}
		}
		
		// Obtains and shows whether components should be aligned with the surface grid.
		public override bool SnapToGrid {
			get { 
				return snapToGrid;
			}
		}
		
		// Gets or sets a value that enables or disables smart tags in the designer.
		public override bool UseSmartTags {
			get { 
				return useSmartTags;
			}
		}
		
		// Gets or sets a value that enables or disables snaplines in the designer.
		public override bool UseSnapLines {
			get { 
				return useSnapLines;
			}
		}

		// Gets or sets a value that enables or disables in-place editing for ToolStrip controls.
		public override bool EnableInSituEditing {
			get { 
				return enableInSituEditing;
			}
		}

		// Obtains and shows whether smart tags are automatically opened.
		public override bool ObjectBoundSmartTagAutoShow {
			get { 
				return objectBoundSmartTagAutoShow;
			}
		}

		// The component cache is a performance enhancement that is incompatible with certain designers. 
		// You can disable it with this property
		public override bool UseOptimizedCodeGeneration {
			get { 
				return useOptimizedCodeGeneration;
			}
		}
	}

	public class SharpDevelopDesignerOptionService : WindowsFormsDesignerOptionService
	{		
		public SharpDevelopDesignerOptionService()
		{
			ApplySharpDevelopSettings();
		}

		void ApplySharpDevelopSettings()
		{
			SharpDevelopDesignerOptions options = new SharpDevelopDesignerOptions();

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
