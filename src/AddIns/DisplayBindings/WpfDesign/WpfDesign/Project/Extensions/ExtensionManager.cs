// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Reflection;

namespace ICSharpCode.WpfDesign.Extensions
{
	/// <summary>
	/// Manages extension creation for a design context.
	/// </summary>
	public sealed class ExtensionManager
	{
		readonly DesignContext _context;
		
		internal ExtensionManager(DesignContext context)
		{
			Debug.Assert(context != null);
			this._context = context;
			
			context.Services.Subscribe<IComponentService>(
				delegate(IComponentService componentService) {
					componentService.ComponentRegistered += OnComponentRegistered;
				});
		}
		
		void OnComponentRegistered(object sender, DesignItemEventArgs e)
		{
			e.Item.SetExtensionServers(GetExtensionServersForItem(e.Item));
			e.Item.ApplyExtensions(this);
		}
		
		#region Manage ExtensionEntries
		sealed class ExtensionEntry
		{
			internal readonly Type ExtensionType;
			internal readonly ExtensionServer Server;
			internal readonly Type OverriddenExtensionType;
			
			public ExtensionEntry(Type extensionType, ExtensionServer server, Type overriddenExtensionType)
			{
				this.ExtensionType = extensionType;
				this.Server = server;
				this.OverriddenExtensionType = overriddenExtensionType;
			}
		}
		
		Dictionary<Type, List<ExtensionEntry>> _extensions = new Dictionary<Type, List<ExtensionEntry>>();
		
		void AddExtensionEntry(Type extendedItemType, ExtensionEntry entry)
		{
			List<ExtensionEntry> list;
			if (!_extensions.TryGetValue(extendedItemType, out list)) {
				list = _extensions[extendedItemType] = new List<ExtensionEntry>();
			}
			list.Add(entry);
		}
		
		List<ExtensionEntry> GetExtensionEntries(Type extendedItemType)
		{
			List<ExtensionEntry> result;
			if (extendedItemType.BaseType != null)
				result = GetExtensionEntries(extendedItemType.BaseType);
			else
				result = new List<ExtensionEntry>();
			
			List<ExtensionEntry> list;
			if (_extensions.TryGetValue(extendedItemType, out list)) {
				foreach (ExtensionEntry entry in list) {
					if (entry.OverriddenExtensionType != null) {
						result.RemoveAll(delegate(ExtensionEntry oldEntry) {
						                 	return oldEntry.ExtensionType == entry.OverriddenExtensionType;
						                 });
					}
					result.Add(entry);
				}
			}
			return result;
		}
		#endregion
		
		#region Create Extensions
		static readonly ExtensionEntry[] emptyExtensionEntryArray = new ExtensionEntry[0];
		
		IEnumerable<ExtensionEntry> GetExtensionEntries(DesignItem extendedItem)
		{
			if (extendedItem.Component == null)
				return emptyExtensionEntryArray;
			else
				return GetExtensionEntries(extendedItem.Component.GetType());
		}
		
		ExtensionServer[] GetExtensionServersForItem(DesignItem item)
		{
			Debug.Assert(item != null);
			
			HashSet<ExtensionServer> servers = new HashSet<ExtensionServer>();
			foreach (ExtensionEntry entry in GetExtensionEntries(item)) {
				servers.Add(entry.Server);
			}
			return Linq.ToArray(servers);
		}
		
		internal IEnumerable<Extension> CreateExtensions(ExtensionServer server, DesignItem item)
		{
			Debug.Assert(server != null);
			Debug.Assert(item != null);
			
			foreach (ExtensionEntry entry in GetExtensionEntries(item)) {
				if (entry.Server == server) {
					yield return server.CreateExtension(entry.ExtensionType, item);
				}
			}
		}
		#endregion
		
		#region RegisterAssembly
		HashSet<Assembly> _registeredAssemblies = new HashSet<Assembly>();
		
		/// <summary>
		/// Registers extensions from the specified assembly.
		/// </summary>
		public void RegisterAssembly(Assembly assembly)
		{
			if (assembly == null)
				throw new ArgumentNullException("assembly");
			
//			object[] assemblyAttributes = assembly.GetCustomAttributes(typeof(IsWpfDesignerAssemblyAttribute), false);
//			if (assemblyAttributes.Length == 0)
//				return;
			
			if (!_registeredAssemblies.Add(assembly)) {
				// the assembly already is registered, don't try to register it again.
				return;
			}
			
//			IsWpfDesignerAssemblyAttribute isWpfDesignerAssembly = (IsWpfDesignerAssemblyAttribute)assemblyAttributes[0];
//			foreach (Type type in isWpfDesignerAssembly.UsePrivateReflection ? assembly.GetTypes() : assembly.GetExportedTypes()) {
			foreach (Type type in assembly.GetTypes()) {
				object[] extensionForAttributes = type.GetCustomAttributes(typeof(ExtensionForAttribute), false);
				if (extensionForAttributes.Length == 0)
					continue;
				
				foreach (ExtensionForAttribute designerFor in extensionForAttributes) {
					ExtensionServer server = GetServerForExtension(type);
					AddExtensionEntry(designerFor.DesignedItemType, new ExtensionEntry(type, server, designerFor.OverrideExtension));
				}
			}
		}
		#endregion
		
		#region Extension Server Creation
		// extension server type => extension server instance
		Dictionary<Type, ExtensionServer> _extensionServers = new Dictionary<Type, ExtensionServer>();
		
		ExtensionServer GetServerForExtension(Type extensionType)
		{
			Debug.Assert(extensionType != null);
			
			foreach (ExtensionServerAttribute esa in extensionType.GetCustomAttributes(typeof(ExtensionServerAttribute), true)) {
				return GetExtensionServer(esa);
			}
			throw new DesignerException("Extension types must have a [ExtensionServer] attribute.");
		}
		
		/// <summary>
		/// Gets the extension server for the specified extension server attribute.
		/// </summary>
		public ExtensionServer GetExtensionServer(ExtensionServerAttribute attribute)
		{
			Type extensionServerType = attribute.ExtensionServerType;
			
			ExtensionServer server;
			if (_extensionServers.TryGetValue(extensionServerType, out server))
				return server;
			
			server = (ExtensionServer)Activator.CreateInstance(extensionServerType);
			server.InitializeExtensionServer(_context);
			_extensionServers[extensionServerType] = server;
			return server;
		}
		#endregion
	}
}
