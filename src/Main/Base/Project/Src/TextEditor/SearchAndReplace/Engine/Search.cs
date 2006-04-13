// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Diagnostics;

using ICSharpCode.SharpDevelop.Gui;

namespace SearchAndReplace
{
	public class Search
	{
		ISearchStrategy             searchStrategy      = null;
		IDocumentIterator           documentIterator    = null;
		ITextIterator               textIterator        = null;
		ITextIteratorBuilder        textIteratorBuilder = null;
		ProvidedDocumentInformation info = null;
		
		public ProvidedDocumentInformation CurrentDocumentInformation {
			get {
				return info;
			}
		}
		
		public ITextIteratorBuilder TextIteratorBuilder {
			get {
				return textIteratorBuilder;
			}
			set {
				textIteratorBuilder = value;
			}
		}
		
		public ITextIterator TextIterator {
			get {
				return textIterator;
			}
		}
		
		public ISearchStrategy SearchStrategy {
			get {
				return searchStrategy;
			}
			set {
				searchStrategy = value;
			}
		}
		
		public IDocumentIterator DocumentIterator {
			get {
				return documentIterator;
			}
			set {
				documentIterator = value;
			}
		}
		
		SearchResult CreateNamedSearchResult(SearchResult pos)
		{
			if (info == null || pos == null) {
				return null;
			}
			pos.ProvidedDocumentInformation = info;
			return pos;
		}
		
		public void Reset()
		{
			documentIterator.Reset();
			textIterator = null;
		}
		
		public void Replace(int offset, int length, string pattern)
		{
			if (CurrentDocumentInformation != null && TextIterator != null) {
				CurrentDocumentInformation.Replace(offset, length, pattern);
				TextIterator.InformReplace(offset, length, pattern.Length);
			}
		}
		
		public SearchResult FindNext() 
		{
			// insanity check
			Debug.Assert(searchStrategy      != null);
			Debug.Assert(documentIterator    != null);
			Debug.Assert(textIteratorBuilder != null);
			
			if (info != null && textIterator != null && documentIterator.CurrentFileName != null) {
				if (info.FileName != documentIterator.CurrentFileName) { // create new iterator, if document changed
					info         = documentIterator.Current;
					textIterator = textIteratorBuilder.BuildTextIterator(info);
				} else { // old document -> initialize iterator position to caret pos
					textIterator.Position = info.CurrentOffset;
				}
				
				SearchResult result = CreateNamedSearchResult(searchStrategy.FindNext(textIterator));
				if (result != null) {
					info.CurrentOffset = textIterator.Position;
					return result;
				}
			}
			
			// not found or first start -> move forward to the next document
			if (documentIterator.MoveForward()) {
				info = documentIterator.Current;
				// document is valid for searching -> set iterator & fileName
				if (info != null && info.TextBuffer != null && info.EndOffset >= 0 && info.EndOffset < info.TextBuffer.Length) {
					textIterator = textIteratorBuilder.BuildTextIterator(info);
				} else {
					textIterator = null;
				}
				
				return FindNext();
			}
			return null;
		}
		
		public SearchResult FindNext(int offset, int length)
		{
			if (info != null && textIterator != null && documentIterator.CurrentFileName != null) {
				if (info.FileName != documentIterator.CurrentFileName) { // create new iterator, if document changed
					info         = documentIterator.Current;
					textIterator = textIteratorBuilder.BuildTextIterator(info);
				} else { // old document -> initialize iterator position to caret pos
					textIterator.Position = info.CurrentOffset;
				}
				
				SearchResult result = CreateNamedSearchResult(searchStrategy.FindNext(textIterator, offset, length));
				if (result != null) {
					info.CurrentOffset = textIterator.Position;
					return result;
				}
			}
			
			// not found or first start -> move forward to the next document
			if (documentIterator.MoveForward()) {
				info = documentIterator.Current;
				// document is valid for searching -> set iterator & fileName
				if (info != null && info.TextBuffer != null && info.EndOffset >= 0 && info.EndOffset < info.TextBuffer.Length) {
					textIterator = textIteratorBuilder.BuildTextIterator(info);
				} else {
					textIterator = null;
				}
				
				return FindNext(offset, length);
			}
			return null;
		}
	}
}
