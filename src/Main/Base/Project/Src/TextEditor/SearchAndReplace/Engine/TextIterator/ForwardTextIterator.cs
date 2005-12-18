// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Collections;
using ICSharpCode.TextEditor.Document;

namespace SearchAndReplace
{
	public class ForwardTextIterator : ITextIterator
	{
		ProvidedDocumentInformation info;
		
		enum TextIteratorState {
			Resetted,
			Iterating,
			Done,
		}
		
		TextIteratorState state;
		
		ITextBufferStrategy textBuffer;
		int                 endOffset;
		int                 oldOffset = -1;
		
		public ITextBufferStrategy TextBuffer {
			get {
				return textBuffer;
			}
		}
		
		public char Current {
			get {
				switch (state) {
					case TextIteratorState.Resetted:
						throw new System.InvalidOperationException("Call moveAhead first");
					case TextIteratorState.Iterating:
						return textBuffer.GetCharAt(Position);
					case TextIteratorState.Done:
						throw new System.InvalidOperationException("TextIterator is at the end");
					default:
						throw new System.InvalidOperationException("unknown text iterator state");
				}
			}
		}
		
		int position;
		public int Position {
			get {
				return position;
			}
			set {
				position = value;
			}
		}
		
		public ForwardTextIterator(ProvidedDocumentInformation info)
		{
			Debug.Assert(info != null);
			this.info       = info;
			this.textBuffer = info.TextBuffer;
			this.position   = info.CurrentOffset;
			this.endOffset  = info.EndOffset;
			
			Reset();
		}
		
		public char GetCharRelative(int offset)
		{
			if (state != TextIteratorState.Iterating) {
				throw new System.InvalidOperationException();
			}
			
			int realOffset = (Position + (1 + Math.Abs(offset) / textBuffer.Length) * textBuffer.Length + offset) % textBuffer.Length;
			return textBuffer.GetCharAt(realOffset);
		}
		
		public bool MoveAhead(int numChars)
		{
			Debug.Assert(numChars > 0);
			
			switch (state) {
				case TextIteratorState.Resetted:
					Position = endOffset;
					state = TextIteratorState.Iterating;
					return true;
				case TextIteratorState.Done:
					return false;
				case TextIteratorState.Iterating:
					Position = (Position + numChars) % textBuffer.Length;
					bool finish = oldOffset != -1 && (oldOffset > Position || oldOffset < endOffset) && Position >= endOffset;
					
					oldOffset = Position;
					if (finish) {
						state = TextIteratorState.Done;
						return false;
					}
					return true;
				default:
					throw new Exception("Unknown text iterator state");
			}
		}
		
		public void InformReplace(int offset, int length, int newLength)
		{
			if (offset <= endOffset) {
				endOffset = endOffset - length + newLength;
			}
			
			if (offset <= Position) {
				Position = Position - length + newLength;
			}
			
			if (offset <= oldOffset) {
				oldOffset = oldOffset - length + newLength;
			}
		}

		public void Reset()
		{
			state      = TextIteratorState.Resetted;
			Position   = endOffset;
			oldOffset  = -1;
		}
		
		public override string ToString()
		{
			return String.Format("[ForwardTextIterator: Position={0}, endOffset={1}, state={2}]", Position, endOffset, state);
		}
	}
}
