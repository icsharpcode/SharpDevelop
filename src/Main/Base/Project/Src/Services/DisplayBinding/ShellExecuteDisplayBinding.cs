// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using System.Diagnostics;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Opens files with the default Windows application for them.
	/// </summary>
	public class ShellExecuteDisplayBinding : IDisplayBinding
	{
		public bool CanCreateContentForFile(string fileName)
		{
			return !FileUtility.IsUrl(fileName);
		}
		
		public ICSharpCode.SharpDevelop.Gui.IViewContent CreateContentForFile(OpenedFile file)
		{
			if (file.IsDirty) {
				// TODO: warn user that the file must be saved
			}
			try {
				Process.Start(file.FileName);
			} catch (Exception ex) {
				MessageService.ShowError(ex.Message);
			}
			return null;
		}
	}
}
