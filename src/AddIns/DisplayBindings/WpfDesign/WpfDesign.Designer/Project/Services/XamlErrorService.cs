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
using ICSharpCode.WpfDesign.XamlDom;
using System.Collections.ObjectModel;

namespace ICSharpCode.WpfDesign.Designer.Services
{
	public class XamlErrorService : IXamlErrorSink
	{
		public XamlErrorService()
		{
			Errors = new ObservableCollection<XamlError>();
		}

		public ObservableCollection<XamlError> Errors { get; private set; }

		public void ReportError(string message, int line, int column)
		{
			Errors.Add(new XamlError() { Message = message, Line = line, Column = column });
		}
	}

	public class XamlError
	{
		public string Message { get; set; }
		public int Line { get; set; }
		public int Column { get; set; }
	}
}
