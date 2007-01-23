// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
				
				if (xmlView is XslOutputView) {
					return;
				}
				
				// Check to see if this view is actually a referenced stylesheet.
				if (!string.IsNullOrEmpty(xmlView.PrimaryFileName)) {
					XmlView associatedXmlView = GetAssociatedXmlView(xmlView.PrimaryFileName);
					if (associatedXmlView != null) {
						LoggingService.Debug("Using associated xml view.");
						xmlView = associatedXmlView;
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
						MessageService.ShowError(ex);
					}
				}
			}
		}
		
		/// <summary>
		/// Gets the xml view that is currently referencing the
		/// specified stylesheet view.
		/// </summary>
		XmlView GetAssociatedXmlView(string stylesheetFileName)
		{
			foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection) {
				XmlView view = content as XmlView;
				if (view != null && view.StylesheetFileName != null) {
					if (FileUtility.IsEqualFileName(view.StylesheetFileName, stylesheetFileName)) {
						return view;
					}
				}
			}
			return null;
		}
		
		string GetStylesheetContent(string fileName)
		{
			// File already open?
			XmlView view = FileService.GetOpenFile(fileName) as XmlView;
			if (view != null) {
				return view.Text;
			}
			
			// Read in file contents.
			StreamReader reader = new StreamReader(fileName, true);
			return reader.ReadToEnd();
		}
	}
}
