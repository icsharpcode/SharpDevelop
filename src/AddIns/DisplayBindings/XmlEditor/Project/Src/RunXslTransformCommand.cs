// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using System;
using System.IO;
using System.Windows.Forms;

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
			XmlView xmlView = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ActiveViewContent as XmlView;
			if (xmlView != null) {
				
				if (xmlView is XslOutputView) {
					return;
				}
				
				// Check to see if this view is actually a referenced stylesheet.
				if (xmlView.FileName != null && xmlView.FileName.Length > 0) {
					XmlView associatedXmlView = GetAssociatedXmlView(xmlView.FileName);
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
			IWorkbenchWindow window = FileService.GetOpenFile(fileName);
			if (window != null) {
				XmlView view = window.ActiveViewContent as XmlView;
				if (view != null) {
					return view.Text;
				} else {
					LoggingService.Warn("Stylesheet file not opened in xml editor.");
				}
			}
			
			// Read in file contents.
			StreamReader reader = new StreamReader(fileName, true);
			return reader.ReadToEnd();
		}
	}
}
