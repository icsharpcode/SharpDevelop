// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// Markup extension that retrieves localized resource strings.
	/// </summary>
	public class LocalizeExtension : MarkupExtension
	{
		public LocalizeExtension(string key)
		{
			this.key = key;
			this.UsesAccessors = true;
		}
		
		protected string key;
		
		public bool UsesAccessors { get; set; }
		
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			try {
				string result = ResourceService.GetString(key);
				if (UsesAccessors)
					result = MenuService.ConvertLabel(result);
				return result;
			} catch (ResourceNotFoundException) {
				return "{Localize:" + key + "}";
			}
		}
	}
}
