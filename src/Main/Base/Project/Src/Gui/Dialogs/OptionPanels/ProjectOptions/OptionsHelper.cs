// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)


using System;
using ICSharpCode.Core;
using Microsoft.Win32;
namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	/// <summary>
	/// Description of OptionsHelper.
	/// </summary>
	public class OptionsHelper
	{
		
		public static string BrowseForFolder(string description,string baseDirectory,string startLocation,string relativeLocation)
		{
			string startAt = startLocation;
			if (!String.IsNullOrEmpty(relativeLocation)) {
				startAt = FileUtility.GetAbsolutePath(startLocation,relativeLocation);
			}
			
			
			using (System.Windows.Forms.FolderBrowserDialog fdiag = FileService.CreateFolderBrowserDialog(description,startAt))
			{
				if (fdiag.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
					string path = fdiag.SelectedPath;
					if (baseDirectory != null) {
						path = FileUtility.GetRelativePath(baseDirectory, path);
					}
					if (!path.EndsWith("\\") && !path.EndsWith("/"))
						path += "\\";
					return path;
				}
				
			}
			return startLocation;
		}
		
		
		public static string OpenFile (string filter)
		{
			var dialog = new OpenFileDialog();
			dialog.Filter = StringParser.Parse(filter);
			if (dialog.ShowDialog() ?? false) {
				return dialog.FileName;
			}
			return string.Empty;
		}
	}
}
