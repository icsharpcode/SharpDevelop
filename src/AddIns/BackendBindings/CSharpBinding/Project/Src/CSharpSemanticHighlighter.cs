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
using System.Diagnostics;
using System.Linq;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Parser;
using CSharpBinding.Parser;

namespace CSharpBinding
{
	/// <summary>
	/// Semantic highlighting for C#.
	/// </summary>
	public class CSharpSemanticHighlighter : IHighlighter
	{
		readonly IDocument document;
		internal CSharpSemanticHighlighterVisitor visitor;
		
		List<IDocumentLine> invalidLines;
		List<CachedLine> cachedLines;
		internal CSharpFullParseInformation parseInfo;
		
		bool hasCrashed;
		bool forceParseOnNextRefresh;
		bool eventHandlersAreRegistered;
		bool inHighlightingGroup;
		
		int lineNumber;
		HighlightedLine line;
		
		public CSharpSemanticHighlighter(IDocument document)
		{
			if (document == null)
				throw new ArgumentNullException("document");
			this.document = document;
			
			this.visitor = new CSharpSemanticHighlighterVisitor(this, document);
			
			if (document is TextDocument && SD.MainThread.CheckAccess()) {
				// Use the cache only for the live AvalonEdit document
				// Highlighting in read-only documents (e.g. search results) does
				// not need the cache as it does not need to highlight the same line multiple times
				cachedLines = new List<CachedLine>();
				// Line invalidation is only necessary for the live AvalonEdit document
				invalidLines = new List<IDocumentLine>();
				// Also, attach these event handlers only for real documents in the editor,
				// we don't need them for the highlighting in search results etc.
				SD.ParserService.ParseInformationUpdated += ParserService_ParseInformationUpdated;
				SD.ParserService.LoadSolutionProjectsThread.Finished += ParserService_LoadSolutionProjectsThreadEnded;
				eventHandlersAreRegistered = true;
			}
		}
		
		public void Dispose()
		{
			if (eventHandlersAreRegistered) {
				SD.ParserService.ParseInformationUpdated -= ParserService_ParseInformationUpdated;
				SD.ParserService.LoadSolutionProjectsThread.Finished -= ParserService_LoadSolutionProjectsThreadEnded;
				eventHandlersAreRegistered = false;
			}
			this.visitor.Dispose();
			this.parseInfo = null;
		}
		
		public event HighlightingStateChangedEventHandler HighlightingStateChanged;
		
		protected virtual void OnHighlightingStateChanged(int fromLineNumber, int toLineNumber)
		{
			if (HighlightingStateChanged != null) {
				HighlightingStateChanged(fromLineNumber, toLineNumber);
			}
		}
		
		IDocument IHighlighter.Document {
			get { return document; }
		}
		
		IEnumerable<HighlightingColor> IHighlighter.GetColorStack(int lineNumber)
		{
			return null;
		}
		
		void IHighlighter.UpdateHighlightingState(int lineNumber)
		{
		}
		
		public HighlightedLine HighlightLine(int lineNumber)
		{
			IDocumentLine documentLine = document.GetLineByNumber(lineNumber);
			if (hasCrashed) {
				// don't highlight anymore after we've crashed
				return new HighlightedLine(document, documentLine);
			}
			ITextSourceVersion newVersion = document.Version;
			CachedLine cachedLine = null;
			if (cachedLines != null) {
				for (int i = 0; i < cachedLines.Count; i++) {
					if (cachedLines[i].DocumentLine == documentLine) {
						if (newVersion == null || !newVersion.BelongsToSameDocumentAs(cachedLines[i].OldVersion)) {
							// cannot list changes from old to new: we can't update the cache, so we'll remove it
							cachedLines.RemoveAt(i);
						} else {
							cachedLine = cachedLines[i];
						}
						break;
					}
				}
				
				if (cachedLine != null && cachedLine.IsValid && newVersion.CompareAge(cachedLine.OldVersion) == 0) {
					// the file hasn't changed since the cache was created, so just reuse the old highlighted line
					#if DEBUG
					cachedLine.HighlightedLine.ValidateInvariants();
					#endif
					return cachedLine.HighlightedLine;
				}
			}
			
			bool wasInHighlightingGroup = inHighlightingGroup;
			if (!inHighlightingGroup) {
				BeginHighlighting();
			}
			try {
				return DoHighlightLine(lineNumber, documentLine, cachedLine, newVersion);
			} finally {
				line = null;
				if (!wasInHighlightingGroup)
					EndHighlighting();
			}
		}
		
		HighlightedLine DoHighlightLine(int lineNumber, IDocumentLine documentLine, CachedLine cachedLine, ITextSourceVersion newVersion)
		{
			if (parseInfo == null) {
				if (forceParseOnNextRefresh) {
					forceParseOnNextRefresh = false;
					parseInfo = SD.ParserService.Parse(FileName.Create(document.FileName), document) as CSharpFullParseInformation;
				} else {
					parseInfo = SD.ParserService.GetCachedParseInformation(FileName.Create(document.FileName), newVersion) as CSharpFullParseInformation;
				}
			}
			if (parseInfo == null) {
				if (invalidLines != null && !invalidLines.Contains(documentLine)) {
					invalidLines.Add(documentLine);
					//Debug.WriteLine("Semantic highlighting for line {0} - marking as invalid", lineNumber);
				}
				
				if (cachedLine != null) {
					// If there's a cached version, adjust it to the latest document changes and return it.
					// This avoids flickering when changing a line that contains semantic highlighting.
					cachedLine.Update(newVersion);
					#if DEBUG
					cachedLine.HighlightedLine.ValidateInvariants();
					#endif
					return cachedLine.HighlightedLine;
				} else {
					return null;
				}
			}
			
			if (visitor.Resolver == null) {
				var compilation = SD.ParserService.GetCompilationForFile(parseInfo.FileName);
				visitor.Resolver = parseInfo.GetResolver(compilation);
			}
			
			line = new HighlightedLine(document, documentLine);
			this.lineNumber = lineNumber;
			visitor.UpdateLineInformation(lineNumber);

			if (Debugger.IsAttached) {
				parseInfo.SyntaxTree.AcceptVisitor(visitor);
				#if DEBUG
				line.ValidateInvariants();
				#endif
			} else {
				try {
					parseInfo.SyntaxTree.AcceptVisitor(visitor);
					#if DEBUG
					line.ValidateInvariants();
					#endif
				} catch (Exception ex) {
					hasCrashed = true;
					throw new ApplicationException("Error highlighting line " + lineNumber, ex);
				}
			}
			//Debug.WriteLine("Semantic highlighting for line {0} - added {1} sections", lineNumber, line.Sections.Count);
			if (cachedLines != null && document.Version != null) {
				cachedLines.Add(new CachedLine(line, document.Version));
			}
			return line;
		}
		
		internal void Colorize(TextLocation start, TextLocation end, HighlightingColor color)
		{
			if (color == null)
				return;
			if (start.Line <= lineNumber && end.Line >= lineNumber) {
				int lineStartOffset = line.DocumentLine.Offset;
				int lineEndOffset = lineStartOffset + line.DocumentLine.Length;
				int startOffset = lineStartOffset + (start.Line == lineNumber ? start.Column - 1 : 0);
				int endOffset = lineStartOffset + (end.Line == lineNumber ? end.Column - 1 : line.DocumentLine.Length);
				// For some parser errors, the mcs parser produces grossly wrong locations (e.g. miscounting the number of newlines),
				// so we need to coerce the offsets to valid values within the line
				startOffset = startOffset.CoerceValue(lineStartOffset, lineEndOffset);
				endOffset = endOffset.CoerceValue(lineStartOffset, lineEndOffset);
				if (line.Sections.Count > 0) {
					HighlightedSection prevSection = line.Sections.Last();
					if (startOffset < prevSection.Offset + prevSection.Length) {
						// The mcs parser sometimes creates strange ASTs with duplicate nodes
						// when there are syntax errors (e.g. "int A() public static void Main() {}"),
						// so we'll silently ignore duplicate colorization.
						return;
						//throw new InvalidOperationException("Cannot create unordered highlighting section");
					}
				}
				line.Sections.Add(new HighlightedSection {
				                  	Offset = startOffset,
				                  	Length = endOffset - startOffset,
				                  	Color = color
				                  });
			}
		}
		
		HighlightingColor IHighlighter.DefaultTextColor {
			get {
				return null;
			}
		}
		
		public void BeginHighlighting()
		{
			if (inHighlightingGroup)
				throw new InvalidOperationException();
			inHighlightingGroup = true;
			if (invalidLines == null) {
				// if invalidation isn't available, we're forced to parse the file now
				forceParseOnNextRefresh = true;
			}
		}
		
		public void EndHighlighting()
		{
			inHighlightingGroup = false;
			visitor.Resolver = null;
			this.parseInfo = null;
			
			// TODO use this to remove cached lines which are no longer visible
//			var visibleDocumentLines = new HashSet<IDocumentLine>(syntaxHighlighter.GetVisibleDocumentLines());
//			cachedLines.RemoveAll(c => !visibleDocumentLines.Contains(c.DocumentLine));
		}
		
		public HighlightingColor GetNamedColor(string name)
		{
			return null;
		}
		
		#region Caching
		// If a line gets edited and we need to display it while no parse information is ready for the
		// changed file, the line would flicker (semantic highlightings disappear temporarily).
		// We avoid this issue by storing the semantic highlightings and updating them on document changes
		// (using anchor movement)
		class CachedLine
		{
			public readonly HighlightedLine HighlightedLine;
			public ITextSourceVersion OldVersion;
			
			/// <summary>
			/// Gets whether the cache line is valid (no document changes since it was created).
			/// This field gets set to false when Update() is called.
			/// </summary>
			public bool IsValid;
			
			public IDocumentLine DocumentLine { get { return HighlightedLine.DocumentLine; } }
			
			public CachedLine(HighlightedLine highlightedLine, ITextSourceVersion fileVersion)
			{
				if (highlightedLine == null)
					throw new ArgumentNullException("highlightedLine");
				if (fileVersion == null)
					throw new ArgumentNullException("fileVersion");
				
				this.HighlightedLine = highlightedLine;
				this.OldVersion = fileVersion;
				this.IsValid = true;
			}
			
			public void Update(ITextSourceVersion newVersion)
			{
				// Apply document changes to all highlighting sections:
				foreach (TextChangeEventArgs change in OldVersion.GetChangesTo(newVersion)) {
					foreach (HighlightedSection section in HighlightedLine.Sections) {
						int endOffset = section.Offset + section.Length;
						section.Offset = change.GetNewOffset(section.Offset);
						endOffset = change.GetNewOffset(endOffset);
						section.Length = endOffset - section.Offset;
					}
				}
				// The resulting sections might have become invalid:
				// - zero-length if section was deleted,
				// - a section might have moved outside the range of this document line (newline inserted in document = line split up)
				// So we will remove all highlighting sections which have become invalid.
				int lineStart = HighlightedLine.DocumentLine.Offset;
				int lineEnd = lineStart + HighlightedLine.DocumentLine.Length;
				for (int i = 0; i < HighlightedLine.Sections.Count; i++) {
					HighlightedSection section = HighlightedLine.Sections[i];
					if (section.Offset < lineStart || section.Offset + section.Length > lineEnd || section.Length <= 0)
						HighlightedLine.Sections.RemoveAt(i--);
				}
				
				this.OldVersion = newVersion;
				this.IsValid = false;
			}
		}
		
		void InvalidateAll()
		{
			cachedLines.Clear();
			invalidLines.Clear();
			forceParseOnNextRefresh = true;
			OnHighlightingStateChanged(1, document.LineCount);
		}
		#endregion
		
		#region Event Handlers
		void syntaxHighlighter_VisibleDocumentLinesChanged(object sender, EventArgs e)
		{

		}
		
		void ParserService_LoadSolutionProjectsThreadEnded(object sender, EventArgs e)
		{
			InvalidateAll();
		}
		
		void ParserService_ParseInformationUpdated(object sender, ParseInformationEventArgs e)
		{
			if (FileUtility.IsEqualFileName(e.FileName, document.FileName) && invalidLines.Count > 0) {
				cachedLines.Clear();
				foreach (IDocumentLine line in invalidLines) {
					if (!line.IsDeleted) {
						OnHighlightingStateChanged(line.LineNumber, line.LineNumber);
					}
				}
				invalidLines.Clear();
			}
		}
		#endregion
	}
}
