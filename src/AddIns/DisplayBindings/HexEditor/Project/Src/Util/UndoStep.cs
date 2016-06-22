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

namespace HexEditor.Util
{
	/// <summary>
	/// Used to save data about changes to undo/redo.
	/// </summary>
	public class UndoStep
	{
		byte[] bytes, oldBytes;
		int start;
		long timeStamp;
		UndoAction action;
		
		public UndoStep(byte[] bytes, byte[] oldBytes, int start, UndoAction action)
		{
			this.bytes = bytes;
			this.oldBytes = oldBytes;
			this.start = start;
			this.action = action;
			this.timeStamp = DateTime.Now.Ticks;
		}
		
		public long TimeStamp {
			get { return timeStamp; }
		}
		
		public byte[] GetBytes() {
			return bytes;
		}
		
		public byte[] GetOldBytes() {
			return oldBytes;
		}
		
		public int Start {
			get { return start; }
		}
		
		public UndoAction Action {
			get { return action; }
		}
		
		public override string ToString()
		{
			return "{ Bytes: " + System.Text.Encoding.Default.GetString(GetBytes()) + ", OldBytes: " + System.Text.Encoding.Default.GetString(GetOldBytes()) + ", Start: " + start.ToString() + ", Action: " + action.ToString() + " }";
		}
	}
}
