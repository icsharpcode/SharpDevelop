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
using System.Windows;
using System.Xml.Serialization;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.XamlBinding
{
	/// <summary>
	/// Contains all static/const info (xmlns, elements, etc.) used in XAML.
	/// </summary>
	public static class XamlConst
	{
		// [XAML 2009]
		public static readonly List<string> XamlBuiltInTypes = new List<string> {
			"Object", "Boolean", "Char", "String", "Decimal", "Single", "Double",
			"Int16", "Int32", "Int64", "TimeSpan", "Uri", "Byte", "Array", "List", "Dictionary",
			"Reference"
		};
		
		public static readonly ReadOnlyCollection<string> XamlNamespaceAttributes = new List<string> {
			"Class", "ClassModifier", "FieldModifier", "Name", "Subclass", "TypeArguments", "Uid", "Key", "Shared"
		}.AsReadOnly();
		
		/// <summary>
		/// values: http://schemas.microsoft.com/winfx/2006/xaml/presentation,
		/// http://schemas.microsoft.com/netfx/2007/xaml/presentation
		/// </summary>
		public static readonly string[] WpfXamlNamespaces = {
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
		/// Returns the list of allow XAML2009 completion items.
		/// </summary>
		public static IEnumerable<string> GetAllowedItems(XamlCompletionContext context)
		{
			string xamlPrefix = context.XamlNamespacePrefix;
			string xKey = string.IsNullOrEmpty(xamlPrefix) ? "" : xamlPrefix + ":";
			var compilation = SD.ParserService.GetCompilationForFile(context.Editor.FileName);
			var resolver = new XamlAstResolver(compilation, context.ParseInformation);
			// TODO : add support for x:Key as attribute element (XAML 2009 only)
			
			switch (context.Description) {
				case XamlContextDescription.AtTag:
					if (context.ParentElement == null || context.RootElement == null)
						yield break;
					if (string.Equals(context.ParentElement.Name, xKey + "Members", StringComparison.OrdinalIgnoreCase)) {
						yield return xKey + "Member";
						yield return xKey + "Property";
					} else if (context.ParentElement == context.RootElement && context.RootElement.Attributes.Any(attr => string.Equals(attr.Name, xKey + "Class", StringComparison.OrdinalIgnoreCase))) {
						yield return xKey + "Code";
						yield return xKey + "Members";
					} else {
						if (string.Equals(context.ParentElement.Name, xKey + "Code", StringComparison.OrdinalIgnoreCase))
							yield break;
						yield return xKey + "Array";
						yield return xKey + "Boolean";
						yield return xKey + "Byte";
						yield return xKey + "Char";
						yield return xKey + "Decimal";
						yield return xKey + "Dictionary";
						yield return xKey + "Double";
						yield return xKey + "Int16";
						yield return xKey + "Int32";
						yield return xKey + "Int64";
						yield return xKey + "List";
						yield return xKey + "Object";
						yield return xKey + "Reference";
						yield return xKey + "Single";
						yield return xKey + "String";
						yield return xKey + "TimeSpan";
						yield return xKey + "Uri";
						if (context.RootElement.Attributes.Any(attr => string.Equals(attr.Name, xKey + "Class", StringComparison.OrdinalIgnoreCase)))
							yield return xKey + "Members";
					}
					break;
				case XamlContextDescription.InTag:
					yield return xKey + "Uid";
					if (context.InRoot) {
						yield return xKey + "Class";
						yield return xKey + "ClassModifier";
						yield return xKey + "Subclass";
						yield return xKey + "Name";
					} else {
						var resourceDictionaryType = compilation.FindType(typeof(ResourceDictionary));
						if (context.ActiveElement != null && string.Equals(context.ActiveElement.Name, xKey + "Array", StringComparison.OrdinalIgnoreCase)) {
							yield return "Type";
						} else if (context.ActiveElement != null && string.Equals(context.ActiveElement.Name, xKey + "Member", StringComparison.OrdinalIgnoreCase)) {
							yield return "Name";
						} else if (context.ActiveElement != null && string.Equals(context.ActiveElement.Name, xKey + "Property", StringComparison.OrdinalIgnoreCase)) {
							yield return "Name";
							yield return "Type";
						} else if (context.RootElement.Attributes.Any(attr => string.Equals(attr.Name, xKey + "Class", StringComparison.OrdinalIgnoreCase))) {
							yield return xKey + "FieldModifier";
							yield return xKey + "Name";
						} else {
							yield return xKey + "Name";
						}
						
						if (context.ParentElement != null) {
							var rr = resolver.ResolveElement(context.ParentElement);
							if (rr != null) {
								if (rr.Type.Equals(resourceDictionaryType)) {
									yield return xKey + "Key";
									yield return xKey + "Shared";
								} else if (rr.Type.TypeParameterCount > 0) {
									yield return xKey + "TypeArguments";
								}
							}
						}
					}
					break;
			}
			yield break;
		}
	}
}
