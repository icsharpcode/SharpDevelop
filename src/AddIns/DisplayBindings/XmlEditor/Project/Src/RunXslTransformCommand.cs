// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
			foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection) {
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
			// File already open?
			ITextEditorProvider view = FileService.GetOpenFile(fileName) as ITextEditorProvider;
			if (view != null) {
				return view.TextEditor.Document.Text;
			}
			
			// Read in file contents.
			StreamReader reader = new StreamReader(fileName, true);
			return reader.ReadToEnd();
		}
	}
}
