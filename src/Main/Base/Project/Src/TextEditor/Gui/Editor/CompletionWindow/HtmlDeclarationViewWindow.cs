//// <file>
////     <copyright see="prj:///doc/copyright.txt"/>
////     <license see="prj:///doc/license.txt"/>
////     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
////     <version value="$version"/>
//// </file>
//
//using System;
//using System.IO;
//using System.Drawing;
//using System.Windows.Forms;
//using System.Reflection;
//using System.Collections;
//
//using ICSharpCode.TextEditor.Document;
//using ICSharpCode.TextEditor.Util;
//using ICSharpCode.TextEditor;
//using ICSharpCode.SharpDevelop.Gui.HtmlControl;
//using ICSharpCode.Core;
//
//namespace ICSharpCode.TextEditor.Gui.CompletionWindow
//{
//	public class HtmlDeclarationViewWindow : Form, IDeclarationViewWindow
//	{
//		string description = String.Empty;
//		HtmlControl hc  = new HtmlControl();
//		public Form DeclarationViewForm {
//			get {
//				return this;
//			}
//		}
//		public string Description {
//			get {
//				return description;
//			}
//			set {
//				description = value;
//				hc.Html = "<HTML><BODY>" + description + "</BODY></HTML>";
//			}
//		}
//		
//		public HtmlDeclarationViewWindow()
//		{
////				
//				
//				hc.CascadingStyleSheet = PropertyService.DataDirectory +
//				                   Path.DirectorySeparatorChar + "resources" +
//				                   Path.DirectorySeparatorChar + "css" +
//				                   Path.DirectorySeparatorChar + "MsdnHelp.css";
//			hc.Dock = DockStyle.Fill;
//			hc.BeforeNavigate += new BrowserNavigateEventHandler(BrowserNavigateCancel);
//			Controls.Add(hc);
//			
//			StartPosition   = FormStartPosition.Manual;
//			FormBorderStyle = FormBorderStyle.None;
//			TopMost         = true;
//			ShowInTaskbar   = false;
//			Size            = new Size(200, 200);
//		}
////		
//		void BrowserNavigateCancel(object sender, BrowserNavigateEventArgs e)
//		{
//			e.Cancel = true;
//		}
//		
//		public void CloseDeclarationViewWindow()
//		{
//			Close();
//			Dispose();
//		}
//		
////		protected override void OnPaint(PaintEventArgs pe)
////		{
////			TipPainterTools.DrawHelpTipFromCombinedDescription
////				(this, pe.Graphics, Font, null, description);
////		}
////		
////		protected override void OnPaintBackground(PaintEventArgs pe)
////		{
////			if (description != null && description.Length > 0) {
////				pe.Graphics.FillRectangle(SystemBrushes.Info, pe.ClipRectangle);
////			}
////		}
//	}
//}
