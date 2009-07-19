// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at" />
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
	/// Markup extension that works like StringParser.Parse
	/// </summary>
	[MarkupExtensionReturnType(typeof(string))]
	public class StringParseExtension : MarkupExtension
	{
		protected string text;
		
		public bool UsesAccessors { get; set; }
		
		public StringParseExtension(string text)
		{
			this.text = text;
			this.UsesAccessors = true;
		}
		
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			string result = StringParser.Parse(text);
			if (UsesAccessors)
				result = MenuService.ConvertLabel(result);
			return result;
		}
	}
}
