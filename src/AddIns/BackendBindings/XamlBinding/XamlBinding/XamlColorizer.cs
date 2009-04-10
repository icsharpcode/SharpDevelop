// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Gui;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.XmlEditor;

namespace ICSharpCode.XamlBinding
{
	public class XamlColorizer : DocumentColorizingTransformer
	{
		static readonly XamlColorizerSettings defaultSettings = new XamlColorizerSettings();
		XamlColorizerSettings settings = defaultSettings;
		string fileContent;
		
		public IViewContent Content { get; set; }
		
		public XamlColorizer(IViewContent content)
		{
			this.Content = content;
		}
		
		protected override void ColorizeLine(DocumentLine line)
		{
			ParseInformation parseInfo = ParserService.GetParseInformation(Content.PrimaryFileName);
			XamlResolver resolver = new XamlResolver();
			
			IFileDocumentProvider document = this.Content as IFileDocumentProvider;
			
			if (document == null)
				return;
			
			this.fileContent = document.GetDocumentForFile(this.Content.PrimaryFile).Text;
			
			if (!line.IsDeleted) {
				HighlightingInfo[] infos = GetInfoForLine(line);
				
				foreach (HighlightingInfo info in infos) {
					ResolveResult rr = resolver.Resolve(info.GetExpressionResult(), parseInfo, fileContent);
					IReturnType type = (rr != null) ? rr.ResolvedType : null;
					if (type != null) {
						//if (type.IsGenericReturnType) 
						ChangeLinePart(line.Offset + info.StartOffset, line.Offset + info.EndOffset, HighlightProperty);
					}
				}
			}
		}
		
		struct HighlightingInfo
		{
			string token;
			int startOffset;
			int endOffset;
			int lineOffset;
			XmlElementPath path;
			
			public HighlightingInfo(string token, int startOffset, int endOffset, int lineOffset, XmlElementPath path)
			{
				this.token = token;
				this.startOffset = startOffset;
				this.endOffset = endOffset;
				this.lineOffset = lineOffset;
				this.path = path;
			}
			
			public string Token {
				get { return token; }
			}
			
			public int StartOffset {
				get { return startOffset; }
			}
			
			public int EndOffset {
				get { return endOffset; }
			}
			
			public int LineOffset {
				get { return lineOffset; }
			}
			
			public XmlElementPath Path {
				get { return path; }
			}
			
			public ExpressionResult GetExpressionResult()
			{
				return new ExpressionResult(token, new XamlExpressionContext(path, token, false));
			}
		}
		
		HighlightingInfo[] GetInfoForLine(DocumentLine line)
		{
			int index = -1;
			List<HighlightingInfo> infos = new List<HighlightingInfo>();
			
			do {
				index = line.Text.IndexOf('=', index + 1);
				if (index > -1) {
					string expr = XmlParser.GetAttributeNameAtIndex(this.fileContent, index + line.Offset);
					XmlElementPath path = XmlParser.GetActiveElementStartPathAtIndex(this.fileContent, index + line.Offset);
					if (!string.IsNullOrEmpty(expr) && path != null && path.Elements.Count > 0) {
						int startIndex = line.Text.Substring(0, index).LastIndexOf(expr);
						infos.Add(new HighlightingInfo(expr, startIndex, startIndex + expr.Length, line.Offset, path));
					}
				}
			} while (index > -1);
			
			return infos.ToArray();
		}
		
		void HighlightProperty(VisualLineElement element)
		{
			element.TextRunProperties.SetForegroundBrush(settings.PropertyForegroundBrush);
			element.TextRunProperties.SetBackgroundBrush(settings.PropertyBackgroundBrush);
		}
		
		void HighlightEvent(VisualLineElement element)
		{
			element.TextRunProperties.SetForegroundBrush(settings.EventForegroundBrush);
			element.TextRunProperties.SetBackgroundBrush(settings.EventBackgroundBrush);
		}
	}

	public static class Extensions
	{
		public static string[] Split(this string s, StringSplitOptions options, params char[] delimiters)
		{
			return s.Split(delimiters, options);
		}
	}

	public class XamlColorizerSettings
	{
		public Brush PropertyForegroundBrush { get; set; }
		public Brush PropertyBackgroundBrush { get; set; }
		
		public Brush EventForegroundBrush { get; set; }
		public Brush EventBackgroundBrush { get; set; }
		
		public XamlColorizerSettings()
		{
			this.PropertyBackgroundBrush = Brushes.Transparent;
			this.PropertyForegroundBrush = Brushes.Red;
			
			this.EventBackgroundBrush = Brushes.Transparent;
			this.EventForegroundBrush = Brushes.Green;
		}
	}
}
