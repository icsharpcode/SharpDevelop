// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: -1 $</version>
// </file>

using ICSharpCode.SharpDevelop.Editor;
using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Displays the resulting output from an XSL transform.
	/// </summary>
	public static class XslOutputView
	{
		static IViewContent instance;
		static ITextEditor editor;
		
		public static ITextEditor EditorInstance {
			get {
				if (editor == null) {
					instance = FileService.NewFile("xslOutput.xml", "");
					editor = (instance as ITextEditorProvider).TextEditor;
					instance.Disposed += new EventHandler(InstanceDisposed);
				}
				
				return editor;
			}
		}
		
		public static IViewContent Instance {
			get {
				if (instance == null) {
					instance = FileService.NewFile("xslOutput.xml", "");
					editor = (instance as ITextEditorProvider).TextEditor;
					instance.Disposed += new EventHandler(InstanceDisposed);
				} else {
					instance.WorkbenchWindow.SelectWindow();
				}
				
				return instance;
			}
		}

		static void InstanceDisposed(object sender, EventArgs e)
		{
			instance = null;
			editor = null;
		}
	}
}
