// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.IO;
using System.Runtime;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;

namespace ICSharpCode.SharpDevelop.BuildWorker.Interprocess
{
	public sealed class PacketReceiver
	{
		Stream stream;
		byte[] buffer = new byte[10000];
		int bufferReadOffset;
		
		public void StartReceive(Stream stream)
		{
			if (stream == null)
				throw new ArgumentNullException("stream");
			if (this.stream != null)
				throw new InvalidOperationException("StartReceive can be called only once.");
			this.stream = stream;
			try {
				stream.BeginRead(buffer, 0, buffer.Length, OnReceive, null);
			} catch (ObjectDisposedException) {
				OnConnectionLost();
			} catch (IOException) {
				OnConnectionLost();
			}
		}
		
		int maxPacketSize = int.MaxValue - 20000;
		
		/// <summary>
		/// Gets/Sets the maximum allowed packet size in bytes.
		/// </summary>
		public int MaxPacketSize {
			get { return maxPacketSize; }
			set {
				if (value < 1)
					throw new ArgumentOutOfRangeException("value", value, "MaxPacketSize must be >0");
				maxPacketSize = value;
			}
		}
		
		void OnReceive(IAsyncResult ar)
		{
			int bytes;
			try {
				bytes = stream.EndRead(ar);
			} catch (ObjectDisposedException) {
				OnConnectionLost();
				return;
			} catch (IOException) {
				OnConnectionLost();
				return;
			}
			if (bytes == 0) {
				// 0 bytes read indicates the end of the stream
				OnConnectionLost();
				return;
			}
			bufferReadOffset += bytes;
			int packetStart = 0;
			int packetSize = -1;
			while (bufferReadOffset >= packetStart + 4) {
				packetSize = BitConverter.ToInt32(buffer, packetStart);
				if (packetSize < 4)
					throw new ProtocolViolationException("packetSize must be > 4");
				if (packetSize - 4 > MaxPacketSize)
					throw new ProtocolViolationException("packetSize must be smaller than MaxPacketSize");
				if (bufferReadOffset >= packetStart + packetSize) {
					//Debug.WriteLine("receiving packet of size " + packetSize);
					byte[] packet = new byte[packetSize - 4];
					Array.Copy(buffer, packetStart + 4, packet, 0, packet.Length);
					OnPacketReceived(packet);
					packetStart += packetSize;
				} else {
					break;
				}
			}
			if (packetStart != 0) {
				// copy half-received packet to the beginning of the buffer
				int copyAmount = bufferReadOffset - packetStart;
				for (int i = 0; i < copyAmount; i++) {
					buffer[i] = buffer[i + packetStart];
				}
				bufferReadOffset = copyAmount;
			}
			if (packetSize > buffer.Length) {
				Debug.WriteLine("resizing receive buffer for packet of size " + packetSize);
				Array.Resize(ref buffer, Math.Max(packetSize, buffer.Length * 2));
			}
			if (bufferReadOffset >= buffer.Length) {
				// should never happen - the buffer now is large enough to contain the packet,
				// and we would have already processed the packet if received it completely
				throw new InvalidOperationException("trying to read 0 bytes from socket");
			}
			try {
				stream.BeginRead(buffer, bufferReadOffset, buffer.Length - bufferReadOffset, OnReceive, null);
			} catch (ObjectDisposedException) {
				OnConnectionLost();
			} catch (IOException) {
				OnConnectionLost();
			}
		}
		
		void OnConnectionLost()
		{
			if (ConnectionLost != null)
				ConnectionLost(this, EventArgs.Empty);
		}
		
		void OnPacketReceived(byte[] packet)
		{
			if (PacketReceived != null)
				PacketReceived(this, new PacketReceivedEventArgs(packet));
		}
		
		public event EventHandler ConnectionLost;
		public event EventHandler<PacketReceivedEventArgs> PacketReceived;
	}
	
	public class PacketReceivedEventArgs : EventArgs
	{
		byte[] packet;
		
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
		public byte[] Packet {
			get { return packet; }
		}
		
		public PacketReceivedEventArgs(byte[] packet)
		{
			this.packet = packet;
		}
	}
}
