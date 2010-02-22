// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Globalization;

namespace Debugger.AddIn.Visualizers.Graph.Layout
{
	/// <summary>
	/// When used as IFormatProvider in string.Format, formats doubles to be suitable for Neato.
	/// </summary>
	public class NeatoDoubleFormatter : IFormatProvider, ICustomFormatter
	{
		private CultureInfo doubleCulture = CultureInfo.GetCultureInfo("en-US");
		/// <summary>
		/// CultureInfo used for formatting and parsing doubles (en-US).
		/// </summary>
		public CultureInfo DoubleCulture
		{
			get { return this.doubleCulture; }
		}

		public object GetFormat(Type formatType)
		{
			if (formatType == typeof(ICustomFormatter))
				return this;
			else
				return null;
		}

		public string Format(string format, object arg, IFormatProvider formatProvider)
		{
			return string.Format(doubleCulture, "{0:0.000}", arg);
		}
	}
}
