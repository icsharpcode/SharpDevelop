// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.Windows;

namespace ICSharpCode.PackageManagement.Design
{
	public static class WpfDesigner
	{
		public static bool IsInDesignMode()
		{
			var isInDesignModeProperty = DesignerProperties.IsInDesignModeProperty;
			return GetPropertyDefaultValueAsBool(isInDesignModeProperty);
		}
		
		static bool GetPropertyDefaultValueAsBool(DependencyProperty property)
		{
			var dependencyPropertyDescriptor = GetDependencyPropertyDescriptor(property);
			return GetPropertyDefaultValueAsBool(dependencyPropertyDescriptor);
		}
		
		static DependencyPropertyDescriptor GetDependencyPropertyDescriptor(DependencyProperty property)
		{
			return DependencyPropertyDescriptor.FromProperty(property, typeof(FrameworkElement));
		}
		
		static bool GetPropertyDefaultValueAsBool(DependencyPropertyDescriptor dependencyPropertyDescriptor)
		{
			return (bool)dependencyPropertyDescriptor.Metadata.DefaultValue;
		}
	}
}
