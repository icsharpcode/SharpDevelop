// Copyright (c) AlphaSierraPapa for the SharpDevelop Team
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

namespace ICSharpCode.Editor
{
// I'm not sure if we need this.
// How about a method in the context - this method could wrap the internal representation.
// public void StartTextLinkMode (int linkLength, IEnumerable<int> offsets)
// and maybe then variations taking more than one link element ?
	
//	/// <summary>
//	/// Represents an element in the text editor that is either editable, or bound to another editable element.
//	/// Used with <see cref="ITextEditor.ShowLinkedElements"/>
//	/// </summary>
//	public class LinkedElement
//	{
//		LinkedElement boundTo;
//		
//		/// <summary>
//		/// Gets/Sets the start offset of this linked element.
//		/// </summary>
//		public int StartOffset { get; set; }
//		
//		/// <summary>
//		/// Gets/Sets the end offset of this linked element.
//		/// </summary>
//		public int EndOffset { get; set; }
//		
//		/// <summary>
//		/// Gets the linked element to which this element is bound.
//		/// </summary>
//		public LinkedElement BoundTo {
//			get { return boundTo; }
//		}
//		
//		/// <summary>
//		/// Gets whether this element is editable. Returns true if this element is not bound.
//		/// </summary>
//		public bool IsEditable {
//			get { return boundTo == null; }
//		}
//		
//		/// <summary>
//		/// Creates a new editable element.
//		/// </summary>
//		public LinkedElement(int startOffset, int endOffset)
//		{
//			this.StartOffset = startOffset;
//			this.EndOffset = endOffset;
//		}
//		
//		/// <summary>
//		/// Creates a new element that is bound to <paramref name="boundTo"/>.
//		/// </summary>
//		public LinkedElement(int startOffset, int endOffset, LinkedElement boundTo)
//		{
//			if (boundTo == null)
//				throw new ArgumentNullException("boundTo");
//			this.StartOffset = startOffset;
//			this.EndOffset = endOffset;
//			while (boundTo.boundTo != null)
//				boundTo = boundTo.boundTo;
//			this.boundTo = boundTo;
//		}
//	}
}
