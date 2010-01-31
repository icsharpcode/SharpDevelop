// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Text;

using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Hosting.Providers;

namespace ICSharpCode.PythonBinding
{
	public class StringTextContentProvider : TextContentProvider
	{
		string code;
		
		public StringTextContentProvider(string code)
		{
			this.code = code;
		}
		
		public override SourceCodeReader GetReader()
		{
			StringReader stringReader = new StringReader(code);
			return new SourceCodeReader(stringReader, Encoding.UTF8);
		}
	}
}
