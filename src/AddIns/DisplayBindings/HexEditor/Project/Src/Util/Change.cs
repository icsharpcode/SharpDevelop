// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision: 2984 $</version>
// </file>

using System;

namespace HexEditor.Util
{
	/// <summary>
	/// Description of Change.
	/// </summary>
	public class Change
	{
		int start, end;
		byte[] data;
		ChangeType type;
		
		public Change(int start, int end, byte[] data, ChangeType type)
		{
			this.start = start;
			this.end = end;
			this.data = data;
			this.type = type;
		}
		
		public int Start {
			get { return start; }
			set { start = value; }
		}
		
		public int End {
			get { return end; }
			set { end = value; }
		}
		
		public byte[] Data {
			get { return data; }
			set { data = value; }
		}
		
		public ChangeType Type {
			get { return type; }
			set { type = value; }
		}
		
		public void Merge(Change change)
		{
			byte[] tmp = new byte[0];
			
			switch (change.type) {
				case ChangeType.Delete:
					
					break;
				case ChangeType.Insert:
					if ((change.start + change.data.Length) < this.start)
						this.Prepend(change);
					else
						if ((change.start + change.data.Length) > this.end)
						this.Append(change);
					else {
						tmp = new byte[change.data.Length + this.data.Length];
						
						for (int i = 0; i < (int)Math.Abs(change.start - this.start); i++)
						{
							tmp[i] = this.data[i];
						}
						
						int offset = (int)Math.Abs(change.start - this.start);
						
						for (int i = offset; i < offset + change.data.Length; i++)
						{
							tmp[i] = change.data[i - offset];
						}
						
						for (int i = offset + change.data.Length; i < (this.data.Length + offset + change.data.Length); i++)
						{
							tmp[i] = this.data[i - (offset + change.data.Length)];
						}
					}					
					break;
				case ChangeType.Overwrite:
					
					break;
			}
			
			this.data = tmp;
		}
		
		public void Append(Change change)
		{
			byte[] tmp = new byte[change.data.Length + this.data.Length];
			
			for (int i = 0; i < this.data.Length; i++)
			{
				tmp[i] = this.data[i];
			}
			
			for (int i = 0; i < change.data.Length; i++)
			{
				tmp[i + this.data.Length] = change.data[i];
			}
			
			this.end = change.end;
			
			this.data = tmp;
		}
		
		public void Prepend(Change change)
		{
			byte[] tmp = new byte[change.data.Length + this.data.Length];
			
			for (int i = 0; i < change.data.Length; i++)
			{
				tmp[i] = change.data[i];
			}
			
			for (int i = 0; i < this.data.Length; i++)
			{
				tmp[i + change.data.Length] = this.data[i];
			}
			
			this.start = change.start;
			
			this.data = tmp;
		}
	}
	
	public enum ChangeType {
		Delete, Insert, Overwrite
	}
}
