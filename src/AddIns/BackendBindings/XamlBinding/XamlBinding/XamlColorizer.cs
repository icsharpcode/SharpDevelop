// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.XmlEditor;
using System.Windows.Threading;

namespace ICSharpCode.XamlBinding
{
	using Tasks = System.Threading.Tasks;
	
	public class XamlColorizer : DocumentColorizingTransformer, ILineTracker
	{
		static readonly XamlColorizerSettings defaultSettings = new XamlColorizerSettings();
		
		public struct Highlight {
			public IMember Member { get; set; }
			public HighlightingInfo Info { get; set; }
		}
		
		public sealed class HighlightTask {
			// input
			public string FileContent { get; private set; }
			public string FileName { get; private set; }
			public string LineText { get; private set; }
			public int LineNumber { get; private set; }
			public int Offset { get; private set; }
			
			TextView textView;
			
			public HighlightTask(string fileContent, string fileName, DocumentLine currentLine, TextView textView)
			{
				this.FileContent = fileContent;
				this.FileName = fileName;
				this.task = new System.Threading.Tasks.Task(Process);
				this.textView = textView;
				this.LineNumber = currentLine.LineNumber;
				this.LineText = currentLine.Text;
				this.Offset = currentLine.Offset;
			}
			
			IList<Highlight> results;
			
			// output
			public IList<Highlight> GetResults()
			{
				return results;
			}
			
			Tasks.Task task;
			
			public void Start()
			{
				task.Start();
			}
			
			public bool CompletedSuccessfully {
				get {
					return task.IsCompleted && task.Status == Tasks.TaskStatus.RanToCompletion;
				}
			}
			
			void Process()
			{
				List<Highlight> results = new List<Highlight>();
				
				foreach (HighlightingInfo info in GetInfo()) {
					IMember member = null;
					
					if (!info.Token.StartsWith("xmlns")) {
						MemberResolveResult rr = new XamlResolver().Resolve(info.GetExpressionResult(), info.Context.ParseInformation, FileContent) as MemberResolveResult;
						member = (rr != null) ? rr.ResolvedMember : null;
					}
					
					results.Add(new Highlight() { Member = member, Info = info });
				}
				
				this.results = results;
				
				WorkbenchSingleton.SafeThreadAsyncCall(InvokeRedraw);
			}
			
			void InvokeRedraw()
			{
				textView.Redraw(this.Offset, this.LineText.Length, DispatcherPriority.Background);
			}
			
			IEnumerable<HighlightingInfo> GetInfo()
			{
				int index = -1;
				XamlContext context = null;
				List<HighlightingInfo> infos = new List<HighlightingInfo>();
				
				do {
					index = LineText.IndexOf('=', index + 1);
					if (index > -1) {
						context = CompletionDataHelper.ResolveContext(FileContent, FileName, LineNumber, index + 1);
						if (!string.IsNullOrEmpty(context.AttributeName)) {
							int startIndex = LineText.Substring(0, index).LastIndexOf(context.AttributeName);
							infos.Add(new HighlightingInfo(context.AttributeName, startIndex, startIndex + context.AttributeName.Length, Offset, context));
						}
					}
				} while (index > -1);
				
				return infos;
			}
		}
		
		XamlColorizerSettings settings = defaultSettings;
		string fileContent;
		string fileName;
		IDocument document;
		
		Dictionary<DocumentLine, HighlightTask> highlightCache = new Dictionary<DocumentLine, HighlightTask>();
		
		public IViewContent Content { get; set; }
		
		public AvalonEdit.Rendering.TextView TextView { get; set;	}
		
		public XamlColorizer(IViewContent content, TextView textView)
		{
			this.Content = content;
			this.TextView = textView;
			
			IFileDocumentProvider documentProvider = this.Content as IFileDocumentProvider;
			
			if (documentProvider == null)
				throw new InvalidOperationException("XamlColorizer only works with ITextEditor view contents");
			
			this.document = documentProvider.GetDocumentForFile(this.Content.PrimaryFile);
			
			WeakLineTracker.Register(this.document.GetService(typeof(TextDocument)) as TextDocument, this);
		}
		
		protected override void Colorize(ITextRunConstructionContext context)
		{
			this.fileContent = this.document.CreateSnapshot().Text;
			this.fileName = this.Content.PrimaryFileName;
			
			base.Colorize(context);
		}
		
		protected override void ColorizeLine(DocumentLine line)
		{
			if (!highlightCache.ContainsKey(line)) {
				HighlightTask task = new HighlightTask(this.fileContent, this.fileName, line, this.TextView);
				task.Start();
				highlightCache.Add(line, task);
			} else {
				HighlightTask task = highlightCache[line];
				if (task.CompletedSuccessfully) {
					foreach (var result in task.GetResults()) {
						ColorizeMember(result.Info, line, result.Member);
					}
				}
			}
		}
		
		void ColorizeMember(HighlightingInfo info, DocumentLine line, IMember member)
		{
			if (member != null) {
				if (member is IEvent)
					ChangeLinePart(line.Offset + info.StartOffset, line.Offset + info.EndOffset, HighlightEvent);
				else
					ChangeLinePart(line.Offset + info.StartOffset, line.Offset + info.EndOffset, HighlightProperty);
			} else {
				if (info.Token.StartsWith("xmlns"))
					ChangeLinePart(line.Offset + info.StartOffset, line.Offset + info.EndOffset, HighlightNamespaceDeclaration);
				else
					Core.LoggingService.Debug(info.Token + " not highlighted; line " + line.LineNumber);
			}
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
		
		public void BeforeRemoveLine(DocumentLine line)
		{
			highlightCache.Remove(line);
		}
		
		public void SetLineLength(DocumentLine line, int newTotalLength)
		{
			highlightCache.Remove(line);
		}
		
		public void LineInserted(DocumentLine insertionPos, DocumentLine newLine)
		{
		}
		
		public void RebuildDocument()
		{
			highlightCache.Clear();
		}
		
		public struct HighlightingInfo
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