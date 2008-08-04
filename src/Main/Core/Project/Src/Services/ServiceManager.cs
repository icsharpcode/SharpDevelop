// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ICSharpCode.Core.Services
{
	/// <summary>
	/// Maintains a list of services that can be shutdown in the reverse order of their initialization.
	/// Maintains references to the core service implementations.
	/// </summary>
	public static class ServiceManager
	{
		readonly static List<IService> services = new List<IService>();
		
		public static ReadOnlyCollection<IService> Services {
			get {
				return services.AsReadOnly();
			}
		}
		
		public static void StartService(IService service)
		{
			lock (services) {
				services.Add(service);
			}
		}
		
		public static void ShutdownAllServices()
		{
			lock (services) {
				for (int i = services.Count; i >= 0; i--) {
					services[i].Shutdown();
				}
				services.Clear();
			}
		}
		
		static ILoggingService loggingService = new TextWriterLoggingService(new DebugTextWriter());
		
		public static ILoggingService LoggingService {
			get { return loggingService; }
			set {
				if (value == null)
					throw new ArgumentNullException();
				loggingService = value;
			}
		}
		
		static IMessageService messageService = new TextWriterMessageService(Console.Out);
		
		public static IMessageService MessageService {
			get { return messageService; }
			set {
				if (value == null)
					throw new ArgumentNullException();
				messageService = value;
			}
		}
	}
}
