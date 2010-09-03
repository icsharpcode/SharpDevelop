// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.WpfDesign.Extensions
{
	/// <summary>
	/// Attribute to specify that the decorated class is an extension using the specified extension server.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
	public sealed class ExtensionServerAttribute : Attribute
	{
		Type _extensionServerType;
		
		/// <summary>
		/// Gets the type of the item that is designed using this extension.
		/// </summary>
		public Type ExtensionServerType {
			get { return _extensionServerType; }
		}
		
		/// <summary>
		/// Create a new ExtensionServerAttribute that specifies that the decorated extension
		/// uses the specified extension server.
		/// </summary>
		public ExtensionServerAttribute(Type extensionServerType)
		{
			if (extensionServerType == null)
				throw new ArgumentNullException("extensionServerType");
			if (!typeof(ExtensionServer).IsAssignableFrom(extensionServerType))
				throw new ArgumentException("extensionServerType must derive from ExtensionServer");
			if (extensionServerType.GetConstructor(new Type[0]) == null)
				throw new ArgumentException("extensionServerType must have a parameter-less constructor");
			_extensionServerType = extensionServerType;
		}
	}
}
