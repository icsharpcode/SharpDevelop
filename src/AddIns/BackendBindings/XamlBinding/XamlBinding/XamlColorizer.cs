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
			
			this.fileContent = document.GetDocumentForFile(this.Content.PrimaryFile).CreateSnapshot().Text;
			this.fileName = this.Content.PrimaryFileName;
			
			base.Colorize(context);
		}
		
		protected override void ColorizeLine(DocumentLine line)
		{
			Stopwatch watch = new Stopwatch();
			watch.Start();
			
			XamlResolver resolver = new XamlResolver();
			
			if (!line.IsDeleted) {
				foreach (HighlightingInfo info in GetInfoForLine(fileContent, fileName, (string)line.Text.Clone(), line.LineNumber, line.Offset)) {
					MemberResolveResult rr = resolver.Resolve(info.GetExpressionResult(), info.Context.ParseInformation, fileContent) as MemberResolveResult;
					IMember member = (rr != null) ? rr.ResolvedMember : null;
					Colorize(member, info, line.Offset);
				}
			}
			
			watch.Stop();
			Core.LoggingService.Debug("ColorizeLine line: " + line.LineNumber + " took: " + watch.ElapsedMilliseconds + "ms");
		}
		
		void Colorize(IMember member, HighlightingInfo info, int offset)
		{
			try {
				if (member != null) {
					if (member is IEvent)
						ChangeLinePart(offset + info.StartOffset, offset + info.EndOffset, HighlightEvent);
					else
						ChangeLinePart(offset + info.StartOffset, offset + info.EndOffset, HighlightProperty);
				} else {
					if (info.Token.StartsWith("xmlns"))
						ChangeLinePart(offset + info.StartOffset, offset + info.EndOffset, HighlightNamespaceDeclaration);
				}
			} catch (Exception e) {
				Debug.Print(e.ToString());
			}
		}
		
		static ParallelQuery<HighlightingInfo> GetInfoForLine(string fileContent, string fileName, string lineText, int line, int offset)
		{
			Stopwatch watch = new Stopwatch();
			watch.Start();
			
			List<int> indices = new List<int>();
			List<HighlightingInfo> infos = new List<HighlightingInfo>();
			
			int cIndex = lineText.IndexOf('=');
			
			while (cIndex > -1) {
				indices.Add(cIndex);
				cIndex = lineText.IndexOf('=', cIndex + 1);
			}
			
			return indices.AsParallel()
				.Select(index => new { Context = CompletionDataHelper.ResolveContext(fileContent, fileName, line, index + 1), Index = index, Offset = offset })
				.Where(item => !string.IsNullOrEmpty(item.Context.AttributeName))
				.Select(context => GetInfo(context.Index, lineText, line, context.Context));
		}
		
		static HighlightingInfo GetInfo(int index, string lineText, int line, XamlContext context)
		{
			int startIndex = lineText.Substring(0, index).LastIndexOf(context.AttributeName);
			return new HighlightingInfo(context.AttributeName, startIndex, startIndex + context.AttributeName.Length, line, context);
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
