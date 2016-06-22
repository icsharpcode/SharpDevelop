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
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.Win32;

namespace ICSharpCode.Build.Tasks
{
	public sealed class FxCop : ToolTask
	{
		string realLogFile;
		
		public string LogFile { get; set; }
		public string InputAssembly { get; set; }
		public string[] Rules { get; set; }
		public string[] RuleAssemblies { get; set; }
		public string[] ReferencePaths { get; set;}
		public ITaskItem[] Dictionary { get; set;}
		
		protected override string ToolName {
			get {
				return "FxCopCmd.exe";
			}
		}
		
		public override bool Execute()
		{
			if (string.IsNullOrEmpty(ToolPath)) {
				string path = FindFxCopPath();
				Log.LogMessage(MessageImportance.High, Resources.RunningCodeAnalysis);
				if (path != null) {
					ToolPath = path;
				} else {
					Log.LogError(Resources.CannotFindFxCop);
					return false;
				}
			}
			realLogFile = LogFile ?? Path.GetTempFileName();
			try {
				bool result = base.Execute();
				if (File.Exists(realLogFile)) {
					try {
						#if DEBUG
						Console.WriteLine(File.ReadAllText(realLogFile));
						#endif
						XmlDocument doc = new XmlDocument();
						doc.Load(realLogFile);
						foreach (XmlNode node in doc.DocumentElement.SelectNodes(".//Exception")) {
							XmlElement el = node as XmlElement;
							if (el == null) continue;
							
							result = false;
							string checkId = el.GetAttribute("CheckId");
							string keyword = el.GetAttribute("Keyword");
							string message = el["ExceptionMessage"].InnerText;
							if (checkId.Length > 0) {
								Log.LogError(null, null, keyword, "", 0, 0, 0, 0,
								             "{0} : {2} ({1}): {3}", keyword, el.GetAttribute("Category"), checkId, el.GetAttribute("Target"), message);
							} else {
								Log.LogError(keyword + " : " + message);
							}
						}
						foreach (XmlNode node in doc.DocumentElement.SelectNodes(".//Message")) {
							XmlElement el = node as XmlElement;
							if (el == null) continue;
							result &= LogMessage(el);
						}
					} catch (XmlException e) {
						Log.LogError(Resources.CannotReadFxCopLogFile + " " + e.Message);
					}
				}
				return result;
			} finally {
				if (LogFile == null) {
					File.Delete(realLogFile);
				}
			}
		}
		
		/// <summary>
		/// Logs a message.
		/// </summary>
		/// <returns>True for warning, false for error.</returns>
		bool LogMessage(XmlElement message)
		{
			bool isWarning = true;
			string checkId = message.GetAttribute("CheckId");
			if (message.HasAttribute("TypeName")) {
				checkId = checkId + ":" + message.GetAttribute("TypeName");
			}
			string category = message.GetAttribute("Category");
			foreach (XmlNode node in message.SelectNodes(".//Issue")) {
				XmlElement issueEl = node as XmlElement;
				if (issueEl == null) continue;
				
				if ("true".Equals(message.GetAttribute("BreaksBuild"), StringComparison.OrdinalIgnoreCase)) {
					isWarning = false;
				}
				string issueText = issueEl.InnerText;
				string issuePath = issueEl.GetAttribute("Path");
				string issueFile = issueEl.GetAttribute("File");
				string issueLine = issueEl.GetAttribute("Line");
				int issueLineNumber = 0;
				string issueFullFile = null;
				
				// Try to find additional information about this type
				string memberName = null;
				string typeName = null;
				XmlNode parent = message.ParentNode;
				while (parent != null) {
					if (parent.Name == "Type") {
						if (typeName == null) {
							typeName = ((XmlElement)parent).GetAttribute("Name");
						} else {
							typeName = ((XmlElement)parent).GetAttribute("Name") + "+" + typeName;
						}
					} else if (parent.Name == "Namespace" && typeName != null) {
						typeName = ((XmlElement)parent).GetAttribute("Name") + "." + typeName;
					} else if (parent.Name == "Member" && memberName == null) {
						memberName = ((XmlElement)parent).GetAttribute("Name");
						// first, strip the # inserted by FxCop
						if (memberName.StartsWith("#"))
							memberName = memberName.Substring(1);
						// second, translate .ctor to #ctor
						memberName = memberName.Replace('.', '#');
					}
					parent = parent.ParentNode;
				}
				if (issuePath.Length > 0 && issueLine.Length > 0 && issueFile.Length > 0) {
					issueFullFile = Path.Combine(issuePath, issueFile);
					issueLineNumber = int.Parse(issueLine, CultureInfo.InvariantCulture);
				} else if (typeName != null) {
					issueFullFile = "positionof#" + typeName + "#" + memberName;
				}
				
				string data = typeName + "|" + memberName;
				
				if (message.HasAttribute("Id")) {
					data = data + "|" + message.GetAttribute("Id");
				}
				
				if (isWarning) {
					Log.LogWarning(data, checkId, category, issueFullFile, issueLineNumber, 0, 0, 0, issueText);
				} else {
					Log.LogError(data, checkId, category, issueFullFile, issueLineNumber, 0, 0, 0, issueText);
				}
			}
			return isWarning;
		}
		
		string FindFxCopPath()
		{
			// Code duplication: FxCopWrapper.cs in CodeAnalysis addin
			string fxCopPath = null;
			using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\VisualStudio\9.0\Setup\EDev")) {
				if (key != null) {
					fxCopPath = key.GetValue("FxCopDir") as string;
				}
			}
			if (IsFxCopPath(fxCopPath)) {
				return fxCopPath;
			}
			using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\VisualStudio\8.0\Setup\EDev")) {
				if (key != null) {
					fxCopPath = key.GetValue("FxCopDir") as string;
				}
			}
			if (IsFxCopPath(fxCopPath)) {
				return fxCopPath;
			}

			fxCopPath = FromRegistry(Registry.ClassesRoot.OpenSubKey(@"FxCop.Project.9.0\Shell\Open\Command"));
			if (IsFxCopPath(fxCopPath)) {
				return fxCopPath;
			}
			fxCopPath = FromRegistry(Registry.CurrentUser.OpenSubKey(@"Software\Classes\FxCopProject\Shell\Open\Command"));
			if (IsFxCopPath(fxCopPath)) {
				return fxCopPath;
			}
			fxCopPath = FromRegistry(Registry.ClassesRoot.OpenSubKey(@"FxCopProject\Shell\Open\Command"));
			if (IsFxCopPath(fxCopPath)) {
				return fxCopPath;
			}
			return null;
		}
		
		bool IsFxCopPath(string fxCopPath)
		{
			return !string.IsNullOrEmpty(fxCopPath) && File.Exists(Path.Combine(fxCopPath, ToolName));
		}
		
		string FromRegistry(RegistryKey key)
		{
			// Code duplication: FxCopWrapper.cs in CodeAnalysis addin
			if (key == null) return string.Empty;
			using (key) {
				string cmd = key.GetValue("").ToString();
				int pos;
				if (cmd.StartsWith("\""))
					pos = cmd.IndexOf('"', 1);
				else
					pos = cmd.IndexOf(' ');
				try {
					if (cmd.StartsWith("\""))
						return Path.GetDirectoryName(cmd.Substring(1, pos - 1));
					else
						return Path.GetDirectoryName(cmd.Substring(0, pos));
				} catch (ArgumentException ex) {
					Log.LogWarning(cmd);
					Log.LogWarningFromException(ex);
					return string.Empty;
				}
			}
		}
		
		protected override string GenerateFullPathToTool()
		{
			return Path.Combine(ToolPath.Trim('"'), ToolName);
		}
		
		static void AppendSwitch(StringBuilder b, string @switch, string val)
		{
			if (!string.IsNullOrEmpty(val)) {
				b.Append(" /");
				b.Append(@switch);
				b.Append(':');
				if (val[0] == '"') {
					b.Append(val);
				} else {
					b.Append('"');
					b.Append(val);
					b.Append('"');
				}
			}
		}
		
		//protected override string GenerateResponseFileCommands()
		protected override string GenerateCommandLineCommands()
		{
			// using a response file fails when the a path contains spaces, but using the command line
			// works fine (FxCop bug?)
			
			StringBuilder b = new StringBuilder();
			AppendSwitch(b, "o", realLogFile);
			AppendSwitch(b, "f", InputAssembly);
			if (ReferencePaths != null) {
				foreach (string path in ReferencePaths) {
					AppendSwitch(b, "d", Path.GetDirectoryName(path));
				}
			}
			if (RuleAssemblies != null) {
				foreach (string asm in RuleAssemblies) {
					if (asm.StartsWith("\\")) {
						AppendSwitch(b, "r", ToolPath + asm);
					} else {
						AppendSwitch(b, "r", asm);
					}
				}
			}
			if (Rules != null) {
				foreach (string rule in Rules) {
					AppendSwitch(b, "ruleid", rule);
				}
			}
			if (Dictionary != null) {
				foreach (ITaskItem dic in Dictionary) {
					AppendSwitch(b, "dic", dic.ItemSpec);
				}
			}
			#if DEBUG
			Console.WriteLine(b.ToString());
			#endif
			return b.ToString();
		}
	}
}
