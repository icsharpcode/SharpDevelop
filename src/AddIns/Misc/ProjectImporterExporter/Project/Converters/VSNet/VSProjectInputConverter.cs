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
	public class VSProjectInputConverter : AbstractInputConverter
	{
		string projectTitle;
		string projectInputDirectory;
		string combineOutputFile;
		
		string projectOutputDirectory;
		
		public override string FormatName {
			get {
				return "Visual Studio.NET 7 / 2003 C# and VB.NET Projects";
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
			return upperExtension == ".VBPROJ" || upperExtension == ".CSPROJ";
		}
		
		public override bool Convert(string solutionInputFile, string outputPath)
		{
			this.projectOutputDirectory   = outputPath;
			string projectOutputFile      = Path.Combine(projectOutputDirectory, Path.ChangeExtension(Path.GetFileName(solutionInputFile), ".prjx"));
			
			projectTitle           = Path.GetFileNameWithoutExtension(solutionInputFile);
			projectInputDirectory  = Path.GetDirectoryName(solutionInputFile);
			combineOutputFile      = Path.Combine(outputPath, Path.ChangeExtension(Path.GetFileName(solutionInputFile), ".prjx"));
			
			switch (Path.GetExtension(solutionInputFile).ToUpper()) {
				case ".VBPROJ":
					ConvertProject(solutionInputFile, projectOutputFile, "VBSolutionConversion.xsl");
					break;
				case ".CSPROJ":
					ConvertProject(solutionInputFile, projectOutputFile, "CSSolutionConversion.xsl");
					break;
				default:
					
					MessageService.ShowError("${res:ICSharpCode.SharpDevelop.ProjectImportExporter.Converters.SolutionInputConverter.CantConvertProjectFileError}");
					break;
			}
			
			return true;
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
		
	}
}
