// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
					
					string xmlFileFilter = (string)node.BuildChildItem("Xml", null, null);
					string allFilesFilter = (string)node.BuildChildItem("AllFiles", null, null);
					string xslFileFilter = (string)node.BuildChildItem("Xsl", null, null);
					
					dialog.Filter = String.Concat(xslFileFilter, "|", xmlFileFilter, "|", allFilesFilter);
					dialog.FilterIndex = 1;
				}
				
				if (dialog.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm) == DialogResult.OK) {
					return dialog.FileName;
				}
			}
			
			return null;
		}
	}
}
