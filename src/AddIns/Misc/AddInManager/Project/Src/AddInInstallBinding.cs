// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

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
	}
	#endif
}
