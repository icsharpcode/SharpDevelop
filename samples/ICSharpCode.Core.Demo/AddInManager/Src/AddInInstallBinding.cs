// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;

namespace ICSharpCode.AddInManager
{
	#if !STANDALONE
	public class AddInInstallBinding : IDisplayBinding
	{
		public bool CanCreateContentForFile(string fileName)
		{
			return true;
		}
		
		public ICSharpCode.SharpDevelop.Gui.IViewContent CreateContentForFile(string fileName)
		{
			ManagerForm.ShowForm();
			ManagerForm.Instance.ShowInstallableAddIns(new string[] { fileName });
			return null;
		}
		
		public bool CanCreateContentForLanguage(string languageName)
		{
			return false;
		}
		
		public ICSharpCode.SharpDevelop.Gui.IViewContent CreateContentForLanguage(string languageName, string content)
		{
			throw new NotImplementedException();
		}
	}
	#endif
}
