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
	public class ShowColorDialog : AbstractMenuCommand
	{
		public override void Run()
		{
			IViewContent viewContent = WorkbenchSingleton.Workbench.ActiveViewContent;
			
			if (viewContent == null || !(viewContent is ITextEditorControlProvider)) {
				return;
			}
			TextEditorControl textarea = ((ITextEditorControlProvider)viewContent).TextEditorControl;
			
			using (SharpDevelopColorDialog cd = new SharpDevelopColorDialog()) {
				if (cd.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm) == DialogResult.OK) {
					string ext = Path.GetExtension(textarea.FileName).ToLowerInvariant();
					string colorstr;
					if (ext == ".cs" || ext == ".vb" || ext == ".boo") {
						if (cd.Color.IsKnownColor) {
							colorstr = "Color." + cd.Color.ToKnownColor().ToString();
						} else if (cd.Color.A < 255) {
							colorstr = "Color.FromArgb(0x" + cd.Color.ToArgb().ToString("x") + ")";
						} else {
							colorstr = string.Format("Color.FromArgb({0}, {1}, {2})", cd.Color.R, cd.Color.G, cd.Color.B);
						}
					} else {
						if (cd.Color.IsKnownColor) {
							colorstr = cd.Color.ToKnownColor().ToString();
						} else if (cd.Color.A < 255) {
							colorstr = "#" + cd.Color.ToArgb().ToString("X");
						} else {
							colorstr = string.Format("#{0:X2}{1:X2}{2:X2}", cd.Color.R, cd.Color.G, cd.Color.B);
						}
					}
					
					textarea.Document.Insert(textarea.ActiveTextAreaControl.Caret.Offset, colorstr);
					int lineNumber = textarea.Document.GetLineNumberForOffset(textarea.ActiveTextAreaControl.Caret.Offset);
					textarea.ActiveTextAreaControl.Caret.Column += colorstr.Length;
					textarea.Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.SingleLine, new TextLocation(0, lineNumber)));
					textarea.Document.CommitUpdate();
				}
			}
		}
	}
	
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
	
	public class SplitTextEditor : AbstractMenuCommand
	{
		public override void Run()
		{
			IViewContent viewContent = WorkbenchSingleton.Workbench.ActiveViewContent;
			
			if (viewContent == null || !(viewContent is ITextEditorControlProvider)) {
				return;
			}
			TextEditorControl textEditorControl = ((ITextEditorControlProvider)viewContent).TextEditorControl;
			if (textEditorControl != null) {
				textEditorControl.Split();
			}
		}
	}

	public class InsertGuidCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			IViewContent viewContent = WorkbenchSingleton.Workbench.ActiveViewContent;
			if (viewContent == null || !(viewContent is ITextEditorControlProvider)) {
				return;
			}
			
			TextEditorControl textEditor = ((ITextEditorControlProvider)viewContent).TextEditorControl;
			if (textEditor == null) {
				return;
			}
			
			string newGuid = Guid.NewGuid().ToString().ToUpperInvariant();
			
			textEditor.ActiveTextAreaControl.TextArea.InsertString(newGuid);
		}
	}
}
