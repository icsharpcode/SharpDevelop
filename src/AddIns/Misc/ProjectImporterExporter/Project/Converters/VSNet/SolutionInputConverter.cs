// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Windows.Forms;
using System.Reflection;
using System.Security.Permissions;
using System.Resources;

using System.Xml;
using System.Xml.Xsl;

using ICSharpCode.Core;

using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Gui;

using ICSharpCode.SharpDevelop.Commands;
using ICSharpCode.SharpDevelop.ProjectImportExporter.Converters;
using ICSharpCode.SharpDevelop.ProjectImportExporter.Dialogs;

namespace ICSharpCode.SharpDevelop.ProjectImportExporter.Converters
{
	public class SolutionInputConverter : AbstractInputConverter
	{
		string projectTitle;
		string projectInputDirectory;
		string projectOutputDirectory;
		string combineOutputFile;
		
		public override string FormatName {
			get {
				return "Visual Studio.NET 7 / 2003 Solutions";
			}
		}
		
		public override string OutputFile {
			get {
				return combineOutputFile;
			}
		}
		
		
		public override bool CanConvert(string fileName)
		{
			string upperExtension = Path.GetExtension(fileName).ToUpper();
			return upperExtension == ".SLN";
		}
		
		public override bool Convert(string solutionInputFile, string outputPath)
		{
			projectTitle = projectInputDirectory = projectOutputDirectory = combineOutputFile = null;
			
			ArrayList projects          = ReadSolution(solutionInputFile);
			ArrayList convertedProjects = new ArrayList();
			for (int i = 0; i < projects.Count; ++i) {
				DictionaryEntry entry = (DictionaryEntry)projects[i];
				this.projectTitle = entry.Key.ToString();
				
				string projectFile  = entry.Value.ToString();
				
				string projectInputFile       = Path.Combine(Path.GetDirectoryName(solutionInputFile), projectFile);
				this.projectOutputDirectory   = Path.Combine(outputPath, Path.GetDirectoryName(projectFile));
				
				if (!File.Exists(projectFile)) {
					using (ChooseProjectLocationDialog cpld = new ChooseProjectLocationDialog()) {
						cpld.FileName = projectFile;
						DialogResult res = cpld.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm);
						if (res == DialogResult.OK) {
							projectInputFile = projectFile = cpld.FileName;
							this.projectOutputDirectory   = Path.Combine(outputPath, Path.GetFileNameWithoutExtension(projectFile));
							entry = new DictionaryEntry(entry.Key, Path.Combine(Path.GetFileNameWithoutExtension(projectFile), Path.ChangeExtension(Path.GetFileName(projectFile), ".prjx")));
							projects[i] = entry;
						} else {
							continue;
						}
					}
				}
				
				string projectOutputFile      = Path.Combine(projectOutputDirectory, Path.ChangeExtension(Path.GetFileName(projectFile), ".prjx"));
				
				if (!Directory.Exists(projectOutputDirectory)) {
					Directory.CreateDirectory(projectOutputDirectory);
				}
				
				this.projectInputDirectory  = Path.GetDirectoryName(projectInputFile);
				switch (Path.GetExtension(projectFile).ToUpper()) {
					case ".VBPROJ":
						ConvertProject(projectInputFile, projectOutputFile, "VBSolutionConversion.xsl");
						convertedProjects.Add(entry);
						break;
					case ".CSPROJ":
						ConvertProject(projectInputFile, projectOutputFile, "CSSolutionConversion.xsl");
						convertedProjects.Add(entry);
						break;
					default:
						
						
						StringParser.Properties["ProjectFile"] = projectFile;
						MessageService.ShowError("${res:ICSharpCode.SharpDevelop.ProjectImportExporter.Converters.SolutionInputConverter.CantConvertProjectFileError}");
						break;
				}
			}
			combineOutputFile = Path.Combine(outputPath, Path.ChangeExtension(Path.GetFileName(solutionInputFile), ".cmbx"));
			WriteCombine(combineOutputFile, convertedProjects);
			return true;
		}
		
		void ConvertResource(string inputFile, string outputFile)
		{
			Hashtable resources = new Hashtable();
			
			// read .resx file
			try {
				Stream s              = File.OpenRead(inputFile);
				ResXResourceReader rx = new ResXResourceReader(s);
				IDictionaryEnumerator n = rx.GetEnumerator();
				while (n.MoveNext()) {
					if (!resources.ContainsKey(n.Key)) {
						resources.Add(n.Key, n.Value);
					}
				}
				
				rx.Close();
				s.Close();
			} catch (Exception e) {
				
				MessageService.ShowError(e, "Can't read resource file " + inputFile +"\nCheck file existance.");
			}
			
			// write .resources file
			try {
				ResourceWriter rw = new ResourceWriter(outputFile);
				foreach (DictionaryEntry entry in resources) {
					rw.AddResource(entry.Key.ToString(), entry.Value);
				}
				rw.Generate();
				rw.Close();
			} catch (Exception e) {
				
				MessageService.ShowError(e, "Can't generate resource file " + outputFile +"\nCheck for write permission.");
			}
		}
		
		void ConvertProject(string inputFile, string outputFile, string resourceStreamFile)
		{
			SolutionConversionTool solutionConversionTool = new SolutionConversionTool(projectTitle, projectInputDirectory, projectOutputDirectory);
			
			XsltArgumentList xsltArgumentList = new XsltArgumentList();
			xsltArgumentList.AddParam("ProjectTitle",          "", projectTitle);
			xsltArgumentList.AddExtensionObject("urn:convtool", solutionConversionTool);
			
			
			
			
			try {
				ConvertXml.Convert(inputFile,
				                   new XmlTextReader(new StreamReader(Assembly.GetCallingAssembly().GetManifestResourceStream(resourceStreamFile), Encoding.UTF8)),
				                   outputFile,
				                   xsltArgumentList);
			} catch (XmlException) {
				// try it again with the system encoding instead of UTF-8
				ConvertXml.Convert(inputFile,
				                   new XmlTextReader(new StreamReader(Assembly.GetCallingAssembly().GetManifestResourceStream(resourceStreamFile), Encoding.UTF8)),
				                   outputFile,
				                   xsltArgumentList,
				                   Encoding.Default);
			}
			
			foreach (DictionaryEntry entry in solutionConversionTool.copiedFiles) {
				string srcFile = entry.Key.ToString();
				string dstFile = entry.Value.ToString();
				if (File.Exists(srcFile)) {
					if (!Directory.Exists(Path.GetDirectoryName(dstFile))) {
						Directory.CreateDirectory(Path.GetDirectoryName(dstFile));
					}
					if (Path.GetExtension(srcFile).ToUpper() == ".RESX") {
						ConvertResource(srcFile, dstFile);
					} else {
						if (srcFile.ToLower() == dstFile.ToLower()) continue;
						try {
							File.Copy(srcFile, dstFile, true);
							File.SetAttributes(dstFile, FileAttributes.Normal);
						} catch (Exception e) {
							
							MessageService.ShowError(e, "Can't Copy file from " + srcFile +" to " + dstFile +". Copy it manually.");
						}
					}
				}
			}
			solutionConversionTool.copiedFiles = new ArrayList();
		}
		
		ArrayList ReadSolution(string fileName)
		{
			StreamReader sr           = File.OpenText(fileName);
			Regex projectLinePattern  = new Regex("Project\\(.*\\)\\s+=\\s+\"(?<Title>.*)\",\\s*\"(?<Location>.*)\",", RegexOptions.Compiled);
			ArrayList projects = new ArrayList();
			while (true) {
				string line = sr.ReadLine();
				if (line == null) {
					break;
				}
				Match match = projectLinePattern.Match(line);
				if (match.Success) {
					projects.Add(new DictionaryEntry(match.Result("${Title}"), match.Result("${Location}")));
				}
			}
			sr.Close();
			return projects;
		}
		
		void WriteCombine(string fileName, ArrayList projects)
		{
			StreamWriter sw = File.CreateText(fileName);
			sw.WriteLine("<Combine fileversion=\"1.0\" name=\"" + Path.GetFileNameWithoutExtension(fileName) + "\" description=\"Converted Visual Studio.NET Solution\">");
			string firstEntry = null;
			sw.WriteLine("<Entries>");
			foreach (DictionaryEntry entry in projects) {
				if (firstEntry == null) {
					firstEntry = entry.Key.ToString();
				}
				sw.WriteLine("\t<Entry filename=\"." + Path.DirectorySeparatorChar + Path.ChangeExtension(entry.Value.ToString(), ".prjx") + "\" />");
			}
			sw.WriteLine("</Entries>");
			sw.WriteLine("<StartMode startupentry=\"" + firstEntry + "\" single=\"True\"/>");
			sw.WriteLine("<Configurations active=\"Debug\">");
			sw.WriteLine("<Configuration name=\"Debug\">");
			foreach (DictionaryEntry entry in projects) {
				sw.WriteLine("\t<Entry name=\"" + entry.Key + "\" configurationname=\"Debug\" build=\"False\" />");
			}
			sw.WriteLine("</Configuration>");
			sw.WriteLine("<Configuration name=\"Release\">");
			foreach (DictionaryEntry entry in projects) {
				sw.WriteLine("\t<Entry name=\"" + entry.Key + "\" configurationname=\"Release\" build=\"False\" />");
			}
			sw.WriteLine("</Configuration>");
			sw.WriteLine("</Configurations>");
			sw.WriteLine("</Combine>");
			sw.Close();
		}
	}
}
