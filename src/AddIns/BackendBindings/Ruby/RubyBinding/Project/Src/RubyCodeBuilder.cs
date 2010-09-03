// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.Text;
using ICSharpCode.Scripting;

namespace ICSharpCode.RubyBinding
{
	public class RubyCodeBuilder : ScriptingCodeBuilder
	{
		public RubyCodeBuilder()
		{
		}
		
		public RubyCodeBuilder(int initialIndent)
			: base(initialIndent)
		{
		}
		
		public void AppendLineIfPreviousLineIsEndStatement()
		{
			string previousLine = GetPreviousLine().Trim();
			if (previousLine.Equals("end", StringComparison.InvariantCultureIgnoreCase)) {
				AppendLine();
			}
		}		
	}
}
