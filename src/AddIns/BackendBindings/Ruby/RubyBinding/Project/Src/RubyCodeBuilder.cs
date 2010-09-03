// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
