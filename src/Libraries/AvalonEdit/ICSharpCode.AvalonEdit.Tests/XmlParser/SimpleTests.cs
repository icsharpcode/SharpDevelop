// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Text;

using ICSharpCode.AvalonEdit.Xml;
using ICSharpCode.SharpZipLib.Zip;
using NUnit.Framework;

namespace ICSharpCode.AvalonEdit.Xml
{
	[TestFixture]
	[Ignore("XmlParser API broken")]
	public class SimpleTests
	{
		string tmpPath;
		
		[TestFixtureSetUp]
		public void PrepareFiles()
		{
			try {
				tmpPath = Path.Combine(Path.GetTempPath(), "XMLTestCases" + new Random().Next(0, 100));
				Directory.CreateDirectory(tmpPath);
				UncompressZip("XmlParser\\testcases.zip", tmpPath);
			} catch (Exception) {
				Directory.Delete(tmpPath, true);
			}
		}
		
		static void UncompressZip(string fileName, string outputFolder)
		{
			ZipInputStream zipIn = new ZipInputStream(File.OpenRead(fileName));
			ZipEntry entry;
			while ((entry = zipIn.GetNextEntry()) != null) {
				string path = Path.Combine(outputFolder, entry.Name);
				if (entry.IsDirectory) {
					Directory.CreateDirectory(path);
					continue;
				}
				FileStream streamWriter = File.Create(path);
				long size = entry.Size;
				byte[] data = new byte[size];
				while (true) {
					size = zipIn.Read(data, 0, data.Length);
					if (size > 0)
						streamWriter.Write(data, 0, (int) size);
					else
						break;
				}
				streamWriter.Close();
			}
		}
		
		[TestFixtureTearDown]
		public void DisposeFiles()
		{
			Directory.Delete(tmpPath, true);
		}
		
		[Test]
		public void FullParseTests()
		{
			foreach (FileInfo file in new DirectoryInfo(Path.Combine(tmpPath, "valid")).GetFiles("*.xml")) {
				FullParseTest(file.FullName, true);
			}
		}
		
		[Test]
		public void FullParseTestsInvalid()
		{
			foreach (FileInfo file in new DirectoryInfo(Path.Combine(tmpPath, "invalid")).GetFiles("*.xml")) {
				FullParseTest(file.FullName, true);
			}
		}
		
		[Test]
		public void FullParseTestsNotWellformed()
		{
			foreach (FileInfo file in new DirectoryInfo(Path.Combine(tmpPath, "not-wellformed")).GetFiles("*.xml")) {
				FullParseTest(file.FullName, false);
			}
		}
		
		void FullParseTest(string fileName, bool isWellFormed)
		{
			Console.WriteLine(fileName);
			string content = File.ReadAllText(fileName);
			AXmlParser parser = new AXmlParser(content);
			parser.EntityReferenceIsError = false;
			var document = parser.Parse();
			PrettyPrintAXmlVisitor printer = new PrettyPrintAXmlVisitor();
			printer.VisitDocument(document);
			
			StringBuilder errorsOutput = new StringBuilder();
			int count = 0;
			foreach (var error in document.GetSelfAndAllChildren().SelectMany(c => c.SyntaxErrors)) {
				count++;
				errorsOutput.AppendFormat("({0}-{1}): {2}\nError at: {3}\n", error.StartOffset, error.EndOffset, error.Message, content.Substring(error.StartOffset));
			}
			if (isWellFormed && count != 0)
				Assert.Fail("Syntax error in well formed file \"{0}\":\n{1}\nFile content:\n{2}", fileName, errorsOutput.ToString(), content);
			else if (!isWellFormed && count == 0)
				Assert.Fail("No syntax error reported for mallformed file \"{0}\"\nFile Content:\n{1}", fileName, content);
			
			Assert.AreEqual(content, printer.Output, "Output of pretty printed xml for '{0}' does not match the original.\nOriginal:\n{1}\nPretty printed:\n{2}", fileName, content, printer.Output);
		}
	}
}
