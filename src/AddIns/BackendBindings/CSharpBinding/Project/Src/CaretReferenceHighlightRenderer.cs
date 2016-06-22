// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Media;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Parser;
using CSharpBinding.Parser;

namespace CSharpBinding
{
	class CaretReferenceHighlightRenderer : IDisposable, IBackgroundRenderer
	{
		readonly ITextEditor editor;
		readonly TextView textView;

		ISymbolReference currentSymbolReference;
		CancellationTokenSource caretMovementTokenSource;
		DispatcherTimer timer;
		List<ISegment> currentReferences;

		Pen borderPen;
		Brush backgroundBrush;

		public readonly Color DefaultBorderColor = Color.FromArgb(52, 30, 130, 255);
		public readonly Color DefaultFillColor = Color.FromArgb(22, 30, 130, 255);
		readonly int borderThickness = 1;
		readonly int cornerRadius = 1;

		public CaretReferenceHighlightRenderer(ITextEditor editor)
		{
			if (editor == null)
				throw new ArgumentNullException("editor");
			this.editor = editor;
			this.textView = editor.GetService<TextView>();
			this.borderPen = new Pen(new SolidColorBrush(DefaultBorderColor), borderThickness);
			this.backgroundBrush = new SolidColorBrush(DefaultFillColor);
			this.borderPen.Freeze();
			this.backgroundBrush.Freeze();
			this.timer = new DispatcherTimer {
				Interval = TimeSpan.FromMilliseconds(500)
			};
			timer.Tick += (sender, e) => ResolveAtCaret();
				
			editor.Caret.LocationChanged += CaretLocationChanged;
			editor.Document.ChangeCompleted += DocumentChanged;
			textView.VisualLinesChanged += VisualLinesChanged;
			SD.ParserService.ParseInformationUpdated += ParseInformationUpdated;
			SD.ParserService.LoadSolutionProjectsThread.Finished += LoadSolutionProjectsThreadFinished;
			this.textView.BackgroundRenderers.Add(this);
		}

		public void Dispose()
		{
			timer.Stop();
			this.textView.BackgroundRenderers.Remove(this);
			editor.Caret.LocationChanged -= CaretLocationChanged;
			editor.Document.ChangeCompleted -= DocumentChanged;
			textView.VisualLinesChanged -= VisualLinesChanged;
			SD.ParserService.ParseInformationUpdated -= ParseInformationUpdated;
			SD.ParserService.LoadSolutionProjectsThread.Finished -= LoadSolutionProjectsThreadFinished;
		}

		public void Draw(TextView textView, DrawingContext drawingContext)
		{
			if (currentReferences == null) {
				if (textView.VisualLines.Count == 0)
					return;
				var start = textView.VisualLines.First().FirstDocumentLine.LineNumber;
				var end = textView.VisualLines.Last().LastDocumentLine.LineNumber;
				currentReferences = new List<ISegment>();
				FindCurrentReferences(start, end);
			}
			if (currentReferences.Count == 0)
				return;
			BackgroundGeometryBuilder builder = new BackgroundGeometryBuilder();
			builder.CornerRadius = cornerRadius;
			builder.AlignToMiddleOfPixels = true;
			foreach (var reference in this.currentReferences) {
				builder.AddSegment(textView, new TextSegment() {
					StartOffset = reference.Offset,
					Length = reference.Length
				});
				builder.CloseFigure();
			}
			Geometry geometry = builder.CreateGeometry();
			if (geometry != null) {
				drawingContext.DrawGeometry(backgroundBrush, borderPen, geometry);
			}
		}

		void VisualLinesChanged(object sender, System.EventArgs e)
		{
			currentReferences = null;
		}

		void ParseInformationUpdated(object sender, ParseInformationEventArgs e)
		{
			if (SD.ParserService.LoadSolutionProjectsThread.IsRunning)
				return;
			if (!e.FileName.Equals(editor.FileName))
				return;
			currentReferences = null;
			textView.InvalidateLayer(KnownLayer.Selection);
		}

		void LoadSolutionProjectsThreadFinished(object sender, EventArgs e)
		{
			StartTimer();
		}

		void CaretLocationChanged(object sender, EventArgs e)
		{
			if (currentReferences != null) {
				int caretOffset = editor.Caret.Offset;
				if (!currentReferences.Any(r => r.Offset <= caretOffset && caretOffset <= r.EndOffset)) {
					// If the caret moved outside any highlighted identifier, immediately clear the highlight
					// as the caret is not on the same symbol as before
					SetCurrentSymbol(null);
				}
			}
			StartTimer();
		}
		
		void DocumentChanged(object sender, EventArgs e)
		{
			// If the document has changed, the current symbol likely also has changed (most edits are at the caret position),
			// so immediately clear the highlighting.
			SetCurrentSymbol(null);
			StartTimer();
		}
		
		void StartTimer()
		{
			if (caretMovementTokenSource != null)
				caretMovementTokenSource.Cancel();
			timer.Stop();
			
			var codeEditorOptions = editor.Options as ICodeEditorOptions;
			if (codeEditorOptions == null || codeEditorOptions.HighlightSymbol) {
				// If symbol highlighting is enabled
				timer.Start();
			} else {
				// Clear highlighting if its disabled
				SetCurrentSymbol(null);
			}
		}
		
		async void ResolveAtCaret()
		{
			timer.Stop();
			caretMovementTokenSource = new CancellationTokenSource();
			try {
				var rr = await SD.ParserService.ResolveAsync(editor.FileName, editor.Caret.Location, editor.Document, cancellationToken: caretMovementTokenSource.Token);
				SetCurrentSymbol(rr.GetSymbol());
			} catch (OperationCanceledException) {
			}
		}

		IResolveVisitorNavigator InitNavigator(ICompilation compilation)
		{
			if (currentSymbolReference == null || compilation == null)
				return null;
			var symbol = currentSymbolReference.Resolve(compilation.TypeResolveContext);
			FindReferences findReferences = new FindReferences();
			if (symbol == null) return null;
			var searchScopes = findReferences.GetSearchScopes(symbol);
			if (searchScopes.Count == 0)
				return null;
			var navigators = new IResolveVisitorNavigator[searchScopes.Count];
			for (int i = 0; i < navigators.Length; i++) {
				navigators[i] = searchScopes[i].GetNavigator(compilation, ColorizeMatch);
			}
			IResolveVisitorNavigator combinedNavigator;
			if (searchScopes.Count == 1) {
				combinedNavigator = navigators[0];
			}
			else {
				combinedNavigator = new CompositeResolveVisitorNavigator(navigators);
			}
			return combinedNavigator;
		}

		void FindCurrentReferences(int start, int end)
		{
			ICompilation compilation = SD.ParserService.GetCompilationForFile(editor.FileName);
			CSharpFullParseInformation parseInfo = SD.ParserService.GetCachedParseInformation(editor.FileName) as CSharpFullParseInformation;
			
			if (currentSymbolReference == null || parseInfo == null)
				return;
			
			IResolveVisitorNavigator currentNavigator = InitNavigator(compilation);
			CSharpAstResolver resolver = parseInfo.GetResolver(compilation);
			
			if (currentNavigator == null || resolver == null)
				return;
			
			VisitVisibleNodes(parseInfo.SyntaxTree, currentNavigator, resolver, start, end);
		}
		
		void VisitVisibleNodes(AstNode node, IResolveVisitorNavigator currentNavigator, CSharpAstResolver resolver, int start, int end)
		{
			if (!CSharpAstResolver.IsUnresolvableNode(node))
				currentNavigator.Resolved(node, resolver.Resolve(node));
			for (var child = node.FirstChild; child != null; child = child.NextSibling) {
				if (child.StartLocation.Line <= end && child.EndLocation.Line >= start)
					VisitVisibleNodes(child, currentNavigator, resolver, start, end);
			}
		}

		void ColorizeMatch(AstNode node, ResolveResult result)
		{
			var identifierNode = FindReferences.GetNodeToReplace(node);
			TextLocation start, end;
			if (identifierNode != null && !identifierNode.IsNull) {
				start = identifierNode.StartLocation;
				end = identifierNode.EndLocation;
			}
			else {
				start = node.StartLocation;
				end = node.EndLocation;
			}
			currentReferences.Add(new TextSegment {
				StartOffset = editor.Document.GetOffset(start),
				EndOffset = editor.Document.GetOffset(end)
			});
		}

		public KnownLayer Layer {
			get {
				return KnownLayer.Selection;
			}
		}

		void SetCurrentSymbol(ISymbol symbol)
		{
			currentSymbolReference = null;
			if (symbol != null)
				currentSymbolReference = symbol.ToReference();
			currentReferences = null;
			textView.InvalidateLayer(KnownLayer.Selection);
		}
	}
}


