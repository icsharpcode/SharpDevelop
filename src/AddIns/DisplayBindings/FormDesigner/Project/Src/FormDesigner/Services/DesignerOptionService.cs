// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Drawing;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;
using ICSharpCode.Core;

namespace ICSharpCode.FormDesigner.Services
{
	public class SharpDevelopDesignerOptions : DesignerOptions
	{
		public SharpDevelopDesignerOptions()
		{
			UseSmartTags = true;
			UseSnapLines = true;
		}
		
//		public override Size GridSize { 
//			get {
//				return new Size(PropertyService.Get("FormsDesigner.DesignerOptions.GridSizeWidth", 8),
//					PropertyService.Get("FormsDesigner.DesignerOptions.GridSizeHeight", 8));
//			}
//			set {
//				LoggingService.Debug("GridSize set");
//				PropertyService.Set("FormsDesigner.DesignerOptions.GridSizeWidth",  value.Width);
//				PropertyService.Set("FormsDesigner.DesignerOptions.GridSizeHeight", value.Height);
//			}
//		}
//		
//		public override bool ShowGrid {
//			get {
//				LoggingService.Debug("ShowGrid get");
//				return PropertyService.Get("FormsDesigner.DesignerOptions.ShowGrid", true);
//			}
//			set {
//				LoggingService.Debug("ShowGrid set");
//				PropertyService.Set("FormsDesigner.DesignerOptions.ShowGrid", value);
//			}
//		}
//		
//		public override bool SnapToGrid {
//			get {
//				LoggingService.Debug("SnapToGrid get");
//				return PropertyService.Get("FormsDesigner.DesignerOptions.SnapToGrid", true);
//			}
//			set {
//				LoggingService.Debug("SnapToGrid set");
//			}
//		}
	}
	
	public class DesignerOptionService : WindowsFormsDesignerOptionService
	{		
		DesignerOptions options;
		
		public DesignerOptionService()
		{
		}
		
		public override DesignerOptions CompatibilityOptions { 
			get {
				if (options == null) {
					options = new SharpDevelopDesignerOptions();
				}
				return options;
			}		
		}
	}
}
