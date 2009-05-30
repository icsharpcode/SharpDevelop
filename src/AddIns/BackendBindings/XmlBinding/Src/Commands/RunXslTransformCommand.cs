// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 2313 $</version>
// </file>

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
			XmlView properties = XmlView.ForViewContent(WorkbenchSingleton.Workbench.ActiveViewContent);
			
			if (properties != null) {
				// Check to see if this view is actually a referenced stylesheet.
				if (!string.IsNullOrEmpty(properties.File.FileName)) {
					
					XmlView assocFile = GetAssociatedXmlView(properties.File.FileName);
					if (assocFile != null) {
						LoggingService.Debug("Using associated xml file.");
						properties = assocFile;
					}
				}
				
				// Assign a stylesheet.
				if (properties.StylesheetFileName == null) {
					properties.StylesheetFileName = AssignStylesheetCommand.BrowseForStylesheetFile();
				}
				
				if (properties.StylesheetFileName != null) {
					try {
						properties.RunXslTransform(GetStylesheetContent(properties.StylesheetFileName));
					} catch (Exception ex) {
						MessageService.ShowError(ex);
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
				XmlView prop = XmlView.ForViewContent(content);
				if (prop != null && !string.IsNullOrEmpty(prop.StylesheetFileName)) {
					if (FileUtility.IsEqualFileName(prop.StylesheetFileName, stylesheetFileName)) {
						return prop;
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
