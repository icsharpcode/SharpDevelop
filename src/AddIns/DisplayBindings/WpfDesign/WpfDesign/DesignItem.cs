// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 3283 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.ComponentModel;
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
		/// Gets the component type of this design site.
		/// This value may be different from Component.GetType() if a CustomInstanceFactory created
		/// an object using a different type (e.g. ComponentType=Window but Component.GetType()=WindowClone).
		/// </summary>
		public abstract Type ComponentType { get; }

		/// <summary>
		/// Gets the view used for the component.
		/// </summary>
		public abstract UIElement View { get; }

		/// <summary>
		/// Gets the design context.
		/// </summary>
		public abstract DesignContext Context { get; }

		/// <summary>
		/// Gets the parent design item.
		/// </summary>
		public abstract DesignItem Parent { get; }

		/// <summary>
		/// Gets the property where this DesignItem is used as a value.
		/// </summary>
		public abstract DesignItemProperty ParentProperty { get; }

		/// <summary>
		/// Gets properties set on the design item.
		/// </summary>
		public abstract DesignItemPropertyCollection Properties { get; }

		/// <summary>
		/// Gets/Sets the name of the design item.
		/// </summary>
		public abstract string Name { get; set; }

		public abstract DesignItemProperty Content { get; }

		public string ContentPropertyName
		{
			get
			{
				if (Content != null) {
					return Content.Name;
				}
				return null;
			}
		}

		/// <summary>
		/// Opens a new change group used to batch several changes.
		/// ChangeGroups work as transactions and are used to support the Undo/Redo system.
		/// Note: the ChangeGroup applies to the whole <see cref="DesignContext"/>, not just to
		/// this item!
		/// </summary>
		public IChangeGroup OpenGroup(string changeGroupTitle)
		{
			return Context.UndoService.OpenGroup(changeGroupTitle, new DesignItem[] { this });
		}

		/// <summary>
		/// This event is raised whenever a model property on the DesignItem changes.
		/// </summary>
		public abstract event ModelChangedEventHandler ModelChanged;

		public override string ToString()
		{
			return GetType().Name + ": " + ComponentType.Name;
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
		public IEnumerable<Extension> Extensions
		{
			get
			{
				foreach (ExtensionEntry entry in _extensions) {
					yield return entry.Extension;
				}
			}
		}

		internal void SetExtensionServers(ExtensionManager extensionManager, ExtensionServer[] extensionServers)
		{
			Debug.Assert(_extensionServers == null);
			Debug.Assert(extensionServers != null);

			_extensionServers = extensionServers;
			_extensionServerIsApplied = new bool[extensionServers.Length];

			for (int i = 0; i < _extensionServers.Length; i++) {
				bool shouldApply = _extensionServers[i].ShouldApplyExtensions(this);
				if (shouldApply != _extensionServerIsApplied[i]) {
					_extensionServerIsApplied[i] = shouldApply;
					ApplyUnapplyExtensionServer(extensionManager, shouldApply, _extensionServers[i]);
				}
			}
		}

		internal void ReapplyExtensionServer(ExtensionManager extensionManager, ExtensionServer server)
		{
			Debug.Assert(_extensionServers != null);

			for (int i = 0; i < _extensionServers.Length; i++) {
				if (_extensionServers[i] == server) {
					bool shouldApply = server.ShouldApplyExtensions(this);
					if (shouldApply != _extensionServerIsApplied[i]) {
						_extensionServerIsApplied[i] = shouldApply;
						ApplyUnapplyExtensionServer(extensionManager, shouldApply, server);
					}
				}
			}
		}

		private void ApplyUnapplyExtensionServer(ExtensionManager extensionManager, bool shouldApply, ExtensionServer server)
		{
			if (shouldApply) {
				// add extensions
				foreach (Extension ext in extensionManager.CreateExtensions(server, this)) {
					_extensions.Add(new ExtensionEntry(ext, server));
				}
			}
			else {
				// remove extensions
				_extensions.RemoveAll(
					delegate(ExtensionEntry entry) {
						if (entry.Server == server) {
							server.RemoveExtension(entry.Extension);
							return true;
						}
						else {
							return false;
						}
					});
			}
		}
		#endregion

		#region Manage behavior
		readonly Dictionary<Type, object> _behaviorObjects = new Dictionary<Type, object>();

		/// <summary>
		/// Adds a bevahior extension object to this design item.
		/// </summary>
		public void AddBehavior(Type bevahiorInterface, object behaviorImplementation)
		{
			if (bevahiorInterface == null)
				throw new ArgumentNullException("bevahiorInterface");
			if (behaviorImplementation == null)
				throw new ArgumentNullException("behaviorImplementation");
			if (!bevahiorInterface.IsInstanceOfType(behaviorImplementation))
				throw new ArgumentException("behaviorImplementation must implement bevahiorInterface", "behaviorImplementation");

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
