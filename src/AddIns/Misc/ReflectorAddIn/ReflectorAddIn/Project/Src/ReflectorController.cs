// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Christian Hornung" email="chhornung@googlemail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Text;

using ICSharpCode.Core;

using Reflector.IpcServer;

namespace ReflectorAddIn
{
	/// <summary>
	/// Controls Lutz Roeders's .NET Reflector.
	/// </summary>
	public static class ReflectorController
	{
		#region Connecting
		
		public static IReflectorService SafeConnect(System.Windows.Forms.IWin32Window owner)
		{
			try {
				return Connect(owner);
			} catch (RemotingException ex) {
				ShowRemotingError(ex);
			}
			return null;
		}
		
		public static void ShowRemotingError(Exception ex)
		{
			MessageService.ShowError(String.Concat(ResourceService.GetString("ReflectorAddIn.ReflectorRemotingFailed"), Environment.NewLine, UnwindExceptionMessages(ex)));
		}
		
		static string UnwindExceptionMessages(Exception ex)
		{
			StringBuilder sb = new StringBuilder();
			do {
				if (sb.Length > 0) sb.AppendLine();
				sb.Append(ex.Message);
			} while ((ex = ex.InnerException) != null);
			return sb.ToString();
		}
		
		/// <summary>
		/// Ensures that an instance of Reflector is running and connects to it.
		/// </summary>
		/// <returns>The <see cref="IReflectorService"/> that can be used to control the running instance of Reflector.</returns>
		public static IReflectorService Connect(System.Windows.Forms.IWin32Window owner)
		{
			IReflectorService service = null;
			try {
				service = TryConnect();
			} catch (RemotingException) {
			}
			if (service != null) return service;
			
			// Get Reflector path and set it up
			string reflectorExeFullPath = ReflectorSetupHelper.GetReflectorExeFullPathInteractive(owner);
			if (reflectorExeFullPath == null) return null;
			if (!ReflectorSetupHelper.SetupReflector(reflectorExeFullPath)) return null;
			
			// start Reflector
			ProcessStartInfo psi = new ProcessStartInfo(reflectorExeFullPath);
			psi.WorkingDirectory = Path.GetDirectoryName(psi.FileName);
			using(Process p = Process.Start(psi)) {
				p.WaitForInputIdle(7500);
			}
			
			Exception lastException = null;
			DateTime start = DateTime.UtcNow;
			do {
				try {
					service = TryConnect();
					if (service != null) return service;
				} catch (RemotingException ex) {
					lastException = ex;
				}
				System.Threading.Thread.Sleep(250);
			} while ((DateTime.UtcNow - start).TotalSeconds <= 10);
			
			if (lastException != null) {
				throw lastException;
			}
			
			return null;
		}
		
		static IReflectorService TryConnect()
		{
			EnsureRemotingInitialized();
			IReflectorService service = RemotingServices.Connect(typeof(IReflectorService), "ipc://ReflectorService/ReflectorService.rem") as IReflectorService;
			if (service == null || !service.CheckIsThere()) {
				throw new RemotingException("Error connecting to Reflector.");
			}
			return service;
		}
		
		public static void Disconnect(IReflectorService service)
		{
			try {
				RemotingServices.Disconnect((MarshalByRefObject)service);
			} catch (RemotingException) {
			}
		}
		
		#endregion
		
		// ********************************************************************************************************************************
		
		#region Remoting initialization
		
		static IpcClientChannel channel;
		
		static void EnsureRemotingInitialized()
		{
			if (channel == null) {
				
				bool success = false;
				
				try {
					
					Dictionary<string, object> props = new Dictionary<string, object>();
					props.Add("name", "Reflector IPC client");
					props.Add("secure", true);
					props.Add("connectionTimeout", 2000);
					
					channel = new IpcClientChannel(props, null);
					ChannelServices.RegisterChannel(channel, true);
					
					success = true;
					
				} finally {
					if (!success && channel != null) {
						try {
							ChannelServices.UnregisterChannel(channel);
						} catch (RemotingException) {
						}
						channel = null;
					}
				}
				
			}
		}
		
		#endregion
		
		// ********************************************************************************************************************************
		
		public static void TryGoTo(CodeElementInfo element, System.Windows.Forms.IWin32Window owner)
		{
			IReflectorService s = null;
			try {
				
				s = SafeConnect(owner);
				if (s == null) return;
				
				s.GoTo(element);
				
			} catch (System.Runtime.Remoting.RemotingException ex) {
				ShowRemotingError(ex);
			} finally {
				if (s != null) {
					Disconnect(s);
				}
			}
		}
	}
}
