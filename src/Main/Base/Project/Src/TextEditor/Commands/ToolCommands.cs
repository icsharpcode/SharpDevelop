// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Xsl;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Commands
{
	public class QuickDocumentation : AbstractMenuCommand
	{
		public override void Run()
		{
			IViewContent viewContent = WorkbenchSingleton.Workbench.ActiveViewContent;

			if (viewContent == null || !(viewContent is ITextEditorControlProvider)) {
				return;
			}
			TextEditorControl textAreaControl = ((ITextEditorControlProvider)viewContent).TextEditorControl;

			int startLine = textAreaControl.Document.GetLineNumberForOffset(textAreaControl.ActiveTextAreaControl.Caret.Offset);
			int endLine   = startLine;

			LineSegment line = textAreaControl.Document.GetLineSegment(startLine);
			string curLine   = textAreaControl.Document.GetText(line.Offset, line.Length).Trim();
			if (!curLine.StartsWith("///") && ! curLine.StartsWith("'''")) {
				return;
			}

			while (startLine > 0) {
				line    = textAreaControl.Document.GetLineSegment(startLine);
				curLine = textAreaControl.Document.GetText(line.Offset, line.Length).Trim();
				if (curLine.StartsWith("///") || curLine.StartsWith("'''")) {
					--startLine;
				} else {
					break;
				}
			}

			while (endLine < textAreaControl.Document.TotalNumberOfLines - 1) {
				line    = textAreaControl.Document.GetLineSegment(endLine);
				curLine = textAreaControl.Document.GetText(line.Offset, line.Length).Trim();
				if (curLine.StartsWith("///") || curLine.StartsWith("'''")) {
					++endLine;
				} else {
					break;
				}
			}

			StringBuilder documentation = new StringBuilder();
			for (int lineNr = startLine + 1; lineNr < endLine; ++lineNr) {
				line    = textAreaControl.Document.GetLineSegment(lineNr);
				curLine = textAreaControl.Document.GetText(line.Offset, line.Length).Trim();
				if(curLine.StartsWith("///")) {
				   	documentation.Append(curLine.Substring(3));
				}
				else
				{
					documentation.Append(curLine.Substring(2));
				}
				documentation.Append('\n');
			}
			string xml  = "<member>" + documentation.ToString() + "</member>";
			string html = String.Empty;

			try
			{
				XslCompiledTransform t = new XslCompiledTransform();
				t.Load(Path.Combine(Path.Combine(PropertyService.DataDirectory, "ConversionStyleSheets"), "ShowXmlDocumentation.xsl"));
				XmlDocument doc = new XmlDocument();
				doc.LoadXml(xml);
				StringBuilder sb = new StringBuilder();
				TextWriter textWriter = new StringWriter(sb);
				XmlWriter writer = new XmlTextWriter(textWriter);
				t.Transform(doc, writer);
				html = sb.ToString();
				textWriter.Close();
				writer.Close();
			}
			catch (Exception e)
			{
				MessageBox.Show(e.ToString());
			}
			new ToolWindowForm(textAreaControl, html).Show();
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
}
