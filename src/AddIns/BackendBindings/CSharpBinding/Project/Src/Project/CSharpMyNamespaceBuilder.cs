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
using System.Collections.Generic;
using ICSharpCode.Core;
using System.IO;
using System.Text;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;

namespace CSharpBinding
{
	/// <summary>
	/// Creates code for the C# version of the "My" namespace.
	/// Used by the VB->C# converter
	/// </summary>
	static class CSharpMyNamespaceBuilder
	{
		static Stream OpenResource()
		{
			return typeof(CSharpMyNamespaceBuilder).Assembly.GetManifestResourceStream("Resources.MyNamespaceSupportForCSharp.cs");
		}
		
		internal static ITypeDefinition FindMyFormsClass(IAssembly asm, string myNamespace)
		{
			if (asm != null) {
				return asm.GetTypeDefinition(myNamespace, "MyForms", 0);
			}
			return null;
		}
		
		internal static string BuildMyNamespaceCode(CompilableProject vbProject)
		{
			string ns;
			if (string.IsNullOrEmpty(vbProject.RootNamespace))
				ns = "My";
			else
				ns = vbProject.RootNamespace + ".My";
			
			string projectType;
			if (vbProject.OutputType == OutputType.WinExe)
				projectType = "WindowsApplication";
			else if (vbProject.OutputType == OutputType.Exe)
				projectType = "ConsoleApplication";
			else
				projectType = "Library";
			
			StringBuilder output = new StringBuilder();
			bool outputActive = true;
			
			using (StreamReader r = new StreamReader(OpenResource())) {
				string line;
				while ((line = r.ReadLine()) != null) {
					string trimmedLine = line.Trim();
					if (trimmedLine == "#endif") {
						outputActive = true;
						continue;
					}
					if (!outputActive) {
						continue;
					}
					if (trimmedLine == "/*LIST OF FORMS*/") {
						var compilation = SD.ParserService.GetCompilation(vbProject);
						ITypeDefinition myFormsClass = FindMyFormsClass(compilation.MainAssembly, ns);
						if (myFormsClass != null) {
							string indentation = line.Substring(0, line.Length - trimmedLine.Length);
							foreach (IProperty p in myFormsClass.Properties) {
								string typeName = "global::" + p.ReturnType.FullName;
								output.AppendLine(indentation + typeName + " " + p.Name + "_instance;");
								output.AppendLine(indentation + "bool " + p.Name + "_isCreating;");
								output.AppendLine(indentation + "public " + typeName + " " + p.Name + " {");
								output.AppendLine(indentation + "\t[DebuggerStepThrough] get { return GetForm(ref " + p.Name + "_instance, ref " + p.Name + "_isCreating); }");
								output.AppendLine(indentation + "\t[DebuggerStepThrough] set { SetForm(ref " + p.Name + "_instance, value); }");
								output.AppendLine(indentation + "}");
								output.AppendLine(indentation);
							}
						}
					} else if (trimmedLine.StartsWith("#if ", StringComparison.Ordinal)) {
						outputActive = trimmedLine.Substring(4) == projectType;
					} else {
						output.AppendLine(StringParser.Parse(line.Replace("MyNamespace", ns)));
					}
				}
			}
			return output.ToString();
		}
	}
}
