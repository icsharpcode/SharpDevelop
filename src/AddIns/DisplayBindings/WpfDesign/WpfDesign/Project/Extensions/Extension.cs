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
using System.Windows;

namespace ICSharpCode.WpfDesign.Extensions
{
	/// <summary>
	/// Base class for all Extensions.
	/// </summary>
	/// <remarks>
	/// The class design in the ICSharpCode.WpfDesign.Extensions namespace was made to match that of Cider
	/// as described in the blog posts:
	/// http://blogs.msdn.com/jnak/archive/2006/04/24/580393.aspx
	/// http://blogs.msdn.com/jnak/archive/2006/08/04/687166.aspx
	/// </remarks>
	public abstract class Extension
	{
		/// <summary>
		/// Gets the value of the <see cref="DisabledExtensionsProperty"/> attached property for an object.
		/// </summary>
		/// <param name="obj">The object from which the property value is read.</param>
		/// <returns>The object's <see cref="DisabledExtensionsProperty"/> property value.</returns>
		public static string GetDisabledExtensions(DependencyObject obj)
		{
			return (string)obj.GetValue(DisabledExtensionsProperty);
		}

		/// <summary>
		/// Sets the value of the <see cref="DisabledExtensionsProperty"/> attached property for an object. 
		/// </summary>
		/// <param name="obj">The object to which the attached property is written.</param>
		/// <param name="value">The value to set.</param>
		public static void SetDisabledExtensions(DependencyObject obj, string value)
		{
			obj.SetValue(DisabledExtensionsProperty, value);
		}

		/// <summary>
		/// Gets or sets a semicolon-separated list with extension names that is disabled for a component's view.
		/// </summary>
		public static readonly DependencyProperty DisabledExtensionsProperty =
			DependencyProperty.RegisterAttached("DisabledExtensions", typeof(string), typeof(Extension), new PropertyMetadata(null));

		
	}
}
