// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Gui;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Internal.Templates
{
	public class ScriptRunner
	{
		FileTemplate item;
		FileDescriptionTemplate file;
		
		readonly static Regex scriptRegex  = new Regex("<%.*?%>");
		
		public string CompileScript(FileTemplate item, FileDescriptionTemplate file)
		{
			if (file.Content == null)
				throw new ArgumentException("file must have textual content");
			Match m = scriptRegex.Match(file.Content);
			// A file must have at least two "<% %>" segments to be recognized as script.
			// I consider this a bug, but we'll keep it for backwards compatibility;
			// at least until the ScriptRunner gets replaced by something more sane.
			m = m.NextMatch();
			if (m.Success) {
				this.item = item;
				this.file = file;
				return CompileAndGetOutput(GenerateCode());
			}
			return file.Content;
		}
		
		static Dictionary<string, Assembly> cachedAssemblies = new Dictionary<string, Assembly>();
		
		static Assembly CompileAssembly(string fileContent)
		{
			if (cachedAssemblies.ContainsKey(fileContent))
				return cachedAssemblies[fileContent];
			
			using (TempFileCollection tf = new TempFileCollection()) {
				string path = Path.Combine(tf.BasePath, tf.TempDir);
				Directory.CreateDirectory(path);
				string generatedScript = Path.Combine(path, "InternalGeneratedScript.cs");
				string generatedDLL    = Path.Combine(path, "A.DLL");
				tf.AddFile(generatedScript, false);
				tf.AddFile(generatedDLL, false);
				
				StreamWriter sw = new StreamWriter(generatedScript);
				sw.Write(fileContent);
				sw.Close();
				
				string output = String.Empty;
				string error  = String.Empty;
				
				Executor.ExecWaitWithCapture(GetCompilerName() + " /target:library \"/out:" + generatedDLL + "\" \"" + generatedScript +"\"", tf, ref output, ref error);
				
				if (!File.Exists(generatedDLL)) {
					
					StreamReader sr = File.OpenText(output);
					string errorMessage = sr.ReadToEnd();
					sr.Close();
					MessageService.ShowMessage(errorMessage);
					return null;
				}
				
				Assembly asm = Assembly.Load(File.ReadAllBytes(generatedDLL));
				cachedAssemblies[fileContent] = asm;
				return asm;
			}
		}
		
		string CompileAndGetOutput(string fileContent)
		{
			Assembly asm = CompileAssembly(fileContent);
			if (asm == null) {
				return ">>>>ERROR IN CODE GENERATION GENERATED SCRIPT WAS:\n" + fileContent + "\n>>>>END";
			}
			
			object templateInstance = asm.CreateInstance("Template");
			
			
			foreach (TemplateProperty property in item.Properties) {
				FieldInfo fieldInfo = templateInstance.GetType().GetField(property.Name);
				fieldInfo.SetValue(templateInstance, Convert.ChangeType(StringParserPropertyContainer.LocalizedProperty["Properties." + property.Name], property.Type.StartsWith("Types:") ? typeof(string): Type.GetType(property.Type)));
			}
			MethodInfo methodInfo = templateInstance.GetType().GetMethod("GenerateOutput");
			string ret = methodInfo.Invoke(templateInstance, null).ToString();
			return ret;
		}
		
		static string GetCompilerName()
		{
			string runtimeDirectory = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory();
			return '"' + Path.Combine(runtimeDirectory, "csc.exe") + '"';
		}
		
		string GenerateCode()
		{
			StringBuilder outPut = new StringBuilder();
			int lastIndex = 0;
			outPut.Append("public class Template {\n");
			foreach (TemplateProperty property in item.Properties) {
				outPut.Append("public ");
				// internal generated enum types are nothing other than strings
				if (property.Type.StartsWith("Types:")) {
					outPut.Append("string");
				} else {
					outPut.Append(property.Type);
				}
				outPut.Append(' ');
				outPut.Append(property.Name);
				outPut.Append(";\n");
			}
			outPut.Append("public string GenerateOutput() {\n");
			outPut.Append("System.Text.StringBuilder outPut = new System.Text.StringBuilder();\n");
			
			for (Match m = scriptRegex.Match(file.Content); m.Success; m = m.NextMatch()) {
				Group g = m.Groups[0];
				outPut.Append("outPut.Append(@\"");
				outPut.Append(file.Content.Substring(lastIndex, g.Index - lastIndex).Replace("\"", "\"\""));
				outPut.Append("\");\n");
				outPut.Append(g.Value.Substring(2, g.Length - 4));
				lastIndex = g.Index + g.Length;
			}
			outPut.Append("outPut.Append(@\"");
			string formattedContent = file.Content.Substring(lastIndex, file.Content.Length - lastIndex).Replace("\"", "\"\"");
			outPut.Append(formattedContent);
			outPut.Append("\");\n");
			outPut.Append("return outPut.ToString();\n");
			outPut.Append("}}\n");
			return outPut.ToString();
		}
	}
}
