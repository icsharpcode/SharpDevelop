// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Threading;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text;
using System.Xml;
using System.Xml.Xsl;

using ICSharpCode.Core;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.TextEditor;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Commands
{
	public class ShowColorDialog : AbstractMenuCommand
	{
		public override void Run()
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			
			if (window == null || !(window.ViewContent is ITextEditorControlProvider)) {
				return;
			}
			TextEditorControl textarea = ((ITextEditorControlProvider)window.ViewContent).TextEditorControl;
			
			using (ColorDialog cd = new ColorDialog()) {
				if (cd.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm) == DialogResult.OK) {
					string colorstr = "#" + cd.Color.ToArgb().ToString("X");
					if (cd.Color.IsKnownColor) {
						colorstr = cd.Color.ToKnownColor().ToString();
					}
					
					textarea.Document.Insert(textarea.ActiveTextAreaControl.Caret.Offset, colorstr);
					int lineNumber = textarea.Document.GetLineNumberForOffset(textarea.ActiveTextAreaControl.Caret.Offset);
					textarea.ActiveTextAreaControl.Caret.Column += colorstr.Length;
					textarea.Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.SingleLine, new Point(0, lineNumber)));
					textarea.Document.CommitUpdate();
				}
			}
		}
	}
	
	public class QuickDocumentation : AbstractMenuCommand
	{
		public override void Run()
		{
//	TODO: New Projectfile system.					

//			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
//			
//			if (window == null || !(window.ViewContent is ITextEditorControlProvider)) {
//				return;
//			}
//			TextEditorControl textAreaControl = ((ITextEditorControlProvider)window.ViewContent).TextEditorControl;
//			
//			int startLine = textAreaControl.Document.GetLineNumberForOffset(textAreaControl.ActiveTextAreaControl.Caret.Offset);
//			int endLine   = startLine;
//			
//			LineSegment line = textAreaControl.Document.GetLineSegment(startLine);
//			string curLine   = textAreaControl.Document.GetText(line.Offset, line.Length).Trim();
//			if (!curLine.StartsWith("///")) {
//				return;
//			}
//			
//			while (startLine > 0) {
//				line    = textAreaControl.Document.GetLineSegment(startLine);
//				curLine = textAreaControl.Document.GetText(line.Offset, line.Length).Trim();
//				if (curLine.StartsWith("///")) {
//					--startLine;
//				} else {
//					break;
//				}
//			}
//			
//			while (endLine < textAreaControl.Document.TotalNumberOfLines - 1) {
//				line    = textAreaControl.Document.GetLineSegment(endLine);
//				curLine = textAreaControl.Document.GetText(line.Offset, line.Length).Trim();
//				if (curLine.StartsWith("///")) {
//					++endLine;
//				} else {
//					break;
//				}
//			}
//			
//			StringBuilder documentation = new StringBuilder();
//			for (int lineNr = startLine + 1; lineNr < endLine; ++lineNr) {
//				line    = textAreaControl.Document.GetLineSegment(lineNr);
//				curLine = textAreaControl.Document.GetText(line.Offset, line.Length).Trim();
//				documentation.Append(curLine.Substring(3));
//				documentation.Append('\n');
//			}
//			string xml  = "<member>" + documentation.ToString() + "</member>";
//			
//			string html = String.Empty;
//			
//			try {
//				
//				
//				html = ICSharpCode.SharpDevelop.Internal.Project.ConvertXml.ConvertData(xml,
//				                   PropertyService.DataDirectory +
//				                   Path.DirectorySeparatorChar + "ConversionStyleSheets" +
//				                   Path.DirectorySeparatorChar + "ShowXmlDocumentation.xsl",
//				                   null);
//			} catch (Exception e) {
//				MessageBox.Show(e.ToString());
//			}
//			new ToolWindowForm(textAreaControl, html).Show();
		}
		
		class ToolWindowForm : Form
		{
			public ToolWindowForm(TextEditorControl textEditorControl, string html)
			{
				Point caretPos  = textEditorControl.ActiveTextAreaControl.Caret.ScreenPosition;
				Point visualPos = new Point(Math.Min(Math.Max(caretPos.X, textEditorControl.ActiveTextAreaControl.TextArea.TextView.DrawingPosition.Left), textEditorControl.ActiveTextAreaControl.TextArea.TextView.DrawingPosition.Right), 
				                            Math.Min(Math.Max(caretPos.Y, textEditorControl.ActiveTextAreaControl.TextArea.TextView.DrawingPosition.Top), textEditorControl.ActiveTextAreaControl.TextArea.TextView.DrawingPosition.Bottom));
				
				Location = textEditorControl.ActiveTextAreaControl.TextArea.PointToScreen(visualPos);
				
				
				WebBrowser hc = new WebBrowser();
				hc.DocumentText = html;
				
//		TODO: Stylesheet.		
//				hc.CascadingStyleSheet = PropertyService.DataDirectory +
//				                   Path.DirectorySeparatorChar + "resources" +
//				                   Path.DirectorySeparatorChar + "css" +
//				                   Path.DirectorySeparatorChar + "MsdnHelp.css";
				hc.Dock = DockStyle.Fill;
				hc.Navigating += new WebBrowserNavigatingEventHandler(BrowserNavigateCancel);
				Controls.Add(hc);
								
				ShowInTaskbar   = false;
				FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
				StartPosition   = FormStartPosition.Manual;
			}
			
			void BrowserNavigateCancel(object sender, WebBrowserNavigatingEventArgs e)
			{
				e.Cancel = true;
			}
			
			protected override void OnDeactivate(EventArgs e)
			{
				Close();
			}
			
			protected override bool ProcessDialogKey(Keys keyData)
			{
				if (keyData == Keys.Escape) {
					Close();
					return true;
				}
				return base.ProcessDialogKey(keyData);
			}
			
		}
	}
	
	public class SplitTextEditor : AbstractMenuCommand
	{
		public override void Run()
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			
			if (window == null || !(window.ViewContent is ITextEditorControlProvider)) {
				return;
			}
			TextEditorControl textEditorControl = ((ITextEditorControlProvider)window.ViewContent).TextEditorControl;
			if (textEditorControl != null) {
				textEditorControl.Split();
			}
		}
	}

}
