// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Xml;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.XmlEditor;

namespace ICSharpCode.XamlBinding
{
	using Task = System.Threading.Tasks.Task;
	using Tasks = System.Threading.Tasks;
	
	public class XamlColorizer : DocumentColorizingTransformer, ILineTracker, IDisposable
	{
		public struct Highlight {
			public IMember Member { get; set; }
			public HighlightingInfo Info { get; set; }
		}
		
		public sealed class HighlightTask {
			// input
			public string FileName { get; private set; }
			public string LineText { get; private set; }
			public int LineNumber { get; private set; }
			public int Offset { get; private set; }
			
			TextView textView;
			AXmlParser parser;
			
			public HighlightTask(string fileName, DocumentLine currentLine, TextView textView)
			{
				this.FileName = fileName;
				this.textView = textView;
				this.parser = (textView.Services.GetService(typeof(XamlLanguageBinding))
				               as XamlLanguageBinding).Parser;
				this.LineNumber = currentLine.LineNumber;
				this.LineText = currentLine.Text;
				this.Offset = currentLine.Offset;
				this.task = new Task(Process);
			}
			
			IList<Highlight> results;
			
			// output
			public IList<Highlight> GetResults()
			{
				return results;
			}
			
			readonly Task task;
			
			public void Start()
			{
				task.Start();
			}
			
			public bool CompletedSuccessfully {
				get {
					return task.IsCompleted && task.Status == Tasks.TaskStatus.RanToCompletion;
				}
			}
			
			public bool Invalid { get; set; }
			
			void Process()
			{
				try {
					List<Highlight> results = new List<Highlight>();
					
					foreach (HighlightingInfo info in GetInfo()) {
						IMember member = null;
						
						if (task.IsCancellationRequested) {
							task.AcknowledgeCancellation();
							return;
						}
						
						if (!info.Token.StartsWith("xmlns", StringComparison.OrdinalIgnoreCase)) {
							MemberResolveResult rr = XamlResolver.Resolve(info.Token, info.Context) as MemberResolveResult;
							member = (rr != null) ? rr.ResolvedMember : null;
						}
						
						results.Add(new Highlight() { Member = member, Info = info });
					}
					
					this.results = results;
					
					WorkbenchSingleton.SafeThreadAsyncCall(InvokeRedraw);
				} catch (Exception e) {
					WorkbenchSingleton.SafeThreadAsyncCall(() => MessageService.ShowError(e));
				}
			}
			
			void InvokeRedraw()
			{
				task.Wait();
				textView.Redraw(this.Offset, this.LineText.Length, DispatcherPriority.Background);
			}
			
			IEnumerable<HighlightingInfo> GetInfo()
			{
				int index = -1;
				XamlContext context = null;
				
				do {
					if (index + 1 >= LineText.Length)
						break;

					index = LineText.IndexOfAny(index + 1, '=', '.');
					if (index > -1) {
						context = CompletionDataHelper.ResolveContext(parser, FileName, Offset + index);
						
						if (context.ActiveElement == null || context.InAttributeValueOrMarkupExtension || context.InCommentOrCData)
							continue;
						
						string propertyName;
						string token;
						
						switch (context.Description) {
							case XamlContextDescription.AtTag:
								token = context.ActiveElement.Name;
								int propertyNameIndex = token.IndexOf('.');
								
								if (propertyNameIndex == -1)
									continue;
								
								propertyName = token.Substring(propertyNameIndex + 1);
								break;
							case XamlContextDescription.InTag:
								if (LineText[index] == '.' || context.Attribute == null)
									continue;
								
								token = propertyName = context.Attribute.Name;
								break;
							default:
								continue;
						}
						
						int startIndex = LineText.LastIndexOf(propertyName, index, StringComparison.Ordinal);
						
						if (startIndex > -1) {
							yield return new HighlightingInfo(token, startIndex, startIndex + propertyName.Length, Offset, context);
						}
					}
				} while (index > -1);
			}
		}
		
		Dictionary<DocumentLine, HighlightTask> highlightCache = new Dictionary<DocumentLine, HighlightTask>();
		
		public ITextEditor Editor { get; set; }
		
		public AvalonEdit.Rendering.TextView TextView { get; set;	}
		
		public XamlColorizer(ITextEditor editor, TextView textView)
		{
			this.Editor = editor;
			this.TextView = textView;
			
			this.weakLineTracker = WeakLineTracker.Register(this.Editor.Document.GetService(typeof(TextDocument)) as TextDocument, this);
			
			ParserService.LoadSolutionProjectsThreadEnded += ParserServiceLoadSolutionProjectsThreadEnded;
			
			colorizers.Add(this);
		}
		
		static IList<XamlColorizer> colorizers = new List<XamlColorizer>();
		
		public static void RefreshAll()
		{
			foreach (XamlColorizer colorizer in colorizers) {
				colorizer.RebuildDocument();
				colorizer.TextView.Redraw();
			}
		}

		void ParserServiceLoadSolutionProjectsThreadEnded(object sender, EventArgs e)
		{
			WorkbenchSingleton.SafeThreadAsyncCall(
				() => {
					highlightCache.Clear();
					TextView.Redraw();
				}
			);
		}
		
		WeakLineTracker weakLineTracker;
		bool disposed;
		
		public void Dispose()
		{
			if (!disposed) {
				ParserService.LoadSolutionProjectsThreadEnded -= ParserServiceLoadSolutionProjectsThreadEnded;
				weakLineTracker.Deregister();
				colorizers.Remove(this);
			}
			disposed = true;
		}
		
		protected override void ColorizeLine(DocumentLine line)
		{
			if (!highlightCache.ContainsKey(line)) {
				HighlightTask task = new HighlightTask(this.Editor.FileName, line, this.TextView);
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
			ColorizeInvalidated();
		}
		
		void ColorizeMember(HighlightingInfo info, DocumentLine line, IMember member)
		{
			try {
				if (info.Context.IgnoredXmlns.Any(item => info.Token.StartsWith(item + ":", StringComparison.OrdinalIgnoreCase))) {
					ChangeLinePart(line.Offset + info.StartOffset, line.Offset + info.EndOffset, HighlightIgnored);
				} else {
					if (member != null) {
						if (member is IEvent)
							ChangeLinePart(line.Offset + info.StartOffset, line.Offset + info.EndOffset, HighlightEvent);
						else
							ChangeLinePart(line.Offset + info.StartOffset, line.Offset + info.EndOffset, HighlightProperty);
					} else {
						if (info.Token.StartsWith("xmlns", StringComparison.OrdinalIgnoreCase) || info.Token.StartsWith(Utils.GetNamespacePrefix(CompletionDataHelper.MarkupCompatibilityNamespace, info.Context) + ":", StringComparison.OrdinalIgnoreCase))
							ChangeLinePart(line.Offset + info.StartOffset, line.Offset + info.EndOffset, HighlightNamespaceDeclaration);
						else
							Core.LoggingService.Debug(info.Token + " not highlighted; line " + line.LineNumber);
					}
				}
			} catch (ArgumentOutOfRangeException) {}
		}
		
		void ColorizeInvalidated()
		{
			foreach (var item in highlightCache.ToArray()) {
				if (item.Value.Invalid) {
					var newTask = new HighlightTask(this.Editor.FileName, item.Key, this.TextView);
					newTask.Start();
					highlightCache[item.Key] = newTask;
				}
			}
		}
		
		void InvalidateLines(DocumentLine line)
		{
			DocumentLine current = line;
			while (current != null) {
				HighlightTask task;
				if (highlightCache.TryGetValue(current, out task))
					task.Invalid = true;

				current = current.NextLine;
			}
		}
		
		void HighlightProperty(VisualLineElement element)
		{
			element.TextRunProperties.SetForegroundBrush(XamlBindingOptions.PropertyForegroundBrush);
			element.TextRunProperties.SetBackgroundBrush(XamlBindingOptions.PropertyBackgroundBrush);
		}
		
		void HighlightEvent(VisualLineElement element)
		{
			element.TextRunProperties.SetForegroundBrush(XamlBindingOptions.EventForegroundBrush);
			element.TextRunProperties.SetBackgroundBrush(XamlBindingOptions.EventBackgroundBrush);
		}
		
		void HighlightNamespaceDeclaration(VisualLineElement element)
		{
			element.TextRunProperties.SetForegroundBrush(XamlBindingOptions.NamespaceDeclarationForegroundBrush);
			element.TextRunProperties.SetBackgroundBrush(XamlBindingOptions.NamespaceDeclarationBackgroundBrush);
		}
		
		void HighlightIgnored(VisualLineElement element)
		{
			element.TextRunProperties.SetForegroundBrush(XamlBindingOptions.IgnoredForegroundBrush);
			element.TextRunProperties.SetBackgroundBrush(XamlBindingOptions.IgnoredBackgroundBrush);
		}
		
		public void BeforeRemoveLine(DocumentLine line)
		{
			highlightCache.Remove(line);
			InvalidateLines(line.NextLine);
		}
		
		public void SetLineLength(DocumentLine line, int newTotalLength)
		{
			highlightCache.Remove(line);
			InvalidateLines(line.NextLine);
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
			string token;
			int startOffset;
			int endOffset;
			int lineOffset;
			XamlContext context;
			
			public HighlightingInfo(string token, int startOffset, int endOffset, int lineOffset, XamlContext context)
			{
				if (token == null)
					throw new ArgumentNullException("token");
				if (startOffset < 0)
					throw new ArgumentOutOfRangeException("startOffset", startOffset, "Value must be greater 0");
				if (endOffset < 0)
					throw new ArgumentOutOfRangeException("endOffset", endOffset, "Value must be greater 0");
				if (lineOffset < 0)
					throw new ArgumentOutOfRangeException("lineOffset", lineOffset, "Value must be greater 0");
				if (context == null)
					throw new ArgumentNullException("context");
				
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
		}
	}
}