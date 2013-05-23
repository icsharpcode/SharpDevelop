// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using System.IO;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Workbench
{
	/// <summary>
	/// Opens files with the default Windows application for them.
	/// </summary>
	sealed class ShellExecuteDisplayBinding : IDisplayBinding
	{
		public bool CanCreateContentForFile(FileName fileName)
		{
			return true;
		}
		
		public IViewContent CreateContentForFile(OpenedFile file)
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
		
		public bool IsPreferredBindingForFile(FileName fileName)
		{
			return false;
		}
		
		public double AutoDetectFileContent(FileName fileName, Stream fileContent, string detectedMimeType)
		{
			return double.NegativeInfinity;
		}
	}
}
