// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Reflection;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Threading;

namespace ICSharpCode.SharpDevelop.BuildWorker.Interprocess
{
	/// <summary>
	/// Manages a worker process that communicates with the host using a local TCP connection.
	/// </summary>
	public sealed class WorkerProcess
	{
		object hostObject;
		
		public WorkerProcess(IHostObject hostObject)
		{
			if (hostObject == null)
				throw new ArgumentException("hostObject");
			this.hostObject = hostObject;
		}
		
		TcpListener listener;
		Process process;
		
		TcpClient client;
		PacketSender sender;
		PacketReceiver receiver;
		
		public void Start(ProcessStartInfo info)
		{
			listener = new TcpListener(IPAddress.Loopback, 0);
			listener.Start();
			string argument = ((IPEndPoint)listener.LocalEndpoint).Port.ToString();
			
			string oldArguments = info.Arguments;
			info.Arguments += " " + argument;
			process = Process.Start(info);
			info.Arguments = oldArguments;
			
			SetTimeout();
			listener.BeginAcceptTcpClient(OnAcceptTcpClient, null);
		}
		
		Timer currentTimer;
		
		void SetTimeout()
		{
			lock (this) {
				ClearTimeout();
				currentTimer = new Timer(OnTimeout, null, HostProcess.SendKeepAliveInterval * 3 / 2, -1);
			}
		}
		
		void ClearTimeout()
		{
			lock (this) {
				if (currentTimer != null) {
					currentTimer.Dispose();
					currentTimer = null;
				}
			}
		}
		
		void OnTimeout(object state)
		{
			Program.Log("OnTimeout");
			Kill();
		}
		
		void OnAcceptTcpClient(IAsyncResult ar)
		{
			SetTimeout();
			try {
				client = listener.EndAcceptTcpClient(ar);
			} catch (SocketException) {
				// error connecting
			}
			listener.Stop();
			listener = null;
			
			if (client == null) {
				OnConnectionLost(null, null);
			} else {
				Stream stream = client.GetStream();
				receiver = new PacketReceiver();
				sender = new PacketSender(stream);
				receiver.ConnectionLost += OnConnectionLost;
				receiver.PacketReceived += OnPacketReceived;
				
				receiver.StartReceive(stream);
				OnReady();
			}
		}
		
		public void Shutdown()
		{
			Program.Log("Shutdown");
			OnWorkerLost();
			if (client != null) {
				client.Close();
			}
		}
		
		void OnConnectionLost(object sender, EventArgs e)
		{
			Program.Log("OnConnectionLost");
			SetTimeout();
			OnWorkerLost();
		}
		
		void OnPacketReceived(object sender, PacketReceivedEventArgs e)
		{
			SetTimeout();
			if (e.Packet.Length != 0) {
				MethodCall mc = (MethodCall)DeserializeObject(e.Packet);
				mc.CallOn(hostObject);
			}
		}
		
		public void Kill()
		{
			Program.Log("Kill");
			ClearTimeout();
			OnWorkerLost();
			if (client != null) {
				client.Close();
				client = null;
			}
			if (process != null) {
				try {
					if (!process.HasExited) {
						process.Kill();
					}
				} catch (InvalidOperationException) {
					// may occur when the worker process crashes
				}
				process = null;
			}
		}
		
		int workerIsLost;
		
		void OnReady()
		{
			if (workerIsLost == 1)
				return;
			Program.Log("OnReady");
			if (Ready != null)
				Ready(this, EventArgs.Empty);
		}
		
		void OnWorkerLost()
		{
			if (Interlocked.Exchange(ref workerIsLost, 1) == 1)
				return;
			Program.Log("OnWorkerLost");
			if (WorkerLost != null)
				WorkerLost(this, EventArgs.Empty);
		}
		
		/// <summary>
		/// Occurs when the worker process is ready to execute a job.
		/// </summary>
		public event EventHandler Ready;
		
		/// <summary>
		/// Occurs when the connection to the worker process broke.
		/// </summary>
		public event EventHandler WorkerLost;
		
		public void CallMethodOnWorker(string methodName, params object[] args)
		{
			sender.Send(SerializeObject(new MethodCall(methodName, args)));
		}
		
		internal static MemoryStream SerializeObject(object obj)
		{
			MemoryStream ms = new MemoryStream();
			BinaryFormatter bf = new BinaryFormatter();
			bf.Serialize(ms, obj);
			ms.Position = 0;
			return ms;
		}
		
		internal static object DeserializeObject(byte[] packet)
		{
			BinaryFormatter bf = new BinaryFormatter();
			return bf.Deserialize(new MemoryStream(packet, false));
		}
		
		[Serializable]
		internal class MethodCall
		{
			public readonly string MethodName;
			public readonly object[] Arguments;
			
			public MethodCall(string methodName, object[] arguments)
			{
				this.MethodName = methodName;
				this.Arguments = arguments;
			}
			
			public void CallOn(object target)
			{
				target.GetType().InvokeMember(MethodName,
				                              BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance,
				                              null, target, Arguments);
			}
		}
	}
}
