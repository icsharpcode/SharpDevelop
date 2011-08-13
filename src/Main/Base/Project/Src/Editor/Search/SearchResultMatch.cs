// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.Editor;
using ICSharpCode.NRefactory;

namespace ICSharpCode.SharpDevelop.Editor.Search
{
	public class SearchResultMatch
	{
		ProvidedDocumentInformation providedDocumentInformation;
		int    offset;
		int    length;
		
		public ProvidedDocumentInformation ProvidedDocumentInformation {
			set { providedDocumentInformation = value; }
		}
		
		public FileName FileName {
			get {
				return providedDocumentInformation.FileName;
			}
		}
		
		public int Offset {
			get {
				return offset;
			}
		}
		
		public int Length {
			get {
				return length;
			}
		}
		
		public virtual string TransformReplacePattern(string pattern)
		{
			return pattern;
		}
		
		public IDocument CreateDocument()
		{
			return providedDocumentInformation.Document;
		}
		
		public SearchResultMatch(int offset, int length)
		{
			if (length < 0)
				throw new ArgumentOutOfRangeException("length");
			if (offset < 0)
				throw new ArgumentOutOfRangeException("offset");
			this.offset   = offset;
			this.length   = length;
		}
		
		public SearchResultMatch(ProvidedDocumentInformation providedDocumentInformation, int offset, int length)
		{
			if (providedDocumentInformation == null)
				throw new ArgumentNullException("providedDocumentInformation");
			if (length < 0)
				throw new ArgumentOutOfRangeException("length");
			if (offset < 0)
				throw new ArgumentOutOfRangeException("offset");
			this.providedDocumentInformation = providedDocumentInformation;
			this.offset   = offset;
			this.length   = length;
		}
		
		public virtual TextLocation GetStartPosition(IDocument document)
		{
			return document.GetLocation(Math.Min(Offset, document.TextLength));
		}
		
		public virtual TextLocation GetEndPosition(IDocument document)
		{
			return document.GetLocation(Math.Min(Offset + Length, document.TextLength));
		}
		
		/// <summary>
		/// Gets a special text to display, or null to display the line's content.
		/// </summary>
		public virtual string DisplayText {
			get {
				return null;
			}
		}
		
		public override string ToString()
		{
			return String.Format("[{3}: FileName={0}, Offset={1}, Length={2}]",
			                     FileName, Offset, Length,
			                     GetType().Name);
		}
	}
	
	public class SimpleSearchResultMatch : SearchResultMatch
	{
		TextLocation position;
		
		public override TextLocation GetStartPosition(IDocument doc)
		{
			return position;
		}
		
		public override TextLocation GetEndPosition(IDocument doc)
		{
			return position;
		}
		
		string displayText;
		
		public override string DisplayText {
			get {
				return displayText;
			}
		}
		
		public SimpleSearchResultMatch(ProvidedDocumentInformation providedDocumentInformation, string displayText, TextLocation position)
			: base(providedDocumentInformation, 0, 0)
		{
			this.position = position;
			this.displayText = displayText;
		}
	}
}
