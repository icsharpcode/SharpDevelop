// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Ivan Shumilin"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICSharpCode.WpfDesign.XamlDom
{
	/// <summary>
	/// Interface where errors during XAML loading are reported.
	/// </summary>
	public interface IXamlErrorSink
	{
		/// <summary>
		/// Reports a XAML load error.
		/// </summary>
		void ReportError(string message, int line, int column);
	}
}
