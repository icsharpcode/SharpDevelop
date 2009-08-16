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
		
		/// <remarks> Also excludes "/out/" files </remarks>
		IEnumerable<ZipEntry> GetXmlFilesStartingWith(string directory)
		{
			return files.Where(f => f.Name.StartsWith(directory) && f.Name.EndsWith(".xml") && !f.Name.Contains("/out/"));
		}
		
		[Test]
		public void Valid()
		{
			string[] exclude = {
				"ibm02v01", "ibm03v01", "ibm85v01", "ibm86v01", "ibm87v01", "ibm88v01", "ibm89v01", // NAME in DTD infoset
			};
			TestFiles(GetXmlFilesStartingWith("ibm/valid/"), true, exclude);
		}
		
		[Test]
		public void Invalid()
		{
			string[] exclude = {
				"ibm56i03", // Default attribute value
			};
			TestFiles(GetXmlFilesStartingWith("ibm/invalid/"), true, exclude);
		}
		
		[Test]
		[Ignore]
		public void NotWellformed()
		{
			string[] exclude = {
			};
			TestFiles(GetXmlFilesStartingWith("ibm/not-wf/"), false, exclude);
		}
		
		StringBuilder errorOutput;
		
		void TestFiles(IEnumerable<ZipEntry> files, bool areWellFormed, string[] exclude)
		{
			errorOutput = new StringBuilder();
			int testsRun = 0;
			foreach (ZipEntry file in files) {
				if (exclude.Any(exc => file.Name.Contains(exc))) continue;
				testsRun++;
				TestFile(file, areWellFormed);
			}
			if (testsRun == 0) {
				Assert.Fail("Test files not found");
			}
			if (errorOutput.Length > 0) {
				Assert.Fail(errorOutput.Replace("]]>", "]]~NUNIT~>").ToString());
			}
		}
		
		/// <remarks>
		/// If using DTD, canonical representation is not checked
		/// If using DTD, uknown entiry references are not error
		/// </remarks>
		bool TestFile(ZipEntry zipEntry, bool isWellFormed)
		{
			bool passed = true;
			
			string fileName = zipEntry.Name;
			Debug.WriteLine("Testing " + fileName + "...");
			string content = Decompress(zipEntry);
			string description = null;
			descriptions.TryGetValue(fileName, out description);
			AXmlParser parser = new AXmlParser(content);
			
			bool usingDTD = content.Contains("<!DOCTYPE") && (content.Contains("<!ENTITY") || content.Contains(" SYSTEM "));
			if (usingDTD)
				parser.UknonwEntityReferenceIsError = false;
			
			var document = parser.Parse();
			
			string printed = PrettyPrintAXmlVisitor.PrettyPrint(document);
			if (content != printed) {
				errorOutput.AppendFormat("Output of pretty printed XML for \"{0}\" does not match the original.\n", fileName);
				errorOutput.AppendFormat("File content:\n{0}\n", Indent(content));
				errorOutput.AppendFormat("Pretty printed:\n{0}\n", Indent(printed));
				errorOutput.AppendLine();
				passed = false;
			}
			
			if (isWellFormed && !usingDTD) {
				string canonicalFilename = fileName.Replace("/ibm", "/out/ibm");
				ZipEntry canonicalFile = files.FirstOrDefault(f => f.Name == canonicalFilename);
				if (canonicalFile != null) {
					string canonicalContent = Decompress(canonicalFile);
					string canonicalPrint = CanonicalPrintAXmlVisitor.Print(document);
					if (canonicalContent != canonicalPrint) {
						errorOutput.AppendFormat("Canonical XML for \"{0}\" does not match the excpected.\n", fileName);
						errorOutput.AppendFormat("Expected:\n{0}\n", Indent(canonicalContent));
						errorOutput.AppendFormat("Seen:\n{0}\n", Indent(canonicalPrint));
						errorOutput.AppendFormat("File content:\n{0}\n", Indent(content));
						errorOutput.AppendLine();
						passed = false;
					}
				} else {
					errorOutput.AppendFormat("Can not find canonical file for \"{0}\"", fileName);
					errorOutput.AppendLine();
					passed = false;
				}
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
				passed = false;
			}
			
			if (!isWellFormed && !hasErrors) {
				errorOutput.AppendFormat("No syntax errors reported for mallformed file \"{0}\"\n", fileName);
				if (description != null) {
					errorOutput.AppendFormat("Test description:\n{0}\n", Indent(description));
				}
				errorOutput.AppendFormat("File content:\n{0}\n", Indent(content));
				errorOutput.AppendLine();
				passed = false;
			}
			
			return passed;
		}
		
		string Indent(string text)
		{
			return "  " + text.TrimEnd().Replace("\n", "\n  ");
		}
	}
}
