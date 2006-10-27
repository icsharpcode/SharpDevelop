// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;

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
		
		protected CodeDomProvider CodeDomProvider {
			get { return codeProvider; }
		}
		protected CodeDOMGeneratorUtility CodeDOMGeneratorUtility {
			get { return codeDOMGeneratorUtility; }
		}
		protected string Indentation {
			get { return indentation; }
		}
		
		public CodeDOMGenerator(CodeDomProvider codeProvider, string indentation)
		{
			this.codeProvider = codeProvider;
			this.indentation = indentation;
		}
		
		public virtual void ConvertContentDefinition(CodeMemberMethod method, TextWriter writer)
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
