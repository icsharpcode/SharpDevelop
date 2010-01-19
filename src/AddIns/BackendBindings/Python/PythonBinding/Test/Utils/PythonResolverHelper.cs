// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom;

namespace PythonBinding.Tests.Utils
{
	public static class PythonResolverHelper
	{
		public static ParseInformation CreateParseInfoWithUsings(ICollection<string> usings)
		{
			MockProjectContent projectContent = new MockProjectContent();
			DefaultCompilationUnit compilationUnit = new DefaultCompilationUnit(projectContent);
			
			if (usings.Count > 0) {
				DefaultUsing newUsing = new DefaultUsing(projectContent);
				foreach (string name in usings) {
					newUsing.Usings.Add(name);
				}
				compilationUnit.UsingScope.Usings.Add(newUsing);
			}
			
			ParseInformation parseInfo = new ParseInformation(compilationUnit);
			return parseInfo;
		}
	}
}
