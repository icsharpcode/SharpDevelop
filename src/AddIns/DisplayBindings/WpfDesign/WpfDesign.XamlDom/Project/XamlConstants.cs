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

namespace ICSharpCode.WpfDesign.XamlDom
{
	/// <summary>
	/// Contains constants used by the Xaml parser.
	/// </summary>
	public static class XamlConstants
	{
		#region Namespaces
		
		/// <summary>
		/// The namespace used to identify "xmlns".
		/// Value: "http://www.w3.org/2000/xmlns/"
		/// </summary>
		public const string XmlnsNamespace = "http://www.w3.org/2000/xmlns/";
		
		/// <summary>
		/// The namespace used for the XAML schema.
		/// Value: "http://schemas.microsoft.com/winfx/2006/xaml"
		/// </summary>
		public const string XamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml";
		
		/// <summary>
		/// The namespace used for the WPF schema.
		/// Value: "http://schemas.microsoft.com/winfx/2006/xaml/presentation"
		/// </summary>
		public const string PresentationNamespace = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
		
		/// <summary>
		/// The namespace used for the DesignTime schema.
		/// Value: "http://schemas.microsoft.com/expression/blend/2008"
		/// </summary>
		public const string DesignTimeNamespace = "http://schemas.microsoft.com/expression/blend/2008";

		/// <summary>
		/// The namespace used for the MarkupCompatibility schema.
		/// Value: "http://schemas.openxmlformats.org/markup-compatibility/2006"
		/// </summary>
		public const string MarkupCompatibilityNamespace = "http://schemas.openxmlformats.org/markup-compatibility/2006";
	
		#endregion
		
		#region Common property names
		
		/// <summary>
		/// The name of the Resources property.
		/// Value: "Resources"
		/// </summary>
		public const string ResourcesPropertyName = "Resources";
		
		/// <summary>
		/// The name of xmlns.
		/// Value: "xmlns"
		/// </summary>
		public const string Xmlns = "xmlns";
		
		#endregion
	}
}
