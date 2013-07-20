// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
