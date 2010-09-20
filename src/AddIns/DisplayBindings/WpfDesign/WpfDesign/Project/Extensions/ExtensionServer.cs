// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;

namespace ICSharpCode.WpfDesign.Extensions
{
	/// <summary>
	/// An ExtensionServer manages a creating and removing extensions of the specific extension type.
	/// For a given DesignContext, a ExtensionServer is created only once.
	/// The ExtensionServer can handle events raised by services without having to unregister its events
	/// handlers because the ExtensionServer runs for the lifetime of the DesignContext.
	/// </summary>
	public abstract class ExtensionServer
	{
		DesignContext _context;
		
		/// <summary>
		/// Gets the context this extension server was created for.
		/// </summary>
		public DesignContext Context {
			[DebuggerStepThrough]
			get {
				if (_context == null)
					throw new InvalidOperationException("Cannot access ExtensionServer.Context: " +
					                                    "The property is not initialized yet. Please move initialization logic " +
					                                    "that depends on Context into the OnInitialized method.");
				return _context;
			}
		}
		
		/// <summary>
		/// Gets the service container of the current context. "x.Services" is equivalent to "x.Context.Services".
		/// </summary>
		public ServiceContainer Services {
			[DebuggerStepThrough]
			get { return this.Context.Services; }
		}
		
		internal void InitializeExtensionServer(DesignContext context)
		{
			Debug.Assert(this._context == null);
			Debug.Assert(context != null);
			this._context = context;
			OnInitialized();
		}
		
		/// <summary>
		/// Is called after the extension server is initialized and the <see cref="Context"/> property has been set.
		/// </summary>
		protected virtual void OnInitialized()
		{
		}
		
		/// <summary>
		/// Gets if the extension manager should apply the extensions from this server to the specified item.
		/// Is called by the ExtensionManager.
		/// </summary>
		public abstract bool ShouldApplyExtensions(DesignItem extendedItem);
		
		/// <summary>
		/// Create an extension of the specified type.
		/// Is called by the ExtensionManager.
		/// </summary>
		public abstract Extension CreateExtension(Type extensionType, DesignItem extendedItem);
		
		/// <summary>
		/// This method is called before an extension is removed from its DesignItem because it should not be applied anymore.
		/// Is called by the ExtensionManager.
		/// </summary>
		public abstract void RemoveExtension(Extension extension);
		
		/// <summary>
		/// This event is raised when ShouldApplyExtensions is invalidated for a set of items.
		/// </summary>
		public abstract event EventHandler<DesignItemCollectionEventArgs> ShouldApplyExtensionsInvalidated;
	}
}
