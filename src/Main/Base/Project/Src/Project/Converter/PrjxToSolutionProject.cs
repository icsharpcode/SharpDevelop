// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Xml;
using System.Xml.XPath;
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
			
			public bool IsVisualBasic;
			public List<string> Resources;
			
			public string basePath;
			
			public string GetLanguageName()
			{
				return IsVisualBasic ? "VisualBasic" : "CSharp";
			}
			
			public string GetGuid(string name)
			{
				return "{" + NameToGuid[name].ToString().ToUpper() + "}";
			}
			
			public string GetRelativeProjectPath(string name)
			{
				return FileUtility.GetRelativePath(basePath, NameToPath[name]);
			}
			
			public bool IsNotGacReference(string hintPath)
			{
				return FileUtility.IsBaseDirectory(FileUtility.NETFrameworkInstallRoot, hintPath);
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
				return (!bool.Parse(booleanString)).ToString();
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
		
		public static void RunConverter(string inFile, string outFile, string script, Conversion conversion)
		{
			conversion.basePath = Path.GetDirectoryName(inFile);
			
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
			
			using (XmlTextReader reader = new XmlTextReader(inFile)) {
				//Create an XmlTextWriter which outputs to the console.
				using (XmlTextWriter writer = new XmlTextWriter(outFile, Encoding.UTF8)) {
					writer.Formatting = Formatting.Indented;
					
					XsltArgumentList argList = new XsltArgumentList();
					argList.AddExtensionObject("urn:Conversion", conversion);
					
					//Transform the data and send the output to the console.
					xslt.Transform(reader, argList, writer, null);
				}
			}
		}
		
		public static IProject ConvertOldProject(string fileName, Conversion conversion)
		{
			string convertedFileName;
			if (conversion.IsVisualBasic)
				convertedFileName = Path.ChangeExtension(fileName, ".vbproj");
			else
				convertedFileName = Path.ChangeExtension(fileName, ".csproj");
			
			RunConverter(fileName, convertedFileName, "CSharp_prjx2csproj.xsl", conversion);
			
			RunConverter(fileName, convertedFileName + ".user", "CSharp_prjx2csproj_user.xsl", conversion);
			
			ILanguageBinding binding = LanguageBindingService.GetBindingPerProjectFile(convertedFileName);
			return binding.LoadProject(convertedFileName, Conversion.GetProjectName(fileName));
		}
		
		public static void ConvertVSNetProject(string fileName)
		{
			string old = fileName + ".old";
			string userFile = fileName + ".user";
			string oldUserFile = fileName + ".user.old";
			File.Copy(fileName, old, true);
			File.Delete(fileName);
			File.Copy(userFile, oldUserFile, true);
			File.Delete(userFile);
			Conversion conversion = new Conversion();
			if (Path.GetExtension(fileName).ToLower() == ".vbproj")
				conversion.IsVisualBasic = true;
			Solution.ReadSolutionInformation(Solution.SolutionBeingLoaded.FileName, conversion);
			RunConverter(old, fileName, "vsnet2msbuild.xsl", conversion);
			RunConverter(oldUserFile, userFile, "vsnet2msbuild_user.xsl", conversion);
		}
	}
}
