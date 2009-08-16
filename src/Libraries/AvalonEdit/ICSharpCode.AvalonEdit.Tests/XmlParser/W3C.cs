// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Text;

using ICSharpCode.AvalonEdit.Xml;
using ICSharpCode.SharpZipLib.Zip;
using NUnit.Framework;

namespace ICSharpCode.AvalonEdit.Xml.Tests
{
	[TestFixture]
	public class W3C
	{
		readonly string zipFileName = @"XmlParser\W3C.zip";
		
		ZipFile zipFile;
		List<ZipEntry> files = new List<ZipEntry>();
		Dictionary<string, string> descriptions = new Dictionary<string, string>();
		
		[TestFixtureSetUp]
		public void OpenZipFile()
		{
			zipFile = new ZipFile(zipFileName);
			files.AddRange(zipFile.Cast<ZipEntry>().Where(zip => zip.IsFile));
			foreach(ZipEntry metaData in GetXmlFilesStartingWith("ibm/ibm_oasis")) {
				string conent = Decompress(metaData);
				var doc = System.Xml.Linq.XDocument.Parse(conent);
				foreach(var testElem in doc.Descendants("TEST")) {
					descriptions.Add("ibm/" + testElem.Attribute("URI").Value, testElem.Value.Replace("\n    ", "\n").TrimStart('\n'));
				}
			}
		}
		
		string Decompress(ZipEntry zipEntry)
		{
			Stream stream = zipFile.GetInputStream(zipEntry);
			return new StreamReader(stream).ReadToEnd();
		}
		
		IEnumerable<ZipEntry> GetXmlFilesStartingWith(string directory)
		{
			return files.Where(f => f.Name.StartsWith(directory) && f.Name.EndsWith(".xml"));
		}
		
		[Test]
		public void Valid()
		{
			string exlucde = "ibm32v02"; // Too long enity name
			TestFiles(GetXmlFilesStartingWith("ibm/valid/").Where(f => !f.Name.Contains(exlucde)), true);
		}
		
		[Test]
		public void Invalid()
		{
			TestFiles(GetXmlFilesStartingWith("ibm/invalid/"), true);
		}
		
		[Test]
		[Ignore]
		public void NotWellformed()
		{
			TestFiles(GetXmlFilesStartingWith("ibm/not-wf/"), false);
		}
		
		StringBuilder errorOutput;
		
		void TestFiles(IEnumerable<ZipEntry> files, bool areWellFormed)
		{
			errorOutput = new StringBuilder();
			int testsRun = 0;
			foreach (ZipEntry file in files) {
				testsRun++;
				TestFile(file, areWellFormed);
			}
			if (testsRun == 0) {
				Assert.Fail("Test files not found");
			}
			if (errorOutput.Length > 0) {
				Assert.Fail(errorOutput.ToString());
			}
		}
		
		void TestFile(ZipEntry zipEntry, bool isWellFormed)
		{
			string fileName = zipEntry.Name;
			Debug.WriteLine("Testing " + fileName + "...");
			string content = Decompress(zipEntry);
			string description = null;
			descriptions.TryGetValue(fileName, out description);
			AXmlParser parser = new AXmlParser(content);
			parser.EntityReferenceIsError = false;
			var document = parser.Parse();
			
			string printed = PrettyPrintAXmlVisitor.PrettyPrint(document);
			if (content != printed) {
				errorOutput.AppendFormat("Output of pretty printed XML for \"{0}\" does not match the original.\n", fileName);
				errorOutput.AppendFormat("File content:\n{0}\n", Indent(content));
				errorOutput.AppendFormat("Pretty printed:\n{0}\n", Indent(printed));
				errorOutput.AppendLine();
			}
			
			bool hasErrors = document.SyntaxErrors.FirstOrDefault() != null;
			if (isWellFormed && hasErrors) {
				errorOutput.AppendFormat("Syntax error(s) in well formed file \"{0}\":\n", fileName);
				foreach (var error in document.SyntaxErrors) {
					string followingText = content.Substring(error.StartOffset, Math.Min(10, content.Length - error.StartOffset));
					errorOutput.AppendFormat("Error ({0}-{1}): {2} (followed by \"{3}\")\n", error.StartOffset, error.EndOffset, error.Message, followingText);
				}
				if (description != null) {
					errorOutput.AppendFormat("Test description:\n{0}\n", Indent(description));
				}
				errorOutput.AppendFormat("File content:\n{0}\n", Indent(content));
				errorOutput.AppendLine();
			}
			
			if (!isWellFormed && !hasErrors) {
				errorOutput.AppendFormat("No syntax errors reported for mallformed file \"{0}\"\n", fileName);
				if (description != null) {
					errorOutput.AppendFormat("Test description:\n{0}\n", Indent(description));
				}
				errorOutput.AppendFormat("File content:\n{0}\n", Indent(content));
				errorOutput.AppendLine();
			}
		}
		
		string Indent(string text)
		{
			return "  " + text.TrimEnd().Replace("\n", "\n  ");
		}
	}
}
