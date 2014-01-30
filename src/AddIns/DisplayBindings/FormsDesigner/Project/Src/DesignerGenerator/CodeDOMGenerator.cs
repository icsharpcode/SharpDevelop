// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.FormsDesigner
{
	/*
	/// <summary>
	/// This class is able to generate a CodeDOM definition out of a XML file.
	/// </summary>
	public class CodeDOMGenerator
	{
		CodeDomProvider codeProvider;
		
//		CodeDOMGeneratorUtility codeDOMGeneratorUtility = new CodeDOMGeneratorUtility();
		string indentation;
		
		protected CodeDomProvider CodeDomProvider {
			get { return codeProvider; }
		}
//		protected CodeDOMGeneratorUtility CodeDOMGeneratorUtility {
//			get { return codeDOMGeneratorUtility; }
//		}
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
		
	}*/
}
