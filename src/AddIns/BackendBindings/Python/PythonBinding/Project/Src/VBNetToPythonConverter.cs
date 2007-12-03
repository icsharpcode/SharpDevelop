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
	/// <summary>
	/// Converts VB.NET to Python.
	/// </summary>
	public class VBNetToPythonConverter : NRefactoryToPythonConverter
	{
		public VBNetToPythonConverter()
		{
		}
		
		/// <summary>
		/// Converts the VB.NET source code to a code DOM that 
		/// the PythonProvider can use to generate Python. Using the
		/// NRefactory code DOM (CodeDomVisitor class) does not
		/// create a code DOM that is easy to convert to Python. 
		/// </summary>
		public CodeCompileUnit ConvertToCodeCompileUnit(string source)
		{
			return ConvertToCodeCompileUnit(source, SupportedLanguage.VBNet);
		}
		
		/// <summary>
		/// Converts the VB.NET source code to Python.
		/// </summary>
		public string Convert(string source)
		{
			return Convert(source, SupportedLanguage.VBNet);
		}
	}
}
