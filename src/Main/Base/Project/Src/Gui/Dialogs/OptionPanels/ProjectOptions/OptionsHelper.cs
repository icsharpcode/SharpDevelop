// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)


using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using Microsoft.Win32;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	/// <summary>
	/// Description of OptionsHelper.
	/// </summary>
	public class OptionsHelper
	{
		
		public static string BrowseForFolder(string description,string baseDirectory,
		                                     string startLocation,string relativeLocation,
		                                     TextBoxEditMode textBoxEditMode)
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
					if (textBoxEditMode == TextBoxEditMode.EditEvaluatedProperty) {
						return path;
					} else {
						return MSBuildInternals.Escape(path);
					}
				}
			}
			return startLocation;
		}

		
		/// <summary>
		/// Open File
		/// </summary>
		/// <param name="filter" or String.Empty></param>
		/// <returns FileName></returns>
		public static string OpenFile (string filter,string baseDirectory,TextBoxEditMode textBoxEditMode)
		{
			var dialog = new OpenFileDialog();
			if (!String.IsNullOrEmpty(filter)) {
				dialog.Filter = StringParser.Parse(filter);
			}
			
			if (dialog.ShowDialog() ?? false) {
				string fileName = dialog.FileName;
				if (!String.IsNullOrEmpty(baseDirectory)) {
						fileName = FileUtility.GetRelativePath(baseDirectory, fileName);
					}
				if (textBoxEditMode == TextBoxEditMode.EditEvaluatedProperty) {
					return fileName;
				} else {
					return MSBuildInternals.Escape(fileName);
				}
			}
			return string.Empty;
		}
	}
}
