using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICSharpCode.Xaml
{
	public class XamlDocumentError
	{
		public string Message { get; set; }
		public int LineNumber { get; set; }
		public int LinePosition { get; set; }
		public XamlDocument Document { get; set; }
	}
}
