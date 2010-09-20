// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
