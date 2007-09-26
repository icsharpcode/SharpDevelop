// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Reflection;

namespace ICSharpCode.SharpDevelop.BuildWorker.Interprocess
{
	/// <summary>
	/// Used in the worker process to refer to the host.
	/// </summary>
	public class HostProcess
	{
		object workerObject;
		
		public HostProcess(object workerObject)
		{
			this.workerObject = workerObject;
		}
		
		TcpClient client;
		PacketReceiver receiver;
		PacketSender sender;
		
		ManualResetEvent shutdownEvent;
		bool connectionIsLost;
		
		internal const int SendKeepAliveInterval = 10000;
		
		public void WorkerProcessMain(string argument)
		{
			int port = int.Parse(argument);
			
			client = new TcpClient();
			client.Connect(new IPEndPoint(IPAddress.Loopback, port));
			Stream stream = client.GetStream();
			receiver = new PacketReceiver();
			sender = new PacketSender(stream);
			shutdownEvent = new ManualResetEvent(false);
			receiver.ConnectionLost += OnConnectionLost;
			receiver.PacketReceived += OnPacketReceived;
			sender.WriteFailed += OnConnectionLost;
			
			receiver.StartReceive(stream);
			while (!shutdownEvent.WaitOne(SendKeepAliveInterval, false)) {
				Program.Log("Sending keep-alive packet");
				sender.Send(new byte[0]);
			}
			
			Program.Log("Closing client (end of WorkerProcessMain)");
			client.Close();
			shutdownEvent.Close();
		}
		
		public void Disconnect()
		{
			client.Close();
		}
		
		public void CallMethodOnHost(string methodName, params object[] args)
		{
			sender.Send(WorkerProcess.SerializeObject(new WorkerProcess.MethodCall(methodName, args)));
		}
		
		void OnConnectionLost(object sender, EventArgs e)
		{
			lock (this) {
				if (connectionIsLost)
					return;
				Program.Log("OnConnectionLost");
				connectionIsLost = true;
				shutdownEvent.Set();
			}
		}
		
		void OnPacketReceived(object sender, PacketReceivedEventArgs e)
		{
			Program.Log("OnPacketReceived");
			if (e.Packet.Length != 0) {
				try {
					WorkerProcess.MethodCall mc = (WorkerProcess.MethodCall)WorkerProcess.DeserializeObject(e.Packet);
					mc.CallOn(workerObject);
				} catch (TargetInvocationException ex) {
					Program.Log(ex.ToString());
					CallMethodOnHost("ReportException", ex.InnerException);
				} catch (Exception ex) {
					Program.Log(ex.ToString());
					CallMethodOnHost("ReportException", ex);
				}
			}
		}
	}
}
