// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
