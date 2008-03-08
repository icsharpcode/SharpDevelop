// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

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
