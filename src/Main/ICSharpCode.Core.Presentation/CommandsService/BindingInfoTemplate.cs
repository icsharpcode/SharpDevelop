using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Reflection;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// Contains data enough to uniquely identify <see cref="CommandBindingInfo" /> 
	/// or <see cref="InputBindingInfo" /> in <see cref="ICSharpCode.Core.Presentation.CommandManager" />
	/// 
	/// When property value set to null it is counted as wildcard when searching for matching binding infos
	/// </summary>
	public struct BindingInfoTemplate
	{
		private string _ownerInstanceName;
		private string _ownerTypeName;
		private string _routedCommandName;
		
		/// <summary>
		/// Gets name of named owner instance as registered using <see cref="ICSharpCode.Core.Presentation.CommandManager.RegisterNamedUIElement()" />
		/// </summary>
		public string OwnerInstanceName
		{
			get {
				return _ownerInstanceName;
			}
		}
		
		/// <summary>
		/// Gets name of named owner type as registered using <see cref="ICSharpCode.Core.Presentation.CommandManager.RegisterNamedUIType()" />
		/// </summary>
		public string OwnerTypeName
		{
			get {
				return _ownerTypeName;
			}
		}
		
		/// <summary>
		/// Gets name of registered named <see cref="RoutedUICommand" />
		/// </summary>
		public string RoutedCommandName
		{
			get {
				return _routedCommandName;
			}
		}
		
		/// <summary>
		/// Creates <see cref="BndingInfoTemplate" /> instance from provided property values
		/// </summary>
		/// <param name="ownerInstanceName">Gets name of named owner instance as registered using <see cref="ICSharpCode.Core.Presentation.CommandManager.RegisterNamedUIElement()" /></param>
		/// <param name="ownerTypeName">Gets name of named owner type as registered using <see cref="ICSharpCode.Core.Presentation.CommandManager.RegisterNamedUIType()" /></param>
		/// <param name="routedCommandName">Gets name of registered named <see cref="RoutedUICommand" /></param>
		/// <returns>Created <see cref="BndingInfoTemplate" /> instance</returns>
		public static BindingInfoTemplate Create(string ownerInstanceName, string ownerTypeName, string routedCommandName)
		{
			var tpl = new BindingInfoTemplate();
			tpl._ownerInstanceName = ownerInstanceName;
			tpl._ownerTypeName = ownerTypeName;
			tpl._routedCommandName = routedCommandName;
			
			return tpl;
		}
		
		/// <summary>
		/// Create <see cref="BindingInfoTemplate" /> instance which can identify provided <see cref="BindingInfoBase" />
		/// </summary>
		/// <param name="bindingInfo">Provided binding info</param>
		/// <returns>Created <see cref="BndingInfoTemplate" /> instance</returns>
		public static BindingInfoTemplate CreateFromIBindingInfo(IBindingInfo bindingInfo)
		{
			var tpl = new BindingInfoTemplate();
			tpl._ownerInstanceName = bindingInfo.OwnerInstanceName;
			tpl._ownerTypeName = bindingInfo.OwnerTypeName;
			tpl._routedCommandName = bindingInfo.RoutedCommandName;
			
			return tpl;
		}
	}
}
