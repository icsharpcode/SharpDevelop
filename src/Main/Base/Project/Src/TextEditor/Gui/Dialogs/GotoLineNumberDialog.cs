// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Resources;

using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.Core;

using ICSharpCode.TextEditor;
using ICSharpCode.SharpDevelop.Gui.XmlForms;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class GotoLineNumberDialog : BaseSharpDevelopForm
	{
		public static bool IsVisible = false;
		
		public GotoLineNumberDialog()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("Resources.GotoLineNumberDialog.xfrm"));
			ControlDictionary["okButton"].Click     += new EventHandler(closeEvent);
			ControlDictionary["cancelButton"].Click += new EventHandler(cancelEvent);
			Owner = (Form)WorkbenchSingleton.Workbench;
			Icon = null;
		}
		
		protected override void OnClosed(System.EventArgs e)
		{
			IsVisible = false;
			base.OnClosed(e);
		}
		
		void cancelEvent(object sender, EventArgs e)
		{
			IsVisible = false;
			Close();
		}
		
		void closeEvent(object sender, EventArgs e)
		{
			try {
				IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
				
				
				if (window != null && window.ViewContent is ITextEditorControlProvider) {
					TextEditorControl textarea = ((ITextEditorControlProvider)window.ViewContent).TextEditorControl;
				
					int i = Math.Min(textarea.Document.TotalNumberOfLines, Math.Max(1, Int32.Parse(ControlDictionary["lineNumberTextBox"].Text)));
					textarea.ActiveTextAreaControl.Caret.Line = i - 1;
					textarea.ActiveTextAreaControl.ScrollToCaret();
				}
			} catch (Exception) {
				
			} finally {
				IsVisible = false;
				Close();
			}
		}
	}
}
