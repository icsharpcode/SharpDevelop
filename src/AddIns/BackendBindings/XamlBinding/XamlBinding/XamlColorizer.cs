// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
			readonly string fileName;
			readonly string lineText;
			readonly int offset;
			
			TextView textView;
			ITextBuffer snapshot;
			
			public HighlightTask(ITextEditor editor, DocumentLine currentLine, TextView textView)
			{
				this.fileName = editor.FileName;
				this.textView = textView;
				this.snapshot = editor.Document.CreateSnapshot();
				this.lineText = textView.Document.GetText(currentLine);
				this.offset = currentLine.Offset;
				this.task = new Task(Process);
			}
			
			public HighlightTask(ITextEditor editor, DocumentLine currentLine, TextView textView, IList<Highlight> oldHighlightData)
				: this(editor, currentLine, textView)
			{
				this.results = oldHighlightData;
			}
			
			IList<Highlight> results;
			
			// output
			public Highlight[] GetResults()
			{
				lock (this) {
					if (results == null)
						return null;
					return results.ToArray();
				}
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
						
						// Commented out because task doesn't come with cancellation support in .NET 4.0 Beta 2
						// (use CancellationToken instead)
						// I didn't have to remove any call to task.Cancel(), so apparently this was dead code.
						//if (task.IsCancellationRequested) {
						//	task.AcknowledgeCancellation();
						//	return;
						//}
						// TODO: implement cancellation support
						
						if (!info.Token.StartsWith("xmlns", StringComparison.OrdinalIgnoreCase)) {
							MemberResolveResult rr = XamlResolver.Resolve(info.Token, info.Context) as MemberResolveResult;
							member = (rr != null) ? rr.ResolvedMember : null;
						}
						
						results.Add(new Highlight() { Member = member, Info = info });
					}
					
					lock (this)
						this.results = results;
					
					WorkbenchSingleton.SafeThreadAsyncCall(InvokeRedraw);
				} catch (Exception e) {
					WorkbenchSingleton.SafeThreadAsyncCall(() => MessageService.ShowException(e));
				}
			}
			
			void InvokeRedraw()
			{
				task.Wait();
				textView.Redraw(this.offset, this.lineText.Length, DispatcherPriority.Background);
			}
			
			IEnumerable<HighlightingInfo> GetInfo()
			{
				int index = -1;
				XamlContext context = null;
				
				do {
					if (index + 1 >= lineText.Length)
						break;

					index = lineText.IndexOfAny(index + 1, '=', '.');
					if (index > -1) {
						context = CompletionDataHelper.ResolveContext(snapshot, fileName, offset + index);
						
						if (context.ActiveElement == null || context.InAttributeValueOrMarkupExtension || context.InCommentOrCData)
							continue;
						
						string propertyName;
						string token;
						int startIndex;
						
						switch (context.Description) {
							case XamlContextDescription.AtTag:
								token = context.ActiveElement.Name;
								int propertyNameIndex = token.IndexOf('.');
								
								if (propertyNameIndex == -1)
									continue;
								
								propertyName = token.Substring(propertyNameIndex + 1);
								startIndex = lineText.IndexOf(propertyName, index, StringComparison.Ordinal);
								break;
							case XamlContextDescription.InTag:
								if (lineText[index] == '.' || context.Attribute == null)
									continue;
								
								token = propertyName = context.Attribute.Name;
								startIndex = lineText.LastIndexOf(propertyName, index, StringComparison.Ordinal);
								break;
							default:
								continue;
						}
						
						if (startIndex > -1) {
							yield return new HighlightingInfo(token, startIndex, startIndex + propertyName.Length, offset, context);
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
				HighlightTask task = new HighlightTask(this.Editor, line, this.TextView);
				task.Start();
				highlightCache.Add(line, task);
			} else {
				HighlightTask task = highlightCache[line];
				var results = task.GetResults();
				if (results != null) {
					foreach (var result in results) {
						ColorizeMember(result.Info, line, result.Member);
					}
				}
			}
			ColorizeInvalidated();
		}
		
		void ColorizeMember(HighlightingInfo info, DocumentLine line, IMember member)
		{
			try {
				Action<VisualLineElement> handler = null;
				
				if (info.Token.StartsWith(info.Context.XamlNamespacePrefix + ":", StringComparison.Ordinal))
					handler = HighlightProperty;
				else if (info.Context.IgnoredXmlns.Any(item => info.Token.StartsWith(item + ":", StringComparison.Ordinal)))
					handler = HighlightIgnored;
				else if (member != null) {
					if (member is IEvent)
						handler = HighlightEvent;
					else
						handler = HighlightProperty;
				} else {
					if (info.Token.StartsWith("xmlns", StringComparison.OrdinalIgnoreCase) || info.Token.StartsWith(Utils.GetNamespacePrefix(CompletionDataHelper.MarkupCompatibilityNamespace, info.Context) + ":", StringComparison.OrdinalIgnoreCase))
						handler = HighlightNamespaceDeclaration;
					else if (info.Token.StartsWith("xml:", StringComparison.OrdinalIgnoreCase))
						handler = HighlightProperty;
					else
						Core.LoggingService.Debug(info.Token + " not highlighted; line " + line.LineNumber);
				}
				if (handler != null)
					ChangeLinePart(line.Offset + info.StartOffset, line.Offset + info.EndOffset, handler);
			} catch (ArgumentOutOfRangeException) {}
		}

		void ColorizeInvalidated()
		{
			foreach (var item in highlightCache.ToArray()) {
				if (item.Key.IsDeleted) {
					highlightCache.Remove(item.Key);
					continue;
				}
				if (item.Value.Invalid) {
					var newTask = new HighlightTask(this.Editor, item.Key, this.TextView, item.Value.GetResults());
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

		#region highlight helpers
		void HighlightProperty(VisualLineElement element)
		{
			element.TextRunProperties.SetForegroundBrush(XamlBindingOptions.PropertyForegroundColor.ToBrush());
			element.TextRunProperties.SetBackgroundBrush(XamlBindingOptions.PropertyBackgroundColor.ToBrush());
		}

		void HighlightEvent(VisualLineElement element)
		{
			element.TextRunProperties.SetForegroundBrush(XamlBindingOptions.EventForegroundColor.ToBrush());
			element.TextRunProperties.SetBackgroundBrush(XamlBindingOptions.EventBackgroundColor.ToBrush());
		}

		void HighlightNamespaceDeclaration(VisualLineElement element)
		{
			element.TextRunProperties.SetForegroundBrush(XamlBindingOptions.NamespaceDeclarationForegroundColor.ToBrush());
			element.TextRunProperties.SetBackgroundBrush(XamlBindingOptions.NamespaceDeclarationBackgroundColor.ToBrush());
		}

		void HighlightIgnored(VisualLineElement element)
		{
			element.TextRunProperties.SetForegroundBrush(XamlBindingOptions.IgnoredForegroundColor.ToBrush());
			element.TextRunProperties.SetBackgroundBrush(XamlBindingOptions.IgnoredBackgroundColor.ToBrush());
		}
		#endregion

		#region ILineTracker implementation
		public void BeforeRemoveLine(DocumentLine line)
		{
			InvalidateLines(line.NextLine);
		}

		public void SetLineLength(DocumentLine line, int newTotalLength)
		{
			InvalidateLines(line);
		}

		public void LineInserted(DocumentLine insertionPos, DocumentLine newLine)
		{
			InvalidateLines(newLine);
		}

		public void RebuildDocument()
		{
			highlightCache.Clear();
		}
		#endregion

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
