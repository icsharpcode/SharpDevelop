// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using System.IO;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.UI.DisplayBinding
{
	public class EDMDesignerDisplayBinding : IDisplayBinding
	{
		public bool CanCreateContentForFile(string fileName)
		{
			return Path.GetExtension(fileName).Equals(".edmx", StringComparison.OrdinalIgnoreCase);
		}

		public IViewContent CreateContentForFile(OpenedFile file)
		{
			try {
				return new EDMDesignerViewContent(file);
			} catch (WizardCancelledException) {
				return null;
			}
		}
	}
	
	public class WizardCancelledException : Exception {}
}
