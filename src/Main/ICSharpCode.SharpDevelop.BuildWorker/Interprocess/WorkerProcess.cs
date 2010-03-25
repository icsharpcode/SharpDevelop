// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Threading;

namespace ICSharpCode.SharpDevelop.BuildWorker.Interprocess
{
	/// <summary>
	/// Manages a worker process that communicates with the host using a local TCP connection.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
	public sealed class WorkerProcess
	{
		readonly object hostObject;
		
		public WorkerProcess(IHostObject hostObject)
		{
			if (hostObject == null)
				throw new ArgumentNullException("hostObject");
			this.hostObject = hostObject;
		}
		
		TcpListener listener;
		Process process;
		
		TcpClient client;
		PacketSender sender;
		PacketReceiver receiver;
		
		// We use TCP, so we'll have to deal with "attackers" trying to hijack our connection.
		// There are two lines of defense:
		// 1. the only local connections are accepted
		// 2. the client must authenticate with a random password
		// We're not protecting against DOS - we're only vulnerable to local attackers during a short
		// time frame. The password is only to defend against deserializing arbitrary objects from
		// unauthenticated clients.
		// We probably should use Named Pipes instead of TCP.
		byte[] password;
		bool clientAuthenticated;
		
		public void Start(ProcessStartInfo info)
		{
			if (info == null)
				throw new ArgumentNullException("info");
			
			password = new byte[16];
			new RNGCryptoServiceProvider().GetBytes(password);
			
			listener = new TcpListener(IPAddress.Loopback, 0);
			listener.Start();
			string argument = ((IPEndPoint)listener.LocalEndpoint).Port.ToString(CultureInfo.InvariantCulture);
			
			string oldArguments = info.Arguments;
			info.Arguments += " " + argument + " " + Convert.ToBase64String(password);
			process = Process.Start(info);
			// "manual process start" - useful for profiling the build worker
			//System.Windows.Forms.MessageBox.Show(info.Arguments);
			//process = Process.GetProcessesByName("ICSharpCode.SharpDevelop.BuildWorker")[0];
			
			info.Arguments = oldArguments;
			
			SetTimeout();
			listener.BeginAcceptTcpClient(OnAcceptTcpClient, null);
		}
		
		Timer currentTimer;
		
		void SetTimeout()
		{
			lock (this) {
				ClearTimeout();
				currentTimer = new Timer(OnTimeout, null, HostProcess.SendKeepAliveInterval * 5 / 2, -1);
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
			clientAuthenticated = false;
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
				if (clientAuthenticated) {
					MethodCall mc = (MethodCall)DeserializeObject(e.Packet);
					mc.CallOn(hostObject);
				} else {
					if (ArrayEquals(e.Packet, password)) {
						clientAuthenticated = true;
					} else {
						Kill();
						throw new InvalidOperationException("Worker process authentication failed.");
					}
				}
			}
		}
		
		static bool ArrayEquals(byte[] a, byte[] b)
		{
			if (a.Length != b.Length)
				return false;
			for (int i = 0; i < a.Length; i++) {
				if (a[i] != b[i])
					return false;
			}
			return true;
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
			Debug.WriteLine("CallMethodOnWorker: " + methodName);
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
				                              null, target, Arguments, CultureInfo.InvariantCulture);
			}
		}
	}
}
