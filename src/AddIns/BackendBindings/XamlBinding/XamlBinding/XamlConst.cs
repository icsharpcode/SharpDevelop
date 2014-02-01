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
using System.Collections.ObjectModel;
using System.Linq;

namespace ICSharpCode.XamlBinding
{
	/// <summary>
	/// Contains all static/const info (xmlns, elements, etc.) used in XAML.
	/// </summary>
	public static class XamlConst
	{
		public const bool EnableXaml2009 = true;
		
		// [XAML 2009]
		public static readonly List<string> XamlBuiltInTypes = new List<string> {
			"Object", "Boolean", "Char", "String", "Decimal", "Single", "Double",
			"Int16", "Int32", "Int64", "TimeSpan", "Uri", "Byte", "Array", "List", "Dictionary",
			// This is no built in type, but a markup extension
			"Reference"
		};
		
		public static readonly ReadOnlyCollection<string> XamlNamespaceAttributes = new List<string> {
			"Class", "ClassModifier", "FieldModifier", "Name", "Subclass", "TypeArguments", "Uid", "Key"
		}.AsReadOnly();
		
		public static readonly ReadOnlyCollection<string> RootOnlyElements = new List<string> {
			"Class", "ClassModifier", "Subclass"
		}.AsReadOnly();
		
		public static readonly ReadOnlyCollection<string> ChildOnlyElements = new List<string> {
			"FieldModifier"
		}.AsReadOnly();
		
		/// <summary>
		/// values: http://schemas.microsoft.com/winfx/2006/xaml/presentation,
		/// http://schemas.microsoft.com/netfx/2007/xaml/presentation
		/// </summary>
		public static readonly string[] WpfXamlNamespaces = new[] {
			"http://schemas.microsoft.com/winfx/2006/xaml/presentation",
			"http://schemas.microsoft.com/netfx/2007/xaml/presentation"
		};
		
		/// <summary>
		/// value: http://schemas.microsoft.com/winfx/2006/xaml
		/// </summary>
		public const string XamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml";
		
		/// <summary>
		/// value: http://schemas.openxmlformats.org/markup-compatibility/2006
		/// </summary>
		public const string MarkupCompatibilityNamespace = "http://schemas.openxmlformats.org/markup-compatibility/2006";
		
		/// <summary>
		/// Returns true if the given localName is a XAML builtin type or a predefined attribute.
		/// </summary>
		public static bool IsBuiltin(string localName)
		{
			if (localName == null)
				throw new ArgumentNullException("localName");
			foreach (string name in XamlBuiltInTypes) {
				if (localName.Equals(name, StringComparison.Ordinal))
					return true;
			}
			foreach (string name in XamlNamespaceAttributes) {
				if (localName.Equals(name, StringComparison.Ordinal))
					return true;
			}
			return false;
		}
		
		/// <summary>
		/// Returns true if the given attribute is allowed in the current element.
		/// </summary>
		public static bool IsAttributeAllowed(bool inRoot, string localName)
		{
			return inRoot ? !ChildOnlyElements.Contains(localName) : !RootOnlyElements.Contains(localName);
		}
	}
}
