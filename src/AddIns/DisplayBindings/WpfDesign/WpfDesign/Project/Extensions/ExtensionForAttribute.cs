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
using System.Collections.Generic;
namespace ICSharpCode.WpfDesign.Extensions
{
	/// <summary>
	/// Attribute to specify that the decorated class is a WPF extension for the specified item type.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=true, Inherited=false)]
	public sealed class ExtensionForAttribute : Attribute
	{
		Type _designedItemType;
		Type _overrideExtension;
		List<Type> _overrideExtensions = new List<Type>();
		
		/// <summary>
		/// Gets the type of the item that is designed using this extension.
		/// </summary>
		public Type DesignedItemType {
			get { return _designedItemType; }
		}
		
		/// <summary>
		/// Gets/Sets the types of another extension that this extension is overriding.
		/// </summary>
		public Type[] OverrideExtensions
		{
			get { return _overrideExtensions.ToArray(); }
			set
			{
				_overrideExtensions.AddRange(value);
			}
		}
		
		/// <summary>
		/// Gets/Sets the type of another extension that this extension is overriding.
		/// </summary>
		public Type OverrideExtension {
			get { return _overrideExtension; }
			set {
				_overrideExtension = value;
				if (value != null) {
					if (!typeof(Extension).IsAssignableFrom(value)) {
						throw new ArgumentException("OverrideExtension must specify the type of an Extension.");
					}
					if(!_overrideExtensions.Contains(value))
						_overrideExtensions.Add(value);
				}
			}
		}
		
		/// <summary>
		/// Create a new ExtensionForAttribute that specifies that the decorated class
		/// is a WPF extension for the specified item type.
		/// </summary>
		public ExtensionForAttribute(Type designedItemType)
		{
			if (designedItemType == null)
				throw new ArgumentNullException("designedItemType");
			if (!designedItemType.IsClass)
				throw new ArgumentException("designedItemType must be a class");
			_designedItemType = designedItemType;
		}
	}
}
