using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Reflection;
using System.Windows;
using System.IO;

namespace ICSharpCode.Xaml
{
	public class XamlConstants
	{
		public static XNamespace XamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml";
		public static XNamespace Presentation2006Namespace = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
		public static XNamespace Presentation2007Namespace = "http://schemas.microsoft.com/netfx/2007/xaml/presentation";

		public static Assembly MscorlibAssembly = typeof(object).Assembly;
		public static Assembly WindowsBaseAssembly = typeof(DependencyObject).Assembly;
		public static Assembly PresentationCoreAssembly = typeof(UIElement).Assembly;
		public static Assembly PresentationFrameworkAssembly = typeof(FrameworkElement).Assembly;

		public static XName XmlSpaceName = XNamespace.Xml.GetName("space");

		public static bool HasXamlExtension(string filePath)
		{
			return Path.GetExtension(filePath).Equals(".xaml", StringComparison.InvariantCultureIgnoreCase);
		}
	}
}
