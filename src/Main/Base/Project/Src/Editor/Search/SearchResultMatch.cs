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
		int offset;
		int length;
		TextLocation startLocation;
		TextLocation endLocation;
		
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
				if (offset < 0)
					offset = providedDocumentInformation.Document.GetOffset(startLocation);
				return offset;
			}
		}
		
		public int Length {
			get {
				if (length < 0)
					length = providedDocumentInformation.Document.GetOffset(endLocation) - this.Offset;
				return length;
			}
		}
		
		public TextLocation StartLocation {
			get { 
				if (startLocation.IsEmpty)
					startLocation = providedDocumentInformation.Document.GetLocation(offset);
				return startLocation; 
			}
		}
		
		public TextLocation EndLocation {
			get { 
				if (endLocation.IsEmpty)
					endLocation = providedDocumentInformation.Document.GetLocation(offset + length);
				return endLocation; 
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
		
		
		public SearchResultMatch(ProvidedDocumentInformation providedDocumentInformation, TextLocation startLocation, TextLocation endLocation)
		{
			if (providedDocumentInformation == null)
				throw new ArgumentNullException("providedDocumentInformation");
			if (length < 0)
				throw new ArgumentOutOfRangeException("length");
			this.offset = -1;
			this.length = -1;
			this.providedDocumentInformation = providedDocumentInformation;
			this.startLocation = startLocation;
			this.endLocation = endLocation;
		}
		
		[Obsolete("Use the StartLocation property instead")]
		public virtual TextLocation GetStartPosition(IDocument document)
		{
			return document.GetLocation(Math.Min(Offset, document.TextLength));
		}
		
		[Obsolete("Use the EndLocation property instead")]
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
		string displayText;
		
		public override string DisplayText {
			get {
				return displayText;
			}
		}
		
		public SimpleSearchResultMatch(ProvidedDocumentInformation providedDocumentInformation, string displayText, TextLocation position)
			: base(providedDocumentInformation, position, position)
		{
			this.displayText = displayText;
		}
	}
}
