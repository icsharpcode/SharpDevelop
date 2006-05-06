// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Xml;
using System.Windows.Forms;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor
{
	public interface IAdvancedHighlighter : IDisposable
	{
		/// <summary>
		/// Is called once after creating the highlighter. Gives your highlighter a chance
		/// to register events on the text editor.
		/// </summary>
		void Initialize(TextEditorControl textEditor);
		
		/// <summary>
		/// Gives your highlighter the chance to change the highlighting of the words.
		/// </summary>
		void MarkLine(IDocument document, LineSegment currentLine, List<TextWord> words);
		
		/// <summary>
		/// When your class fires this event, MarkLine will be called for all waiting lines.
		/// You can fire this event on any thread, but MarkLine will be called on that thread
		/// - so if you use multithreading, you must invoke when accessing the document inside
		/// MarkLine.
		/// </summary>
		event EventHandler MarkOutstandingRequests;
	}
	
	internal class ParserHighlightingStrategy : DefaultHighlightingStrategy
	{
		readonly object lockObject = new object();
		readonly IAdvancedHighlighter highlighter;
		IDocument document;
		Dictionary<LineSegment, List<TextWord>> outstanding = new Dictionary<LineSegment, List<TextWord>>();
		
		public ParserHighlightingStrategy(DefaultHighlightingStrategy baseStrategy, IAdvancedHighlighter highlighter)
		{
			ImportSettingsFrom(baseStrategy);
			this.highlighter = highlighter;
			highlighter.MarkOutstandingRequests += OnMarkOutstandingRequest;
		}
		
		int directMark; // counter for immediate marking when only few lines have changed
		
		public override void MarkTokens(IDocument document)
		{
			lock (lockObject) {
				outstanding.Clear();
			}
			base.MarkTokens(document);
		}
		
		public override void MarkTokens(IDocument document, List<LineSegment> inputLines)
		{
			directMark = (inputLines.Count < 3) ? inputLines.Count : 0;
			base.MarkTokens(document, inputLines);
			directMark = 0;
		}
		
		protected override void OnParsedLine(IDocument document, LineSegment currentLine, List<TextWord> words)
		{
			int ln = currentLineNumber;
			
			if (directMark > 0) {
				directMark--;
				highlighter.MarkLine(document, currentLine, words);
			} else {
				this.document = document;
				lock (lockObject) {
					outstanding[currentLine] = words;
				}
			}
		}
		
		void OnMarkOutstandingRequest(object sender, EventArgs e)
		{
			Dictionary<LineSegment, List<TextWord>> oldOutstanding;
			lock (lockObject) {
				oldOutstanding = outstanding;
				outstanding = new Dictionary<LineSegment, List<TextWord>>();
			}
			// We cannot call MarkLine inside lock(lockObject) because then the main
			// thread could deadlock with the highlighter thread.
			foreach (KeyValuePair<LineSegment, List<TextWord>> pair in oldOutstanding) {
				highlighter.MarkLine(document, pair.Key, pair.Value);
			}
		}
	}
}
