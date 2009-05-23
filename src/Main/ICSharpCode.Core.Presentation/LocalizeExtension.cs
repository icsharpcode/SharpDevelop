// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
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
	
	/// <summary>
	/// Markup extension that works like StringParser.Parse
	/// </summary>
	public class StringParseExtension : MarkupExtension
	{
		protected string text;
		
		public StringParseExtension(string text)
		{
			this.text = text;
		}
		
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return StringParser.Parse(text);
		}
	}
}
