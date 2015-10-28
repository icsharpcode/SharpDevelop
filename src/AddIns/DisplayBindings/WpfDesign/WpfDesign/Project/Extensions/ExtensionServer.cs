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
		/// Returns if the Extension Server should be reapplied (for multiple selection extension server for example).
		/// </summary>
		public virtual bool ShouldBeReApplied() 
		{
			return false;
		}
		
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
