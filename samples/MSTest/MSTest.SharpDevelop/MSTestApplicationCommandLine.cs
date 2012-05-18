// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Text;

namespace ICSharpCode.MSTest
{
	public class MSTestApplicationCommandLine
	{
		StringBuilder commandLine = new StringBuilder();
		
		public MSTestApplicationCommandLine()
		{
		}
		
		public void Append(string argument, string value)
		{
			AppendFormat("/{0}:{1} ", argument, value);
		}
		
		public void AppendQuoted(string argument, string value)
		{
			AppendFormat("/{0}:\"{1}\" ", argument, value);
		}
		
		void AppendFormat(string format, string argument, string value)
		{
			commandLine.AppendFormat(format, argument, value);
		}
		
		public override string ToString()
		{
			return commandLine.ToString();
		}
	}
}
