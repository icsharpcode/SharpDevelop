// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
