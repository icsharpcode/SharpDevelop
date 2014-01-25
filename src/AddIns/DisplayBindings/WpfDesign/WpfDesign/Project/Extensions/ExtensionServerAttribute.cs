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
