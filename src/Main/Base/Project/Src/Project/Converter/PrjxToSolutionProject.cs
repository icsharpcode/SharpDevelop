// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.Generic;
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
			
			public string basePath;
			
			public string GetGuid(string name)
			{
				return "{" + NameToGuid[name] + "}";
			}
			
			public string GetRelativeProjectPath(string name)
			{
				return FileUtility.GetRelativePath(basePath, NameToPath[name]);
			}
			
			public string CanocializeFileName(string fileName)
			{
				if (fileName.StartsWith("./") || fileName.StartsWith(".\\")) {
					return fileName.Substring(2);
				}
				return fileName;
			}
			
			public string CanocializePath(string fileName) 
			{
				return CanocializeFileName(fileName)+ Path.DirectorySeparatorChar;
			}
			
			public string GetFileName(string fileName) 
			{
				return Path.GetFileName(fileName);
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
		
		static void RunConverter(string inFile, string outFile, string script, Conversion conversion)
		{
			conversion.basePath = Path.GetDirectoryName(inFile);
			
			//Create a new XslTransform object.
			XslTransform xslt = new XslTransform();
			
			//Load the stylesheet.
			xslt.Load(FileUtility.Combine(PropertyService.DataDirectory, "ConversionStyleSheets", script));
			
			//Create a new XPathDocument and load the XML data to be transformed.
			XPathDocument mydata = new XPathDocument(inFile);
			
			//Create an XmlTextWriter which outputs to the console.
			using (XmlTextWriter writer = new XmlTextWriter(outFile, Encoding.UTF8)) {
				writer.Formatting = Formatting.Indented;
				
				XsltArgumentList argList = new XsltArgumentList();
				argList.AddExtensionObject("urn:Conversion", conversion);
				
				//Transform the data and send the output to the console.
				xslt.Transform(mydata,argList,writer, null);
			}
			
//			System.Xml.Xsl.XslCompiledTransform xslt = new System.Xml.Xsl.XslCompiledTransform();
//			
//			xslt.Compile(FileUtility.Combine(PropertyService.DataDirectory, "ConversionStyleSheets", script));
//			
//			XmlArgumentList argList = new XmlArgumentList();
//			
//			argList.AddExtensionObject("urn:Conversion", conversion);
//			
//			if (File.Exists(outFile)) {
//				File.Delete(outFile);
//			}
//			using (Stream outStream = File.OpenWrite(outFile)) {
//				xslt.Execute(inFile, new XmlUrlResolver(), argList, outStream);
//			}
		}
		
		public static IProject ConvertOldProject(string fileName, Conversion conversion)
		{
			string convertedFileName = Path.ChangeExtension(fileName, ".csproj");
			RunConverter(fileName, 
			             convertedFileName,
			             "CSharp_prjx2csproj.xsl",
			             conversion);
			
			RunConverter(fileName, 
			             convertedFileName + ".user",
			             "CSharp_prjx2csproj_user.xsl",
			             conversion);
			
			ILanguageBinding binding = LanguageBindingService.GetBindingPerProjectFile(convertedFileName);
			return binding.LoadProject(convertedFileName, Conversion.GetProjectName(fileName));
		}
	}
}
