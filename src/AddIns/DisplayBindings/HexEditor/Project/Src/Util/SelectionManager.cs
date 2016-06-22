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
using System.Text;

namespace HexEditor.Util
{
	/// <summary>
	/// Contains data for the selection of the HexEditor
	/// </summary>
	public class SelectionManager
	{
		BufferManager buffer;
		
		public event EventHandler SelectionChanged;
		
		protected virtual void OnSelectionChanged(EventArgs e)
		{
			if (SelectionChanged != null) {
				SelectionChanged(this, e);
			}
		}
		
		public SelectionManager(ref BufferManager buffer)
		{
			this.buffer = buffer;
		}
		
		#region Properties
		int start, end;
		bool hasSelection;
		
		public int Start {
			get { return start; }
			set {
				start = value;
				EventArgs e = new EventArgs();
				OnSelectionChanged(e);
			}
		}
		
		public int End {
			get { return end; }
			set {
				end = value;
				EventArgs e = new EventArgs();
				OnSelectionChanged(e);
			}
		}
		
		public bool HasSomethingSelected {
			get { return hasSelection; }
			set { hasSelection = value; }
		}
		
		#endregion
		
		#region Methods
		/// <summary>
		/// 
		/// </summary>
		/// <returns>A string with the selected text</returns>
		public string SelectionText {
			get {
				int start = this.Start;
				int end = this.End;
				if (this.End < this.Start) {
					start = this.End;
					end = this.Start;
				}
				return Encoding.Default.GetString(buffer.GetBytes(this.Start, Math.Abs(end - start)));
			}
		}
		
		public byte[] GetSelectionBytes()
		{
			int start = this.Start;
			int end = this.End;
			if (this.End < this.Start) {
				start = this.End;
				end = this.Start;
			}
			return buffer.GetBytes(this.Start, Math.Abs(end - start));
		}
		
		/// <summary>
		/// Cleares the selection (no text is altered)
		/// </summary>
		public void Clear()
		{
			this.HasSomethingSelected = false;
			this.Start = 0;
			this.End = 0;
		}
		#endregion
	}
}
