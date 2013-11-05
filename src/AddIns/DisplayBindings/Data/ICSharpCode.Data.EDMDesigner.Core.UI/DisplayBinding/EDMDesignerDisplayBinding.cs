// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.IO;
using System.Linq;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Workbench;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.UI.DisplayBinding
{
	public class EDMDesignerDisplayBinding : IDisplayBinding
	{
		public bool CanCreateContentForFile(FileName fileName)
		{
			return true; // .addin file filters for *.edmx
		}

		public IViewContent CreateContentForFile(OpenedFile file)
		{
			try {
				return new EDMDesignerViewContent(file);
			} catch (WizardCancelledException) {
				return null;
			}
		}
		
		public bool IsPreferredBindingForFile(FileName fileName)
		{
			return true;
		}
		
		public double AutoDetectFileContent(FileName fileName, Stream fileContent, string detectedMimeType)
		{
			return 1;
		}
	}
	
	public class WizardCancelledException : Exception {}
}
