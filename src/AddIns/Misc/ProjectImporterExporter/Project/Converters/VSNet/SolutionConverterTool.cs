// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Security;
using System.Security.Permissions;

using System.Windows.Forms;

using MSjogren.GacTool.FusionNative;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.ProjectImportExporter.Commands;

namespace ICSharpCode.SharpDevelop.ProjectImportExporter.Converters
{
//	[UIPermissionAttribute(SecurityAction.Demand, Window = UIPermissionWindow.AllWindows, Unrestricted = true)]
	public class SolutionConversionTool
	{
		public ArrayList copiedFiles = new ArrayList();
		
		Hashtable GacReferences = new Hashtable();
		string projectTitle;
		string projectInputDirectory;
		string projectOutputDirectory;
		
		void WriteLine(string str)
		{
			Console.WriteLine(str);
//			
//			TaskService.CompilerOutput += str + "\n";
//			TaskService.NotifyTaskChange();
		}
		
		public SolutionConversionTool(string projectTitle, string projectInputDirectory, string projectOutputDirectory)
		{
			GenerateGacReferences();
			this.projectTitle           = projectTitle;
			this.projectInputDirectory  = projectInputDirectory;
			this.projectOutputDirectory = projectOutputDirectory;
		}
		
		void GenerateGacReferences()
		{
			IApplicationContext applicationContext = null;
			IAssemblyEnum assemblyEnum = null;
			IAssemblyName assemblyName = null;
			
			Fusion.CreateAssemblyEnum(out assemblyEnum, null, null, 2, 0);
			
			while (assemblyEnum.GetNextAssembly(out applicationContext, out assemblyName, 0) == 0) {
				uint nChars = 0;
				assemblyName.GetDisplayName(null, ref nChars, 0);
				
				StringBuilder sb = new StringBuilder((int)nChars);
				assemblyName.GetDisplayName(sb, ref nChars, 0);
				
				string[] info = sb.ToString().Split(',');
				
				string aName    = info[0];
				string aVersion = info[1].Substring(info[1].LastIndexOf('=') + 1);
				GacReferences[aName] = sb.ToString();
			}
		}
		
		static string[] commonAssemblies = new string[] {
			"mscorlib",
			"Accessibility",
			"Microsoft.Vsa",
			"System.Configuration.Install",
			"System.Data",
			"System.Design",
			"System.DirectoryServices",
			"System",
			"System.Drawing.Design",
			"System.Drawing",
			"System.EnterpriseServices",
			"System.Management",
			"System.Messaging",
			"System.Runtime.Remoting",
			"System.Runtime.Serialization.Formatters.Soap",
			"System.Security",
			"System.ServiceProcess",
			"System.Web",
			"System.Web.RegularExpressions",
			"System.Web.Services",
			"System.Windows.Forms",
			"System.XML"
		};
		
		public bool ShouldGenerateReference(bool filter, string assemblyName, string hintPath)
		{
			if (filter) {
				foreach (string reference in commonAssemblies) {
					if (reference.ToUpper() == assemblyName.ToUpper()) {
						return false;
					}
				}
			}
			
			if (hintPath != null && hintPath.Length > 0) {
				string assemblyLocation = Path.Combine(this.projectInputDirectory, hintPath);
				if (File.Exists(assemblyLocation)) {
					return true;
				}
			}
			
			if (!File.Exists(Path.Combine(this.projectInputDirectory, assemblyName))) {
				if (GacReferences[assemblyName] != null) {
					return true;
				}
			} else {
				return true;
			}
			this.WriteLine("Can't import reference " + assemblyName + " (" + hintPath + ")");
			return false;
		}
		
		public string GenerateReferenceType(string assemblyName, string hintPath)
		{
			if (hintPath != null && hintPath.Length > 0) {
				string assemblyLocation = Path.Combine(this.projectInputDirectory, hintPath);
				if (File.Exists(assemblyLocation)) {
					return "Assembly";
				}
			}
			
			if (!File.Exists(Path.Combine(this.projectInputDirectory, assemblyName))) {
				if (GacReferences[assemblyName] == null) {
					this.WriteLine("Can't find Assembly reference " + assemblyName);
				} else {
					return "Gac";
				}
			} else {
				return "Assembly";
			}
			
			this.WriteLine("Can't determine reference type for " + assemblyName);
			return "Assembly";
		}
		
		public string GenerateReference(string assemblyName, string hintPath)
		{
			if (hintPath != null && hintPath.Length > 0) {
				string assemblyLocation = Path.Combine(this.projectInputDirectory, hintPath);
				if (File.Exists(assemblyLocation)) {
					VerifyFileLocation(hintPath);
					return hintPath;
				}
			}
			
			if (!File.Exists(Path.Combine(this.projectInputDirectory, assemblyName))) {
				if (GacReferences[assemblyName] == null) {
					this.WriteLine("Can't find Assembly reference " + assemblyName);
				} else {
					return GacReferences[assemblyName].ToString();
				}
			} else {
				return "." + Path.DirectorySeparatorChar + assemblyName;
			}
			
			this.WriteLine("Created illegal, empty reference (should never happen) remove manually");
			return null;
		}
		
		public string VerifyFileLocation(string itemFile)
		{
			if (itemFile.Length == 0) {
				return String.Empty;
			}
			string itemInputFile = Path.Combine(this.projectInputDirectory, itemFile);
			if (itemInputFile.StartsWith("..")) {
				string correctLocation = this.projectOutputDirectory + Path.DirectorySeparatorChar +
					"MovedFiles" + Path.DirectorySeparatorChar + Path.GetFileName(itemFile);
				try {
					if (File.Exists(correctLocation)) {
						File.Delete(correctLocation);
					}
					this.WriteLine("Copy file " + itemInputFile + " to " + correctLocation);
					copiedFiles.Add(new DictionaryEntry(itemInputFile, correctLocation));
				} catch (Exception) {
//					
//					MessageService.ShowError(e, "Can't copy " + itemInputFile + " to " + correctLocation +"\nCheck for write permission.");
				}
				return "." + correctLocation.Substring(this.projectOutputDirectory.Length);
			}
			copiedFiles.Add(new DictionaryEntry(itemInputFile, this.projectOutputDirectory + Path.DirectorySeparatorChar + itemFile));
			return itemFile.StartsWith(".") ? itemFile : "." + Path.DirectorySeparatorChar + itemFile;
		}
		
		public string EnsureBool(string txt)
		{
			if (txt.ToUpper() == "TRUE") {
				return true.ToString();
			}
			return false.ToString();
		}
		
		public string Negate(string txt)
		{
			if (txt.ToUpper() == "TRUE") {
				return false.ToString();
			}
			return true.ToString();
		}
		
		/// <summary>
		/// Imports a resource file.
		/// </summary>
		/// <param name="resourceFile">The resource file.</param>
		/// <param name="dependentFile">The dependent source file.</param>
		/// <returns>The output resource file path.</returns>
		public string ImportDependentResource(string resourceFile, string dependentFile, string rootNamespace)
		{
			WriteLine("Import resource " + resourceFile);
			WriteLine("Searching namespace in " + dependentFile);
			
			string resourceOutputFile;
			
			string Namespace = null;
			if (dependentFile != null && dependentFile.Length > 0)
				Namespace = GetNamespaceFromFile(Path.Combine(this.projectInputDirectory, dependentFile));
			if (Namespace == null)
				Namespace = rootNamespace;
			WriteLine("  Namespace is '" + Namespace + "'");
			
			if (Namespace != null && Namespace.Length > 0) {
				resourceOutputFile = Path.Combine(Path.GetDirectoryName(resourceFile),
				                                  Namespace + "." + Path.GetFileName(resourceFile));
			} else {
				resourceOutputFile = resourceFile;
			}
			return CopyResource(resourceFile, resourceOutputFile);
		}
		
		#region GetNamespace
		// TODO: Get namespace using the parser, not with regular expressions.
		
		/// <summary>Gets the class namespace from a piece of code.</summary>
		/// <param name="code">The code to extract the namespace from.</param>
		/// <returns>The namespace of the classes in the source code.</returns>
		private string GetCSharpNamespace(string code)
		{
			// The regular expression that extracts the text
			// "namespace Name {" from a code string.
			
			string pattern = @"^[ \t]*namespace\s+([\w\d\.]+)\s*{";
			Regex regex = new Regex(pattern, RegexOptions.Multiline);
			Match match = regex.Match(code);
			if (match.Success)
				return match.Groups[1].Value;
			else
				return null;
		}
		
		/// <summary>Gets the class's namespace from a piece of code.</summary>
		/// <param name="code">The code to extract the namespace from.</param>
		/// <returns>The namespace of the classes in the source code.</returns>
		private string GetVBNamespace(string code)
		{
			string pattern = @"^[ \t]*Namespace\s+([\w\d\.]+)[ \t]*$";
			Regex regex = new Regex(pattern, RegexOptions.Multiline | RegexOptions.IgnoreCase);
			Match match = regex.Match(code);
			if (match.Success)
				return match.Groups[1].Value;
			else
				return null;
		}
		
		/// <summary>
		/// Gets the namespace of the first class specified in a file.
		/// </summary>
		/// <param name="filename">The filename to search for
		/// a namespace.</param>
		private string GetNamespaceFromFile(string filename)
		{
			try {
				using (StreamReader reader = new StreamReader(filename, true)) {
					string extension = Path.GetExtension(filename).ToLower();
					if (extension == ".cs") {
						return GetCSharpNamespace(reader.ReadToEnd());
					} else if (extension == ".vb") {
						return GetVBNamespace(reader.ReadToEnd());
					}
					return null;
				}
			} catch (Exception ex) {
				WriteLine(ex.ToString());
				return null;
			}
		}
		#endregion
		
		public string ImportResource(string resourceFile, string rootNamespace)
		{
			WriteLine("Import resource " + resourceFile + " (ns=" + rootNamespace + ")");
			
			string resourceOutputFile = resourceFile;
			
			if (!Path.IsPathRooted(resourceFile) && !resourceFile.StartsWith("..")) {
				resourceOutputFile = resourceOutputFile.Replace(Path.DirectorySeparatorChar, '.');
				resourceOutputFile = resourceOutputFile.Replace(Path.AltDirectorySeparatorChar, '.');
				
				if (rootNamespace == null || rootNamespace.Length == 0)
					resourceOutputFile = Path.Combine(Path.GetDirectoryName(resourceFile),
					                                  resourceOutputFile);
				else
					resourceOutputFile = Path.Combine(Path.GetDirectoryName(resourceFile),
					                                  rootNamespace + "." + resourceOutputFile);
			} else {
				if (rootNamespace == null || rootNamespace.Length == 0)
					resourceOutputFile = Path.GetFileName(resourceFile);
				else
					resourceOutputFile = rootNamespace + "." + Path.GetFileName(resourceFile);
			}
			
			return CopyResource(resourceFile, resourceOutputFile);
		}
		
		private string CopyResource(string inputFile, string outputFile) {
			inputFile = Path.Combine(this.projectInputDirectory, inputFile);
			if (Path.GetExtension(outputFile).ToUpper() == ".RESX") {
				outputFile = Path.ChangeExtension(outputFile, ".resources");
			}
			
			string outputFileFull = Path.Combine(this.projectOutputDirectory, outputFile);
			
			WriteLine("Needed to copy file " + inputFile + " to " + outputFileFull);
			copiedFiles.Add(new DictionaryEntry(inputFile, outputFileFull));
			return (outputFile.StartsWith(".") ? "" : "." + Path.DirectorySeparatorChar) + outputFile;
		}
	}
}
