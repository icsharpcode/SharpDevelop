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
using System.ComponentModel;
using System.Windows;

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
			var instance = Activator.CreateInstance(type, arguments);
			var uiElement = instance as UIElement;
			if (uiElement != null)
				DesignerProperties.SetIsInDesignMode(uiElement, true);
			return instance;
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
