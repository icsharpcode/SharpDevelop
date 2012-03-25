// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.ReferenceDialog.ServiceReference
{
	public class SvcUtilOptions : ServiceReferenceGeneratorOptions
	{
		public SvcUtilOptions()
		{
		}
		
		public string GetNamespaceMapping()
		{
			if (Namespace != null) {
				return "*," + Namespace;
			}
			return null;
		}
	}
}
