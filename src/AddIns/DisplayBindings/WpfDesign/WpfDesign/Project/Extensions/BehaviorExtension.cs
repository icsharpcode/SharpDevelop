// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.WpfDesign.Extensions
{
	/// <summary>
	/// Base class for extensions that provide a behavior interface for the designed item.
	/// These extensions are always loaded. They must have an parameter-less constructor.
	/// </summary>
	[ExtensionServer(typeof(BehaviorExtension.BehaviorExtensionServer))]
	public abstract class BehaviorExtension : Extension
	{
		DesignItem _extendedItem;
		
		/// <summary>
		/// Gets the item that is being extended by the BehaviorExtension.
		/// </summary>
		public DesignItem ExtendedItem {
			get {
				if (_extendedItem == null)
					throw new InvalidOperationException("Cannot access BehaviorExtension.ExtendedItem: " +
					                                    "The property is not initialized yet. Please move initialization logic " +
					                                    "that depends on ExtendedItem into the OnInitialized method.");
				return _extendedItem;
			}
		}
		
		/// <summary>
		/// Gets the design context of the extended item. "Context" is equivalent to "ExtendedItem.Context".
		/// </summary>
		public DesignContext Context {
			get {
				return this.ExtendedItem.Context;
			}
		}
		
		/// <summary>
		/// Gets the service container of the extended item. "Services" is equivalent to "ExtendedItem.Services".
		/// </summary>
		public ServiceContainer Services {
			get {
				return this.ExtendedItem.Services;
			}
		}
		
		/// <summary>
		/// Is called after the ExtendedItem was set.
		/// Override this method to register your behavior with the item.
		/// </summary>
		protected virtual void OnInitialized()
		{
		}
		
		sealed class BehaviorExtensionServer : ExtensionServer
		{
			public override bool ShouldApplyExtensions(DesignItem extendedItem)
			{
				return true;
			}
			
			public override Extension CreateExtension(Type extensionType, DesignItem extendedItem)
			{
				BehaviorExtension ext = (BehaviorExtension)Activator.CreateInstance(extensionType);
				ext._extendedItem = extendedItem;
				ext.OnInitialized();
				return ext;
			}
			
			public override void RemoveExtension(Extension extension)
			{
			}
		}
	}
}
