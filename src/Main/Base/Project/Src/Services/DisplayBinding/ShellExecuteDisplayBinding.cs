// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
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
			return true;
		}
		
		public ICSharpCode.SharpDevelop.Gui.IViewContent CreateContentForFile(OpenedFile file)
		{
			if (file.IsDirty) {
				// TODO: warn user that the file must be saved
			}
			try {
				Process.Start(new ProcessStartInfo(file.FileName) {
				              	WorkingDirectory = Path.GetDirectoryName(file.FileName)
				              });
			} catch (Exception ex) {
				MessageService.ShowError(ex.Message);
			}
			return null;
		}
		
		public bool IsPreferredBindingForFile(string fileName)
		{
			return false;
		}
		
		public double AutoDetectFileContent(string fileName, Stream fileContent, string detectedMimeType)
		{
			return double.NegativeInfinity;
		}
	}
}
