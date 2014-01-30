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

using ICSharpCode.SharpDevelop.Editor;
using System;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Runs an XSL transform on an xml document.
	/// </summary>
	public class RunXslTransformCommand : AbstractCommand
	{
		/// <summary>
		/// Runs the transform on the xml file using the assigned stylesheet.
		/// If no stylesheet is assigned the user is prompted to choose one.
		/// If the view represents a stylesheet that is currently assigned to an
		/// opened document then run the transform on that document.
		/// </summary>
		public override void Run()
		{
			XmlView xmlView = XmlView.ActiveXmlView;
			
			if (xmlView != null) {
				// Check to see if this view is actually a referenced stylesheet.
				if (!string.IsNullOrEmpty(xmlView.File.FileName)) {
					
					XmlView assocFile = GetAssociatedXmlView(xmlView.File.FileName);
					if (assocFile != null) {
						LoggingService.Debug("Using associated xml file.");
						xmlView = assocFile;
					}
				}
				
				// Assign a stylesheet.
				if (xmlView.StylesheetFileName == null) {
					xmlView.StylesheetFileName = AssignStylesheetCommand.BrowseForStylesheetFile();
				}
				
				if (xmlView.StylesheetFileName != null) {
					try {
						xmlView.RunXslTransform(GetStylesheetContent(xmlView.StylesheetFileName));
					} catch (Exception ex) {
						MessageService.ShowException(ex);
					}
				}
			}
		}
		
		/// <summary>
		/// Gets the xml view that is currently referencing the
		/// specified stylesheet view.
		/// </summary>
		static XmlView GetAssociatedXmlView(string stylesheetFileName)
		{
			foreach (IViewContent content in SD.Workbench.ViewContentCollection) {
				XmlView view = XmlView.ForViewContent(content);
				if (view != null && !string.IsNullOrEmpty(view.StylesheetFileName)) {
					if (FileUtility.IsEqualFileName(view.StylesheetFileName, stylesheetFileName)) {
						return view;
					}
				}
			}
			return null;
		}
		
		static string GetStylesheetContent(string fileName)
		{
			return SD.FileService.GetFileContent(fileName).Text;
		}
	}
}
