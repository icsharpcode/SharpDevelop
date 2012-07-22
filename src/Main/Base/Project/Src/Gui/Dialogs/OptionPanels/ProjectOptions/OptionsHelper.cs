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
		/*
		public void Event(object sender, EventArgs e)
			{
				string startLocation = panel.baseDirectory;
				if (startLocation != null) {
					string text = panel.ControlDictionary[target].Text;
					if (textBoxEditMode == TextBoxEditMode.EditRawProperty)
						text = MSBuildInternals.Unescape(text);
					startLocation = FileUtility.GetAbsolutePath(startLocation, text);
				}
				
				using (FolderBrowserDialog fdiag = FileService.CreateFolderBrowserDialog(description, startLocation)) {
					if (fdiag.ShowDialog() == DialogResult.OK) {
						string path = fdiag.SelectedPath;
						if (panel.baseDirectory != null) {
							path = FileUtility.GetRelativePath(panel.baseDirectory, path);
						}
						if (!path.EndsWith("\\") && !path.EndsWith("/"))
							path += "\\";
						if (textBoxEditMode == TextBoxEditMode.EditEvaluatedProperty) {
							panel.ControlDictionary[target].Text = path;
						} else {
							panel.ControlDictionary[target].Text = MSBuildInternals.Escape(path);
						}
					}
				}
			}
		*/
		
		/// <summary>
		/// Open File
		/// </summary>
		/// <param name="filter" or String.Empty></param>
		/// <returns FileName></returns>
		public static string OpenFile (string filter)
		{
			var dialog = new OpenFileDialog();
			if (String.IsNullOrEmpty(filter)) {
				dialog.Filter = StringParser.Parse(filter);
			}
			
			if (dialog.ShowDialog() ?? false) {
				return dialog.FileName;
			}
			return string.Empty;
		}
	}
}
