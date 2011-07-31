// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit.AddIn;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public interface ICustomizedHighlightingRules
	{
		List<CustomizedHighlightingColor> LoadColors();
		void SaveColors(IEnumerable<CustomizedHighlightingColor> colors);
	}
}
