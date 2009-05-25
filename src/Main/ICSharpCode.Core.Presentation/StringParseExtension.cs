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
