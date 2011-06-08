// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit.AddIn;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CustomizedHighlightingRules : ICustomizedHighlightingRules
	{
		IPackageManagementWorkbench workbench;
		
		public CustomizedHighlightingRules()
		{
			this.workbench = new PackageManagementWorkbench();
		}
		
		public List<CustomizedHighlightingColor> LoadColors()
		{
			return CustomizedHighlightingColor.LoadColors();
		}
		
		public void SaveColors(IEnumerable<CustomizedHighlightingColor> colors)
		{
			if (workbench.InvokeRequired) {
				Action<IEnumerable<CustomizedHighlightingColor>> action = SaveColors;
				workbench.SafeThreadAsyncCall(action, colors);
			} else {
				CustomizedHighlightingColor.SaveColors(colors);
			}
		}
	}
}
