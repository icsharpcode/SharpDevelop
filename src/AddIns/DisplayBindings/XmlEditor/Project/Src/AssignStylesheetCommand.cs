// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

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
				
				if (dialog.ShowDialog(SD.WinForms.MainWin32Window) == DialogResult.OK) {
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
