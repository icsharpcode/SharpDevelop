using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICSharpCode.WpfDesign.XamlDom
{
	public interface IXamlErrorSink
	{
		void ReportError(string message, int line, int column);
	}
}
