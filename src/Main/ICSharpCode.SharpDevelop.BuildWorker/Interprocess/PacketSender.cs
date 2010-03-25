// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Threading;

namespace ICSharpCode.SharpDevelop.BuildWorker.Interprocess
{
	/// <summary>
	/// Description of PacketSender.
	/// </summary>
	public sealed class PacketSender
	{
		Stream targetStream;
		public PacketSender(Stream stream)
		{
			if (stream == null)
				throw new ArgumentNullException("stream");
			targetStream = stream;
		}
		
		public void Send(byte[] packet)
		{
			if (packet == null)
				throw new ArgumentNullException("packet");
			Send(new MemoryStream(packet, false));
		}
		
		public void Send(Stream stream)
		{
			if (stream == null)
				throw new ArgumentNullException("stream");
			byte[] buffer = new byte[4 + stream.Length];
			unchecked {
				buffer[0] = (byte)buffer.Length;
				buffer[1] = (byte)(buffer.Length >> 8);
				buffer[2] = (byte)(buffer.Length >> 16);
				buffer[3] = (byte)(buffer.Length >> 24);
			}
			int pos = 4;
			int c;
			do {
				c = stream.Read(buffer, pos, buffer.Length - pos);
				pos += c;
			} while (c > 0);
			try {
				targetStream.Write(buffer, 0, buffer.Length);
			} catch (IOException ex) {
				ICSharpCode.SharpDevelop.BuildWorker.Program.Log(ex.ToString());
				OnWriteFailed();
			} catch (ObjectDisposedException ex) {
				ICSharpCode.SharpDevelop.BuildWorker.Program.Log(ex.ToString());
				OnWriteFailed();
			}
		}
		
		void OnWriteFailed()
		{
			if (WriteFailed != null)
				WriteFailed(this, EventArgs.Empty);
		}
		
		public event EventHandler WriteFailed;
	}
}
