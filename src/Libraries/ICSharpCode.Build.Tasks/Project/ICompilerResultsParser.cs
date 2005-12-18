// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.CodeDom.Compiler;

namespace ICSharpCode.Build.Tasks
{
	public interface ICompilerResultsParser
	{
		CompilerResults Parse(TempFileCollection tempFiles, string fileName);
	}
}
