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
	public class XmlParserTests
	{
		readonly string zipFileName = @"XmlParser\testcases.zip";
		
		ZipFile zipFile;
		List<ZipEntry> files = new List<ZipEntry>();
		
		[TestFixtureSetUp]
		public void OpenZipFile()
		{
			zipFile = new ZipFile(zipFileName);
			files.AddRange(zipFile.Cast<ZipEntry>().Where(zip => zip.IsFile));
		}
		
		string Decompress(ZipEntry zipEntry)
		{
			byte[] data = new byte[zipEntry.Size];
			Stream stream = zipFile.GetInputStream(zipEntry);
			string text = new StreamReader(stream).ReadToEnd();
			return text;
		}
		
		IEnumerable<ZipEntry> GetFiles(string directory)
		{
			return files.Where(f => f.Name.StartsWith(directory + @"/") && f.Name.EndsWith(".xml"));
		}
		
		[Test]
		public void Valid()
		{
			foreach (ZipEntry file in GetFiles("valid")) {
				if (file.Name == "valid/042.xml") continue; // Long entity reference
				if (file.Name == "valid/056.xml") continue; // Long entity reference
				ParseTest(file, true);
			}
		}
		
		[Test]
		public void Invalid()
		{
			foreach (ZipEntry file in GetFiles("invalid")) {
				ParseTest(file, true);
			}
		}
		
		[Test]
		[Ignore("Unfinished")]
		public void NotWellformed()
		{
			foreach (ZipEntry file in GetFiles("not-wellformed")) {
				ParseTest(file, false);
			}
		}
		
		void ParseTest(ZipEntry zipEntry, bool isWellFormed)
		{
			string fileName = zipEntry.Name;
			System.Diagnostics.Debug.WriteLine("\nTesting " + fileName + "...");
			string content = Decompress(zipEntry);
			AXmlParser parser = new AXmlParser(content);
			parser.EntityReferenceIsError = false;
			var document = parser.Parse();
			string printed = PrettyPrintAXmlVisitor.PrettyPrint(document);
			
			int errorCount = 0;
			StringBuilder errorsOutput = new StringBuilder();
			foreach (var error in document.SyntaxErrors) {
				errorCount++;
				string followingText = content.Substring(error.StartOffset, Math.Min(16, content.Length - error.StartOffset));
				errorsOutput.AppendFormat("Error ({0}-{1}): {2}\nFollowing text: {3}\n", error.StartOffset, error.EndOffset, error.Message, followingText);
			}
			if (isWellFormed && errorCount != 0)
				Assert.Fail("Syntax error(s) in well formed file \"{0}\":\n{1}File content:\n{2}\n\n", fileName, errorsOutput, content);
			if (!isWellFormed && errorCount == 0)
				Assert.Fail("No syntax error reported for mallformed file \"{0}\"\nFile Content:\n{1}\n\n", fileName, content);
			
			Assert.AreEqual(content, printed, "Output of pretty printed XML for \"{0}\" does not match the original.", fileName);
		}
	}
}
