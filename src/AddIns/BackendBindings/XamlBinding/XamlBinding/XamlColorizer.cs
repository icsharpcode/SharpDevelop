// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Linq;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.XmlEditor;
using System.Diagnostics;

namespace ICSharpCode.XamlBinding
{
	public class XamlColorizer : DocumentColorizingTransformer
	{
		static readonly XamlColorizerSettings defaultSettings = new XamlColorizerSettings();
		XamlColorizerSettings settings = defaultSettings;
		string fileContent;
		string fileName;
		ParseInformation parseInfo;
		XamlResolver resolver = new XamlResolver();
		
		public IViewContent Content { get; set; }
		
		public XamlColorizer(IViewContent content)
		{
			this.Content = content;
		}
		
		protected override void Colorize(ITextRunConstructionContext context)
		{
			IFileDocumentProvider document = this.Content as IFileDocumentProvider;
			
			if (document == null)
				return;
			
			this.parseInfo = ParserService.GetParseInformation(Content.PrimaryFileName);
			this.fileContent = document.GetDocumentForFile(this.Content.PrimaryFile).CreateSnapshot().Text;
			this.fileName = this.Content.PrimaryFileName;
			
			base.Colorize(context);
		}
		
		protected override void ColorizeLine(DocumentLine line)
		{
			if (!line.IsDeleted) {
				HighlightingInfo[] infos = GetInfoForLine(line);
				
				foreach (HighlightingInfo info in infos) {
					MemberResolveResult rr = resolver.Resolve(info.GetExpressionResult(), parseInfo, fileContent) as MemberResolveResult;
					IMember member = (rr != null) ? rr.ResolvedMember : null;
					
					if (member != null) {
						if (member is IEvent)
							ChangeLinePart(line.Offset + info.StartOffset, line.Offset + info.EndOffset, HighlightEvent);
						else
							ChangeLinePart(line.Offset + info.StartOffset, line.Offset + info.EndOffset, HighlightProperty);
					} else {
						if (info.Token.StartsWith("xmlns"))
							ChangeLinePart(line.Offset + info.StartOffset, line.Offset + info.EndOffset, HighlightNamespaceDeclaration);
					}
				}
			}
		}
		
		HighlightingInfo[] GetInfoForLine(DocumentLine line)
		{
			int index = -1;
			int ltCharIndex = -1;
			XamlContext context = null;
			List<HighlightingInfo> infos = new List<HighlightingInfo>();
			
			do {
				index = line.Text.IndexOf('=', index + 1);
				if (index > -1) {
					context = CompletionDataHelper.ResolveContext(this.fileContent, this.fileName, line.LineNumber, index + 1);
					if (!string.IsNullOrEmpty(context.AttributeName)) {
						int startIndex = line.Text.Substring(0, index).LastIndexOf(context.AttributeName);
						infos.Add(new HighlightingInfo(context.AttributeName, startIndex, startIndex + context.AttributeName.Length, line.Offset, context));
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
		
		void HighlightNamespaceDeclaration(VisualLineElement element)
		{
			element.TextRunProperties.SetForegroundBrush(settings.NamespaceDeclarationForegroundBrush);
			element.TextRunProperties.SetBackgroundBrush(settings.NamespaceDeclarationBackgroundBrush);
		}
		
		struct HighlightingInfo
		{
			public static readonly HighlightingInfo Empty = new HighlightingInfo(string.Empty, 0, 0, 0, new XamlContext());
			
			string token;
			int startOffset;
			int endOffset;
			int lineOffset;
			XamlContext context;
			
			public HighlightingInfo(string token, int startOffset, int endOffset, int lineOffset, XamlContext context)
			{
				this.token = token;
				this.startOffset = startOffset;
				this.endOffset = endOffset;
				this.lineOffset = lineOffset;
				this.context = context;
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
			
			public XamlContext Context {
				get { return context; }
			}
			
			public ExpressionResult GetExpressionResult() {
				return new ExpressionResult(token, context);
			}
		}
	}
}
