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
		}
		
		protected string key;
		
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return ResourceService.GetString(key);
		}
	}
}
