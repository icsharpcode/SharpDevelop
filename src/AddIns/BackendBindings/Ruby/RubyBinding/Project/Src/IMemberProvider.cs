// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

namespace ICSharpCode.RubyBinding
{
	/// <summary>
	/// Returns member names or global names for the ruby console command line.
	/// </summary>
	public interface IMemberProvider
	{
		IList<string> GetMemberNames(string name);
		
		IList<string> GetGlobals(string name);
	}
}
