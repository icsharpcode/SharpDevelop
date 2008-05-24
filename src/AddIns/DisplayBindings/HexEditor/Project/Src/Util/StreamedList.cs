// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision: 2995 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;

using ICSharpCode.Core;

namespace HexEditor.Util
{
	public class StreamedList : IList<byte>
	{
		List<FileStream> streams;
		
		int maxStreamLength = 25 * 1024 * 1024;
		
		public StreamedList()
		{
			this.streams = new List<FileStream>();
			this.streams.Add(new FileStream(Path.GetTempFileName(), FileMode.Create));
		}
		
		~StreamedList()
		{
			this.Clear();
		}
		
		private long GetStreamPosition(long index, out int streamNumber)
		{
			streamNumber = -1;
			
			// Convert 0-based to 1-based.
			index++;
			
			if (index > 255) {
				Console.WriteLine("Test");
			}
			for (int i = 0; i < streams.Count; i++) {
				LoggingService.Info("Länge stream " + i + ": " + streams[i].Length + " current index: " + index);
				if (index <= streams[i].Length) {
					streamNumber = i;
					return index - 1;
				} else {
					index -= streams[i].Length;
				}
			}
			
			LoggingService.Info("Fail!");
			return -1;
		}
		
		private int GetIndex(int position, int stream)
		{
			int index = 0;
			
			for (int i = 0; i < stream; i++) {
				index += (int)streams[i].Length;
			}
			return index + position;
		}
		
		private long GetLength()
		{
			long length = 0;
			foreach (FileStream stream in streams)
			{
				length += stream.Length;
			}
			
			return length;
		}
		
		public byte this[int index]
		{
			get {
				int stream = -1;
				int newIndex = (int)GetStreamPosition((long)index, out stream);
				
				this.streams[stream].Position = newIndex;
				return (byte)this.streams[stream].ReadByte();
			}
			set {
				int stream = -1;
				index = (int)GetStreamPosition((long)index, out stream);
				
				this.streams[stream].Position = index;
				this.streams[stream].WriteByte(value);
			}
		}
		
		public byte[] GetRange(int index, int count)
		{
			if (count == 0)
				return new byte[0];
			int stream = -1;
			int newIndex = (int)GetStreamPosition((long)index, out stream);
			
			this.streams[stream].Position = newIndex;
			
			byte[] data = new byte[count];
			
			if (newIndex + count < this.streams[stream].Length) {
				this.streams[stream].Read(data, 0, count);
			} else {
				int offset = (int)this.streams[stream].Length - newIndex;
				this.streams[stream].Read(data, 0, offset);
				if (stream < this.streams.Count - 1) {
					stream++;
					this.streams[stream].Read(data, offset, count - offset);
				}
			}
			
			return data;
		}
		
		public int Count {
			get {
				return (int)GetLength();
			}
		}
		
		public bool IsReadOnly {
			get {
				return false;
			}
		}
		
		public int IndexOf(byte item)
		{
			for(int i = 0; i < streams.Count; i++)
			{
				for (int j = 0; j < streams[i].Length; j++) {
					streams[i].Position = j;
					if (streams[i].ReadByte() == item) {
						return GetIndex(j, i);
					}
				}
			}
			
			return -1;
		}
		
		public void Insert(int index, byte item)
		{
			int stream = -1;
			index = (int)GetStreamPosition(index, out stream);
			byte[] bytes = CopyTo(stream, index, (int)this.streams[stream].Length - index - 1);
			
			this.streams[stream].Position = index;
			this.streams[stream].WriteByte(item);
			this.streams[stream].SetLength(this.streams[stream].Length + 1);
			this.streams[stream].Write(bytes, 0, bytes.Length);
		}
		
		public void RemoveAt(int index)
		{
			int stream = -1;
			int newIndex = (int)GetStreamPosition(index, out stream);
			if (index == 0)
				newIndex = 0;
			
			byte[] bytes = CopyTo(stream, newIndex + 1, (int)this.streams[stream].Length - newIndex - 1);
			
			this.streams[stream].Position = newIndex;
			this.streams[stream].SetLength(this.streams[stream].Length - 1);
			this.streams[stream].Write(bytes, 0, bytes.Length);
		}
		
		public void RemoveRange(int start, int count)
		{
			int stream = -1;
			int newIndex = (int)GetStreamPosition(start, out stream);
			if (start == 0)
				newIndex = 0;
			
			byte[] bytes = CopyTo(stream, newIndex + count, (int)this.streams[stream].Length - newIndex - count);
			
			this.streams[stream].Position = newIndex;
			this.streams[stream].SetLength(this.streams[stream].Length - count);
			this.streams[stream].Write(bytes, 0, bytes.Length);
		}
		
		public void Add(byte item)
		{
			if (this.streams[this.streams.Count - 1].Length > maxStreamLength) {
				this.streams.Add(new FileStream(Path.GetTempFileName(), FileMode.Create));
			}
			
			this.streams[this.streams.Count - 1].Position
				= this.streams[this.streams.Count - 1].Length;
			this.streams[this.streams.Count - 1].WriteByte(item);
		}
		
		public void Clear()
		{
			foreach (FileStream s in streams)
			{
				s.Close();
				File.Delete(s.Name);
			}
			this.streams.Clear();
		}
		
		public bool Contains(byte item)
		{
			throw new NotImplementedException();
		}
		
		public void CopyTo(byte[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}
		
		private byte[] CopyTo(int stream, int start, int count)
		{
			byte[] data = new byte[count];
			
			this.streams[stream].Position = start;
			this.streams[stream].Read(data, 0, count);
			
			return data;
		}
		
		public bool Remove(byte item)
		{
			throw new NotImplementedException();
		}
		
		public IEnumerator<byte> GetEnumerator()
		{
			throw new NotImplementedException();
		}
		
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}
		
		public void AddRange(byte[] data)
		{
			if (this.streams[this.streams.Count - 1].Length > maxStreamLength) {
				LoggingService.Info("Stream added!");
				this.streams.Add(new FileStream(Path.GetTempFileName(), FileMode.Create));
			}
						
			this.streams[this.streams.Count - 1].Position
				= this.streams[this.streams.Count - 1].Length;
			this.streams[this.streams.Count - 1].SetLength(this.streams[this.streams.Count - 1].Length + data.Length);
			this.streams[this.streams.Count - 1].Write(data, 0, data.Length);
			LoggingService.Info("Wrote " + data.Length + " bytes");
		}
		
		internal void AddRange(byte[] data, int readBytes)
		{
			if (this.streams[this.streams.Count - 1].Length > maxStreamLength) {
				LoggingService.Info("Stream added!");
				this.streams.Add(new FileStream(Path.GetTempFileName(), FileMode.Create));
			}
						
			this.streams[this.streams.Count - 1].Position
				= this.streams[this.streams.Count - 1].Length;
			this.streams[this.streams.Count - 1].SetLength(this.streams[this.streams.Count - 1].Length + readBytes);
			this.streams[this.streams.Count - 1].Write(data, 0, readBytes);
			LoggingService.Info("Wrote " + readBytes + " bytes");
		}
		
		public void ConcatenateTo(Stream stream)
		{
			stream.SetLength(this.Count);
			foreach (FileStream fs in this.streams)
			{
				fs.Position = 0;
				byte[] tmp = new byte[fs.Length];
				
				fs.Read(tmp, 0, tmp.Length);
				
				stream.Write(tmp, 0, tmp.Length);
			}
		}
	}
}

