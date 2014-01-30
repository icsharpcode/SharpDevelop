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

namespace ICSharpCode.WpfDesign.PropertyGrid
{
	/// <summary>
	/// Attribute to specify that the decorated class is a editor for the specified property.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=true, Inherited=false)]
	public sealed class PropertyEditorAttribute : Attribute
	{
		readonly Type propertyDeclaringType;
		readonly string propertyName;
		
		/// <summary>
		/// Creates a new PropertyEditorAttribute that specifies that the decorated class is a editor
		/// for the "<paramref name="propertyDeclaringType"/>.<paramref name="propertyName"/>".
		/// </summary>
		public PropertyEditorAttribute(Type propertyDeclaringType, string propertyName)
		{
			if (propertyDeclaringType == null)
				throw new ArgumentNullException("propertyDeclaringType");
			if (propertyName == null)
				throw new ArgumentNullException("propertyName");
			this.propertyDeclaringType = propertyDeclaringType;
			this.propertyName = propertyName;
		}
		
		/// <summary>
		/// Gets the type that declares the property that the decorated editor supports.
		/// </summary>
		public Type PropertyDeclaringType {
			get { return propertyDeclaringType; }
		}
		
		/// <summary>
		/// Gets the name of the property that the decorated editor supports.
		/// </summary>
		public string PropertyName {
			get { return propertyName; }
		}
	}
}
