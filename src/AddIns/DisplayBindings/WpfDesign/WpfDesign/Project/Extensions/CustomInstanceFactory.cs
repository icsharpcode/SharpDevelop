// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.WpfDesign.Extensions
{
	/// <summary>
	/// A special kind of extension that is used to create instances of objects when loading XAML inside
	/// the designer.
	/// </summary>
	/// <remarks>
	/// CustomInstanceFactory in Cider: http://blogs.msdn.com/jnak/archive/2006/04/10/572241.aspx
	/// </remarks>
	[ExtensionServer(typeof(NeverApplyExtensionsExtensionServer))]
	public class CustomInstanceFactory : Extension
	{
		/// <summary>
		/// Gets a default instance factory that uses Activator.CreateInstance to create instances.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		public static readonly CustomInstanceFactory DefaultInstanceFactory = new CustomInstanceFactory();
		
		/// <summary>
		/// Creates a new CustomInstanceFactory instance.
		/// </summary>
		protected CustomInstanceFactory()
		{
		}
		
		/// <summary>
		/// Creates an instance of the specified type, passing the specified arguments to its constructor.
		/// </summary>
		public virtual object CreateInstance(Type type, params object[] arguments)
		{
			return Activator.CreateInstance(type, arguments);
		}
	}
	
	// An extension server that never applies its extensions - used for special extensions
	// like CustomInstanceFactory
	sealed class NeverApplyExtensionsExtensionServer : ExtensionServer
	{
		public override bool ShouldApplyExtensions(DesignItem extendedItem)
		{
			return false;
		}
		
		public override Extension CreateExtension(Type extensionType, DesignItem extendedItem)
		{
			throw new NotImplementedException();
		}
		
		public override void RemoveExtension(Extension extension)
		{
			throw new NotImplementedException();
		}
		
		// since the event is never raised, we don't have to store the event handlers
		public override event EventHandler<DesignItemCollectionEventArgs> ShouldApplyExtensionsInvalidated {
			add {} remove {}
		}
	}
}
