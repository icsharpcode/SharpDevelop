// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 2644$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.Core;
using System.IO;
using System.Text;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
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
		
		internal static IClass FindMyFormsClass(IProjectContent pc, string myNamespace)
		{
			if (pc != null) {
				return pc.GetClass(myNamespace + ".MyForms", 0);
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
						IClass myFormsClass = FindMyFormsClass(ParserService.GetProjectContent(vbProject), ns);
						if (myFormsClass != null) {
							string indentation = line.Substring(0, line.Length - trimmedLine.Length);
							foreach (IProperty p in myFormsClass.Properties) {
								string typeName = "global::" + p.ReturnType.FullyQualifiedName;
								output.AppendLine(indentation + typeName + " " + p.Name + "_instance;");
								output.AppendLine(indentation + "bool " + p.Name + "_isCreating;");
								output.AppendLine(indentation + "public " + typeName + " " + p.Name + " {");
								output.AppendLine(indentation + "\t[DebuggerStepThrough] get { return GetForm(ref " + p.Name + "_instance, ref " + p.Name + "_isCreating); }");
								output.AppendLine(indentation + "\t[DebuggerStepThrough] set { SetForm(ref " + p.Name + "_instance, value); }");
								output.AppendLine(indentation + "}");
								output.AppendLine(indentation);
							}
						}
					} else if (trimmedLine.StartsWith("#if ")) {
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
