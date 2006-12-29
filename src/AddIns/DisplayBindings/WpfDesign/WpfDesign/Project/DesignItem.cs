// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using ICSharpCode.WpfDesign.Extensions;

namespace ICSharpCode.WpfDesign
{
	/// <summary>
	/// The DesignItem connects a component with the service system and the designers.
	/// Equivalent to Cider's ModelItem.
	/// </summary>
	public abstract class DesignItem
	{
		/// <summary>
		/// Gets the component this DesignSite was created for.
		/// </summary>
		public abstract object Component { get; }
		
		/// <summary>
		/// Gets the view used for the component.
		/// </summary>
		public abstract UIElement View { get; }
		
		/// <summary>
		/// Gets the design context.
		/// </summary>
		public abstract DesignContext Context { get; }
		
		/// <summary>
		/// Gets an instance that provides convenience properties for the most-used designers.
		/// </summary>
		public ServiceContainer Services {
			[DebuggerStepThrough]
			get { return this.Context.Services; }
		}
		
		#region Extensions support
		private struct ExtensionEntry
		{
			internal readonly Extension Extension;
			internal readonly ExtensionServer Server;
			
			public ExtensionEntry(Extension extension, ExtensionServer server)
			{
				this.Extension = extension;
				this.Server = server;
			}
		}
		
		ExtensionServer[] _extensionServers;
		bool[] _extensionServerIsApplied;
		
		List<ExtensionEntry> _extensions = new List<ExtensionEntry>();
		
		/// <summary>
		/// Gets the extensions registered for this DesignItem.
		/// </summary>
		public IEnumerable<Extension> Extensions {
			get {
				foreach (ExtensionEntry entry in _extensions) {
					yield return entry.Extension;
				}
			}
		}
		
		internal void SetExtensionServers(ExtensionServer[] extensionServers)
		{
			Debug.Assert(_extensionServers == null);
			Debug.Assert(extensionServers != null);
			
			_extensionServers = extensionServers;
			_extensionServerIsApplied = new bool[extensionServers.Length];
		}
		
		internal void ApplyExtensions(ExtensionManager extensionManager)
		{
			Debug.Assert(_extensionServers != null);
			for (int i = 0; i < _extensionServers.Length; i++) {
				bool shouldApply = _extensionServers[i].ShouldApplyExtensions(this);
				if (shouldApply != _extensionServerIsApplied[i]) {
					ExtensionServer server = _extensionServers[i];
					if (shouldApply) {
						// add extensions
						foreach (Extension ext in extensionManager.CreateExtensions(server, this)) {
							_extensions.Add(new ExtensionEntry(ext, server));
						}
					} else {
						// remove extensions
						_extensions.RemoveAll(
							delegate (ExtensionEntry entry) {
								if (entry.Server == server) {
									server.RemoveExtension(entry.Extension);
									return true;
								} else {
									return false;
								}
							});
					}
				}
			}
		}
		#endregion
		
		#region Manage behavior
		Dictionary<Type, object> _behaviorObjects = new Dictionary<Type, object>();
		
		/// <summary>
		/// Adds a bevahior extension object to this design item.
		/// </summary>
		public void AddBehavior(Type bevahiorInterface, object behaviorImplementation)
		{
			if (bevahiorInterface == null)
				throw new ArgumentNullException("bevahiorInterface");
			if (behaviorImplementation == null)
				throw new ArgumentNullException("behaviorImplementation");
			
			_behaviorObjects.Add(bevahiorInterface, behaviorImplementation);
		}
		
		/// <summary>
		/// Gets a bevahior extension object from the design item.
		/// </summary>
		/// <returns>The behavior object, or null if it was not found.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
		public T GetBehavior<T>() where T : class
		{
			object obj;
			_behaviorObjects.TryGetValue(typeof(T), out obj);
			return (T)obj;
		}
		#endregion
	}
}
