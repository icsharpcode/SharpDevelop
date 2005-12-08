// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Text;
using System.Text.RegularExpressions;

using System.CodeDom;
using System.CodeDom.Compiler;

using ICSharpCode.Core;
using ICSharpCode.FormsDesigner.Services;

namespace ICSharpCode.FormsDesigner
{
	/// <summary>
	/// This class is able to generate a CodeDOM definition out of a XML file.
	/// </summary>
	public class CodeDOMGenerator
	{
		CodeDomProvider codeProvider;
		
		CodeDOMGeneratorUtility codeDOMGeneratorUtility = new CodeDOMGeneratorUtility();
		string indentation;
		
		public CodeDOMGenerator(CodeDomProvider codeProvider, string indentation)
		{
			this.codeProvider = codeProvider;
			this.indentation = indentation;
		}
		
		public void ConvertContentDefinition(CodeMemberMethod method, TextWriter writer)
		{
			LoggingService.Info("Generate code for: "+method.Name);
			
			CodeGeneratorOptions options = codeDOMGeneratorUtility.CreateCodeGeneratorOptions;
			options.IndentString = indentation;
			foreach (CodeStatement statement in method.Statements) {
				// indentation isn't generated when calling GenerateCodeFromStatement
				writer.Write(options.IndentString);
				try {
//						outputGenerator.PublicGenerateCodeFromStatement(statement, Console.Out, options);
					codeProvider.GenerateCodeFromStatement(statement, writer, options);
				} catch (Exception e) {
					codeProvider.GenerateCodeFromStatement(new CodeCommentStatement("TODO: Error while generating statement : " + e.Message),
					                                       writer,
					                                       options);
					LoggingService.Error(e);
				}
			}
			
		}
		
		public void ConvertContentDefinition(CodeMemberField field, TextWriter writer)
		{
			LoggingService.Info("Generate field declaration for: "+field.Name);
			
			CodeGeneratorOptions options = codeDOMGeneratorUtility.CreateCodeGeneratorOptions;
			options.IndentString = indentation;
			try {
				codeProvider.GenerateCodeFromMember(field, writer, options);
			} catch (Exception e) {
				codeProvider.GenerateCodeFromStatement(new CodeCommentStatement("TODO: Error while generating statement : " + e.Message),
				                                       writer,
				                                       options);
				LoggingService.Error(e);
			}
			
		}
		
	}
}
