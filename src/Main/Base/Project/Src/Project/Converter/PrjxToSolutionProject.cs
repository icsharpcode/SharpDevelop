// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Xsl;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Project.Converter
{
	/// <summary>
	/// Description of Class1.
	/// </summary>
	public static class PrjxToSolutionProject
	{
		public class Conversion
		{
			public Dictionary<string, Guid>   NameToGuid = new Dictionary<string, Guid>();
			public Dictionary<string, string> NameToPath = new Dictionary<string, string>();
			/// <summary>only in VS03 -&gt; MSBuild conversion</summary>
			public Dictionary<Guid,   string> GuidToPath = new Dictionary<Guid,   string>();
			
			public bool IsVisualBasic;
			public List<string> Resources;
			
			public string basePath;
			
			public string GetLanguageName()
			{
				return IsVisualBasic ? "VisualBasic" : "CSharp";
			}
			
			public string GetGuid(string name)
			{
				return "{" + NameToGuid[name].ToString().ToUpperInvariant() + "}";
			}
			
			public string GetRelativeProjectPath(string name)
			{
				if (!NameToPath.ContainsKey(name)) {
					if (MessageService.AskQuestion("Project reference to " + name + " could not be resolved.\n" +
					                               "Do you want to specify it manually?")) {
						using (OpenFileDialog dlg = new OpenFileDialog()) {
							dlg.Title = "Find " + name;
							dlg.InitialDirectory = basePath;
							dlg.Filter = "SharpDevelop 1.x project|*.prjx";
							if (dlg.ShowDialog() == DialogResult.OK) {
								NameToPath[name] = dlg.FileName;
								NameToGuid[name] = Guid.NewGuid();
								return FileUtility.GetRelativePath(basePath, NameToPath[name]);
							}
						}
					}
					return "NotFound." + name + ".proj";
				}
				return FileUtility.GetRelativePath(basePath, NameToPath[name]);
			}
			
			public string GetRelativeProjectPathByGuid(string name, string guidText)
			{
				if (!NameToPath.ContainsKey(name)) {
					Guid guid = new Guid(guidText);
					if (!GuidToPath.ContainsKey(guid)) {
						MessageService.ShowWarning("Project reference to " + name + " could not be resolved.");
						return "NotFound." + name + ".proj";
					}
					return GuidToPath[guid];
				}
				return NameToPath[name];
			}
			
			public bool IsNotGacReference(string hintPath)
			{
				if (hintPath == null || hintPath.Length == 0)
					return false;
				return !FileUtility.IsBaseDirectory(FileUtility.NetFrameworkInstallRoot, hintPath);
			}
			
			string rootNamespace;
			
			public string SetRootNamespace(string ns)
			{
				return rootNamespace = ns;
			}
			
			/// <summary>
			/// Convert SharpDevelop 1.x resource to SharpDevelop 2.x resource.
			/// SharpDevelop 1.x includes resources by their filename as resource,
			/// SD 2.x/MsBuild also uses the project's root namespace and the directory name(s).
			/// </summary>
			public string ConvertResource(string fileName)
			{
				if (Resources == null)
					Resources = new List<string>();
				fileName = CanocializeFileName(fileName);
				string name = Path.GetFileName(fileName);
				if (rootNamespace.Length > 0) {
					if (name.StartsWith(rootNamespace + ".")) {
						name = name.Substring(rootNamespace.Length + 1);
						name = ConvertResourceInternal(fileName, name);
						if (name != null)
							return name;
					}
				} else {
					name = ConvertResourceInternal(fileName, name);
					if (name != null)
						return name;
				}
				Resources.Add(Path.Combine(basePath, fileName));
				return fileName;
			}
			
			string ConvertResourceInternal(string fileName, string name)
			{
				string[] parts = name.Split('.');
				string directory = basePath;
				for (int i = 0; i < parts.Length; i++) {
					if (Directory.Exists(Path.Combine(directory, parts[i]))) {
						directory = Path.Combine(directory, parts[i]);
					} else {
						directory = Path.Combine(directory, parts[i]);
						for (int j = i + 1; j < parts.Length; j++) {
							directory += '.' + parts[j];
						}
						try {
							File.Move(Path.Combine(basePath, fileName), directory);
							return FileUtility.GetRelativePath(basePath, directory);
						} catch {}
						break;
					}
				}
				return null;
			}
			
			public string CanocializeFileName(string fileName)
			{
				if (fileName.StartsWith("..\\") || fileName.StartsWith("../")) {
					// work around a bug in older Fidalgo versions
					if (!File.Exists(Path.Combine(basePath, fileName))) {
						string fixedFileName = fileName.Substring(3);
						if (File.Exists(Path.Combine(basePath, fixedFileName))) {
							fileName = fixedFileName;
						}
					}
				}
				if (fileName.StartsWith("./") || fileName.StartsWith(".\\")) {
					return fileName.Substring(2);
				}
				return fileName;
			}
			
			public string CanocializePath(string fileName)
			{
				return CanocializeFileName(fileName) + Path.DirectorySeparatorChar;
			}
			
			public string Negate(string booleanString)
			{
				return "false".Equals(booleanString, StringComparison.OrdinalIgnoreCase).ToString();
			}
			
			public string GetFileName(string fileName)
			{
				return Path.GetFileName(fileName);
			}
			
			public string GetFileNameWithoutExtension(string fileName)
			{
				return Path.GetFileNameWithoutExtension(fileName);
			}
			
			public string ConvertBuildEvent(string executeScript, string arguments)
			{
				if (executeScript == null || executeScript.Length == 0) {
					return "";
				}
				
				if (arguments != null && arguments.Length > 0) {
					return FileUtility.GetAbsolutePath(basePath, executeScript) + " " + arguments;
				}
				return FileUtility.GetAbsolutePath(basePath, executeScript);
			}
			
			public static string GetProjectName(string fileName)
			{
				XmlTextReader reader = new XmlTextReader(fileName);
				try {
					reader.MoveToContent();
					if (reader.MoveToAttribute("name")) {
						return reader.Value;
					}
				} finally {
					reader.Close();
				}
				return fileName;
			}
		}
		
		static Dictionary<string, XslCompiledTransform> xsltDict = new Dictionary<string, XslCompiledTransform>();
		
		public static void RunConverter(TextReader inFile, string outFile, string script, Conversion conversion)
		{
			XslCompiledTransform xslt;
			if (xsltDict.ContainsKey(script)) {
				xslt = xsltDict[script];
			} else {
				//Create a new XslTransform object.
				xslt = new XslCompiledTransform();
				//Load the stylesheet.
				xslt.Load(FileUtility.Combine(PropertyService.DataDirectory, "ConversionStyleSheets", script));
				
				xsltDict[script] = xslt;
			}
			
			StringWriter stringWriter = new StringWriter();
			using (XmlTextReader reader = new XmlTextReader(inFile)) {
				//Create an XmlTextWriter which outputs to memory.
				using (XmlTextWriter writer = new XmlTextWriter(stringWriter)) {
					XsltArgumentList argList = new XsltArgumentList();
					argList.AddExtensionObject("urn:Conversion", conversion);
					
					//Transform the data and send the output to the console.
					xslt.Transform(reader, argList, writer, null);
				}
			}
			// We have to use the stringWriter for writing because xslt.Transform doesn't use
			// writer.Formatting. Also, we need to remove some unwanted whitespace from the beginning.
			using (XmlTextWriter writer = new XmlTextWriter(outFile, Encoding.UTF8)) {
				writer.Formatting = Formatting.Indented;
				using (XmlTextReader reader = new XmlTextReader(new StringReader(stringWriter.ToString()))) {
					reader.WhitespaceHandling = WhitespaceHandling.Significant;
					writer.WriteNode(reader, false);
				}
			}
		}
		
		public static IProject ConvertOldProject(string fileName, Conversion conversion, IMSBuildEngineProvider provider)
		{
			string convertedFileName;
			if (conversion.IsVisualBasic)
				convertedFileName = Path.ChangeExtension(fileName, ".vbproj");
			else
				convertedFileName = Path.ChangeExtension(fileName, ".csproj");
			
			conversion.basePath = Path.GetDirectoryName(fileName);
			using (StreamReader fileReader = new StreamReader(fileName)) {
				RunConverter(fileReader, convertedFileName, "CSharp_prjx2csproj.xsl", conversion);
			}
			using (StreamReader fileReader = new StreamReader(fileName)) {
				RunConverter(fileReader, convertedFileName + ".user", "CSharp_prjx2csproj_user.xsl", conversion);
			}
			
			return LanguageBindingService.LoadProject(provider, convertedFileName, Conversion.GetProjectName(fileName));
		}
		
		public static void ConvertVSNetProject(string fileName)
		{
			string old = fileName + ".old";
			string userFile = fileName + ".user";
			string oldUserFile = fileName + ".user.old";
			File.Copy(fileName, old, true);
			File.Delete(fileName);
			if (File.Exists(userFile)) {
				File.Copy(userFile, oldUserFile, true);
				File.Delete(userFile);
			}
			Conversion conversion = new Conversion();
			if (Path.GetExtension(fileName).ToLowerInvariant() == ".vbproj")
				conversion.IsVisualBasic = true;
			if (Solution.SolutionBeingLoaded != null) {
				Solution.ReadSolutionInformation(Solution.SolutionBeingLoaded.FileName, conversion);
			}
			
			conversion.basePath = Path.GetDirectoryName(fileName);
			
			string content = ICSharpCode.TextEditor.Util.FileReader.ReadFileContent(old, Encoding.Default);
			RunConverter(new StringReader(content), fileName, "vsnet2msbuild.xsl", conversion);
			if (File.Exists(oldUserFile)) {
				content = ICSharpCode.TextEditor.Util.FileReader.ReadFileContent(oldUserFile, Encoding.Default);
				RunConverter(new StringReader(content), userFile, "vsnet2msbuild_user.xsl", conversion);
			}
		}
	}
}
