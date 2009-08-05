using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Reflection;

namespace ICSharpCode.Core.Presentation
{
	public struct BindingInfoTemplate
	{
		public string OwnerInstanceName
		{
			get; set;
		}
		
		public string OwnerTypeName
		{
			get; set;
		}
		
		public string RoutedCommandName
		{
			get; set;
		}
		
		public static BindingInfoTemplate CreateFromIBindingInfo(IBindingInfo bindingInfo)
		{
			var tpl = new BindingInfoTemplate();
			tpl.OwnerInstanceName = bindingInfo.OwnerInstanceName;
			tpl.OwnerTypeName = bindingInfo.OwnerTypeName;
			tpl.RoutedCommandName = bindingInfo.RoutedCommandName;
			
			return tpl;
		}
	}
}
