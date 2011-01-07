// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Gui;
using System;
using System.Windows.Forms;
using ICSharpCode.Core;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Allows the user to browse for an XSLT stylesheet.  The selected
	/// stylesheet will be assigned to the currently open xml file.
	/// </summary>
	public class AssignStylesheetCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			// Get active xml document.
			XmlView xmlView = XmlView.ActiveXmlView;
			
			if (xmlView != null) {
				// Prompt user for filename.
				string stylesheetFileName = BrowseForStylesheetFile();

				// Assign stylesheet.
				if (stylesheetFileName != null) {
					xmlView.StylesheetFileName = stylesheetFileName;
				}
			}
		}
		
		public static string BrowseForStylesheetFile()
		{
			using (OpenFileDialog dialog = new OpenFileDialog()) {
				dialog.AddExtension = true;
				dialog.Multiselect = false;
				dialog.CheckFileExists = true;
				dialog.Title = ResourceService.GetString("ICSharpCode.XmlEditor.AssignXSLT.Title");
				
				AddInTreeNode node = AddInTree.GetTreeNode("/SharpDevelop/Workbench/FileFilter");
				if (node != null) {
					string xmlFileFilter = GetFileFilter(node, "Xml");
					string allFilesFilter = GetFileFilter(node, "AllFiles");
					string xslFileFilter = GetFileFilter(node, "Xsl");
					
					dialog.Filter = String.Join("|", xslFileFilter, xmlFileFilter, allFilesFilter);
					dialog.FilterIndex = 1;
				}
				
				if (dialog.ShowDialog(WorkbenchSingleton.MainWin32Window) == DialogResult.OK) {
					return dialog.FileName;
				}
			}
			
			return null;
		}
		
		static string GetFileFilter(AddInTreeNode node, string filterName)
		{
			FileFilterDescriptor fileFilter = node.BuildChildItem(filterName, null, null) as FileFilterDescriptor;
			if (fileFilter != null) {
				return fileFilter.ToString();
			}
			return String.Empty;
		}
	}
}
