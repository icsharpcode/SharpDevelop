// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;

using ICSharpCode.SharpDevelop;
using ICSharpCode.TextEditor.Document;

namespace CSharpBinding.FormattingStrategy
{
	/// <summary>
	/// Interface used for the indentation class to access the document.
	/// </summary>
	public interface IDocumentAccessor
	{
		/// <summary>Gets if something was changed in the document.</summary>
		bool Dirty { get; }
		/// <summary>Gets if the current line is read only (because it is not in the
		/// selected text region)</summary>
		bool ReadOnly { get; }
		/// <summary>Gets the number of the current line.</summary>
		int LineNumber { get; }
		/// <summary>Gets/Sets the text of the current line.</summary>
		string Text { get; set; }
		/// <summary>Advances to the next line.</summary>
		bool Next();
	}
	
	#region DocumentAccessor
	public sealed class DocumentAccessor : IDocumentAccessor
	{
		IDocument doc;
		
		int minLine;
		int maxLine;
		int changedLines = 0;
		
		public DocumentAccessor(IDocument document)
		{
			doc = document;
			this.minLine = 0;
			this.maxLine = doc.TotalNumberOfLines - 1;
		}
		
		public DocumentAccessor(IDocument document, int minLine, int maxLine)
		{
			doc = document;
			this.minLine = minLine;
			this.maxLine = maxLine;
		}
		
		int num = -1;
		bool dirty;
		string text;
		LineSegment line;
		
		public bool ReadOnly {
			get {
				return num < minLine;
			}
		}
		
		public bool Dirty {
			get {
				return dirty;
			}
		}
		
		public int LineNumber {
			get {
				return num;
			}
		}
		
		public int ChangedLines {
			get {
				return changedLines;
			}
		}
		
		bool lineDirty = false;
		
		public string Text {
			get { return text; }
			set {
				if (num < minLine) return;
				text = value;
				dirty = true;
				lineDirty = true;
			}
		}
		
		public bool Next()
		{
			if (lineDirty) {
				DefaultFormattingStrategy.SmartReplaceLine(doc, line, text);
				lineDirty = false;
				++changedLines;
			}
			++num;
			if (num > maxLine) return false;
			line = doc.GetLineSegment(num);
			text = doc.GetText(line);
			return true;
		}
	}
	#endregion
	
	#region FileAccessor
	public sealed class FileAccessor : IDisposable, IDocumentAccessor
	{
		public bool Dirty {
			get {
				return dirty;
			}
		}
		
		public bool ReadOnly {
			get {
				return false;
			}
		}
		
		FileStream f;
		StreamReader r;
		List<string> lines = new List<string>();
		bool dirty = false;
		
		string filename;
		public FileAccessor(string filename)
		{
			this.filename = filename;
			f = new FileStream(filename, FileMode.Open, FileAccess.Read);
			r = ICSharpCode.TextEditor.Util.FileReader.OpenStream(f, ParserService.DefaultFileEncoding);
		}
		
		int num = 0;
		
		public int LineNumber {
			get { return num; }
		}
		
		
		string text = "";
		
		public string Text {
			get {
				return text;
			}
			set {
				dirty = true;
				text = value;
			}
		}
		
		public bool Next()
		{
			if (num > 0) {
				lines.Add(text);
			}
			text = r.ReadLine();
			++num;
			return text != null;
		}
		
		void IDisposable.Dispose()
		{
			Close();
		}
		
		public void Close()
		{
			System.Text.Encoding encoding = r.CurrentEncoding;
			r.Close();
			f.Close();
			if (dirty) {
				f = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None);
				using (StreamWriter w = new StreamWriter(f, encoding)) {
					foreach (string line in lines) {
						w.WriteLine(line);
					}
				}
				f.Close();
			}
		}
	}
	#endregion
	
	#region StringAccessor
	public sealed class StringAccessor : IDocumentAccessor
	{
		public bool Dirty {
			get {
				return dirty;
			}
		}
		
		public bool ReadOnly {
			get {
				return false;
			}
		}
		
		StringReader r;
		StringWriter w;
		bool dirty = false;
		
		public string CodeOutput {
			get {
				return w.ToString();
			}
		}
		
		public StringAccessor(string code)
		{
			r = new StringReader(code);
			w = new StringWriter();
		}
		
		int num = 0;
		
		public int LineNumber {
			get { return num; }
		}
		
		string text = "";
		
		public string Text {
			get {
				return text;
			}
			set {
				dirty = true;
				text = value;
			}
		}
		
		public bool Next()
		{
			if (num > 0) {
				w.WriteLine(text);
			}
			text = r.ReadLine();
			++num;
			return text != null;
		}
	}
	#endregion
}
