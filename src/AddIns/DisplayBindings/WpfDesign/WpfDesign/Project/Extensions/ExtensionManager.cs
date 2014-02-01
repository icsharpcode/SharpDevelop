// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
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
			
			context.Services.RunWhenAvailable<IComponentService>(
				delegate(IComponentService componentService) {
					componentService.ComponentRegistered += OnComponentRegistered;
				});
		}
		
		void OnComponentRegistered(object sender, DesignItemEventArgs e)
		{
			e.Item.SetExtensionServers(this, GetExtensionServersForItem(e.Item));
		}
		
		/// <summary>
		/// Re-applies extensions from the ExtensionServer to the specified design items.
		/// </summary>
		void ReapplyExtensions(IEnumerable<DesignItem> items, ExtensionServer server)
		{
			foreach (DesignItem item in items) {
				if (item != null) {
					item.ReapplyExtensionServer(this, server);
				}
			}
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
		
		/// <summary>
		/// Gets all the types of all extensions that are applied to the specified item type.
		/// </summary>
		public IEnumerable<Type> GetExtensionTypes(Type extendedItemType)
		{
			if (extendedItemType == null)
				throw new ArgumentNullException("extendedItemType");
			foreach (ExtensionEntry entry in GetExtensionEntries(extendedItemType)) {
				yield return entry.ExtensionType;
			}
		}
		#endregion
		
		#region Create Extensions
		static readonly ExtensionEntry[] emptyExtensionEntryArray = new ExtensionEntry[0];
		
		IEnumerable<ExtensionEntry> GetExtensionEntries(DesignItem extendedItem)
		{
			if (extendedItem.Component == null)
				return emptyExtensionEntryArray;
			else
				return GetExtensionEntries(extendedItem.ComponentType);
		}
		
		ExtensionServer[] GetExtensionServersForItem(DesignItem item)
		{
			Debug.Assert(item != null);
			
			HashSet<ExtensionServer> servers = new HashSet<ExtensionServer>();
			foreach (ExtensionEntry entry in GetExtensionEntries(item)) {
				servers.Add(entry.Server);
			}
			return servers.ToArray();
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
			
			object[] extensionServerAttributes = extensionType.GetCustomAttributes(typeof(ExtensionServerAttribute), true);
			if (extensionServerAttributes.Length != 1)
				throw new DesignerException("Extension types must have exactly one [ExtensionServer] attribute.");
			
			return GetExtensionServer((ExtensionServerAttribute)extensionServerAttributes[0]);
		}
		
		/// <summary>
		/// Gets the extension server for the specified extension server attribute.
		/// </summary>
		public ExtensionServer GetExtensionServer(ExtensionServerAttribute attribute)
		{
			if (attribute == null)
				throw new ArgumentNullException("attribute");
			
			Type extensionServerType = attribute.ExtensionServerType;
			
			ExtensionServer server;
			if (_extensionServers.TryGetValue(extensionServerType, out server))
				return server;
			
			server = (ExtensionServer)Activator.CreateInstance(extensionServerType);
			server.InitializeExtensionServer(_context);
			_extensionServers[extensionServerType] = server;
			server.ShouldApplyExtensionsInvalidated += delegate(object sender, DesignItemCollectionEventArgs e) {
				ReapplyExtensions(e.Items, (ExtensionServer)sender);
			};
			return server;
		}
		#endregion
		
		#region Special extensions (CustomInstanceFactory and DefaultInitializer)
		static readonly object[] emptyObjectArray = new object[0];
		
		/// <summary>
		/// Create an instance of the specified type using the specified arguments.
		/// The instance is created using a CustomInstanceFactory registered for the type,
		/// or using reflection if no instance factory is found.
		/// </summary>
		public object CreateInstanceWithCustomInstanceFactory(Type instanceType, object[] arguments)
		{
			if (instanceType == null)
				throw new ArgumentNullException("instanceType");
			if (arguments == null)
				arguments = emptyObjectArray;
			foreach (Type extensionType in GetExtensionTypes(instanceType)) {
				if (typeof(CustomInstanceFactory).IsAssignableFrom(extensionType)) {
					CustomInstanceFactory factory = (CustomInstanceFactory)Activator.CreateInstance(extensionType);
					return factory.CreateInstance(instanceType, arguments);
				}
			}
			return CustomInstanceFactory.DefaultInstanceFactory.CreateInstance(instanceType, arguments);
		}
		
		/// <summary>
		/// Applies all DefaultInitializer extensions on the design item.
		/// </summary>
		public void ApplyDefaultInitializers(DesignItem item)
		{
			if (item == null)
				throw new ArgumentNullException("item");
			
			foreach (ExtensionEntry entry in GetExtensionEntries(item)) {
				if (typeof(DefaultInitializer).IsAssignableFrom(entry.ExtensionType)) {
					DefaultInitializer initializer = (DefaultInitializer)Activator.CreateInstance(entry.ExtensionType);
					initializer.InitializeDefaults(item);
				}
			}
		}
		#endregion
	}
}
