// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;

namespace ICSharpCode.PythonBinding
{
	public class CSharpToPythonConverter : NRefactoryToPythonConverter
	{	
		public CSharpToPythonConverter()
		{
		}
		
		/// <summary>
		/// Converts the C# source code to Python.
		/// </summary>
		public string Convert(string source)
		{
			return Convert(source, SupportedLanguage.CSharp);
		}
	}	
}
