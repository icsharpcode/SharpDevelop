// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Microsoft.CSharp;

namespace WrapperGenerator
{
	public class FileGenerator
	{
		CodeGenerator codeGenerator;
		string header;
		string footer;
		
		public FileGenerator(CodeGenerator codeGenerator, string header, string footer)
		{
			this.codeGenerator = codeGenerator;
			this.header = header;
			this.footer = footer;
		}
		
		public string SaveFiles(string saveDirectory)
		{
			string allCode = "";
			
			foreach(CodeCompileUnit compileUnit in codeGenerator.MakeCompileUnits()) {
				string code = GenerateCode(compileUnit);
				code = code.Remove(0, code.IndexOf("namespace"));
				
				allCode += code;
				
				string className = (string)compileUnit.UserData["filename"];
				
				TextWriter textWriter = new StreamWriter(Path.Combine(saveDirectory, className + ".cs"));
				textWriter.Write(header);
				textWriter.Write(code);
				textWriter.Write(footer);
				textWriter.Close();
			}
			
			return allCode;
		}
		
		string GenerateCode(CodeCompileUnit compileUnit)
		{
			CSharpCodeProvider provider = new CSharpCodeProvider();
			
			TextWriter stringWriter = new StringWriter();
			
			CodeGeneratorOptions options = new CodeGeneratorOptions();
			options.BlankLinesBetweenMembers = true;
			options.BracingStyle = "C";
			options.ElseOnClosing = true;
			options.IndentString = "\t";
			options.VerbatimOrder = true;
			
			provider.GenerateCodeFromCompileUnit(compileUnit, stringWriter, options);
			
			return stringWriter.ToString();
		}
	}
}
