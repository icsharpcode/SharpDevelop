// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.AddInManager
{
	#if !STANDALONE
	public class AddInInstallBinding : IDisplayBinding
	{
		public bool CanCreateContentForFile(string fileName)
		{
			return true;
		}
		
		public ICSharpCode.SharpDevelop.Gui.IViewContent CreateContentForFile(OpenedFile file)
		{
			ManagerForm.ShowForm();
			ManagerForm.Instance.ShowInstallableAddIns(new string[] { file.FileName });
			return null;
		}
		
		public bool IsPreferredBindingForFile(string fileName)
		{
			return true;
		}
		
		public double AutoDetectFileContent(string fileName, System.IO.Stream fileContent, string detectedMimeType)
		{
			return 1;
		}
	}
	#endif
}
