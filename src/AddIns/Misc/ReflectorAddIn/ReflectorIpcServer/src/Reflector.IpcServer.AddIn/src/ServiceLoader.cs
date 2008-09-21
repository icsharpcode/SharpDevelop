// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Christian Hornung" email="chhornung@googlemail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;

namespace Reflector.IpcServer.AddIn
{
	/// <summary>
	/// Initializes our remoting service when the AddIn is loaded in Reflector.
	/// </summary>
	public sealed class ServiceLoader : IPackage
	{
		public ServiceLoader()
		{
		}
		
		ReflectorService serviceInstance;
		IpcServerChannel channel;
		
		public void Load(IServiceProvider serviceProvider)
		{
			try {
				TryLoad(serviceProvider);
			} catch (RemotingException ex) {
				((IWindowManager)serviceProvider.GetService(typeof(IWindowManager))).ShowMessage(String.Concat("ReflectorAddIn: Failed to initialize remoting service.", Environment.NewLine, Environment.NewLine, "Exception:", Environment.NewLine, ex.ToString()));
			}
		}
		
		void TryLoad(IServiceProvider serviceProvider)
		{
			const int tryCount = 20;
			const int waitTimeMS = 500;
			
			for (int i = 0; i < tryCount; ++i) {
				bool success = false;
				
				try {
					
					serviceInstance = new ReflectorService(serviceProvider);
					
					Dictionary<string, object> props = new Dictionary<string, object>();
					props.Add("name", "Reflector IPC server");
					props.Add("secure", true);
					props.Add("portName", "ReflectorService");
					// The following line is needed to fix an "access denied" error
					// when Reflector is restarted within a certain amount of time
					props.Add("exclusiveAddressUse", false);
					
					channel = new IpcServerChannel(props, null);
					ChannelServices.RegisterChannel(channel, true);
					
					RemotingServices.Marshal(serviceInstance, "ReflectorService.rem");
					
					success = true;
					return;
					
				} catch (RemotingException) {
					if (!((i + 1) < tryCount)) {
						throw;
					}
				} finally {
					if (!success) {
						Unload();
					}
				}
				
				System.Threading.Thread.Sleep(waitTimeMS);
			}
		}
		
		public void Unload()
		{
			if (serviceInstance != null) {
				try {
					serviceInstance.Dispose();
				} catch {
				}
				try {
					RemotingServices.Disconnect(serviceInstance);
				} catch (RemotingException) {
				}
				serviceInstance = null;
			}
			if (channel != null) {
				try {
					ChannelServices.UnregisterChannel(channel);
				} catch (RemotingException) {
				}
				channel = null;
			}
		}
	}
}
