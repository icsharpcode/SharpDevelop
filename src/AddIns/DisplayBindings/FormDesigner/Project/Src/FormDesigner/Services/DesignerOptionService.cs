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
	public class DesignerOptionService : IDesignerOptionService
	{
		public const string GridSize   = "GridSize";
		public const string ShowGrid   = "ShowGrid";
		public const string SnapToGrid = "SnapToGrid";
		public const string LayoutMode = "LayoutMode";
		
		const string GridSizeWidth  = "GridSize.Width";
		const string GridSizeHeight = "GridSize.Height";
		
		public const string FormsDesignerPageName = "SharpDevelop Forms Designer\\General";
		
		Hashtable pageOptionTable = new Hashtable();
		
		
		
		public DesignerOptionService()
		{
			pageOptionTable[FormsDesignerPageName] = new Hashtable();
		}
		
		public object GetOptionValue(string pageName, string valueName)
		{
			switch (valueName) {
				case GridSize:
					return new Size(PropertyService.Get("FormsDesigner.DesignerOptions.GridSizeWidth", 8),
					                PropertyService.Get("FormsDesigner.DesignerOptions.GridSizeHeight", 8));
				case ShowGrid:
					return PropertyService.Get("FormsDesigner.DesignerOptions.ShowGrid", true);
				case SnapToGrid:
					return PropertyService.Get("FormsDesigner.DesignerOptions.SnapToGrid", true);
				case GridSizeWidth:
					return PropertyService.Get("FormsDesigner.DesignerOptions.GridSizeWidth", 8);
				case GridSizeHeight:
					return PropertyService.Get("FormsDesigner.DesignerOptions.GridSizeHeight", 8);
//				case LayoutMode:
//					return PropertyService.Get("FormsDesigner.DesignerOptions.LayoutMode", LayoutOptions.SnapLines);
				default:
					Hashtable pageTable = (Hashtable)pageOptionTable[pageName];
					
					if (pageTable == null) {
						return null;
					}
					return pageTable[valueName];
			}
		}
		
		public void SetOptionValue(string pageName, string valueName, object val)
		{
			switch (valueName) {
				case GridSize:
					Size size = (Size)val;
					PropertyService.Set("FormsDesigner.DesignerOptions.GridSizeWidth",  size.Width);
					PropertyService.Set("FormsDesigner.DesignerOptions.GridSizeHeight", size.Height);
					break;
				case ShowGrid:
					PropertyService.Set("FormsDesigner.DesignerOptions.ShowGrid", (bool)val);
					break;
				case SnapToGrid:
					PropertyService.Set("FormsDesigner.DesignerOptions.SnapToGrid", (bool)val);
					break;
				case GridSizeWidth:
					PropertyService.Set("FormsDesigner.DesignerOptions.GridSizeWidth", (int)val);
					break;
				case GridSizeHeight:
					PropertyService.Set("FormsDesigner.DesignerOptions.GridSizeHeight", (int)val);
					break;
//				case LayoutMode:
//					PropertyService.Set("FormsDesigner.DesignerOptions.LayoutMode", (LayoutOptions)val);
//					break;
					
				default:
					Hashtable pageTable = (Hashtable)pageOptionTable[pageName];
					if (pageTable == null) {
						pageOptionTable[pageName] = pageTable = new Hashtable();
					}
					pageTable[valueName] = val;
					break;
			}
		}
	}
}
