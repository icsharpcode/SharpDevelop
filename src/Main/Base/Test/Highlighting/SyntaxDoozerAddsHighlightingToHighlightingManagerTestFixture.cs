// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Tests.Utils;
using NUnit.Framework;

namespace ICSharpCode.SharpDevelop.Tests.Highlighting
{
	/* TODO: create Tests-project for AvalonEdit.AddIn and move this test there
	[TestFixture]
	public class SyntaxDoozerAddsHighlightingToHighlightingManagerTestFixture
	{
		SyntaxModeDoozer doozer;
		HighlightingManager highlightingManager;
		IHighlightingDefinition highlightingDefinition;
		
		[SetUp]
		public void Init()
		{
			Properties p = new Properties();
			p["name"] = "XML";
			p["extensions"] = ".xml;.xsd";
			p["resource"] = "ICSharpCode.Highlighting.Resource.xshd";
			
			AddIn addIn = CreateAddIn();
			addIn.FileName = @"D:\SharpDevelop\AddIns\XmlEditor.addin";
			addIn.Enabled = true;
			
			Codon codon = new Codon(addIn, "SyntaxMode", p, new ICondition[0]);
			
			byte[] bytes = UnicodeEncoding.UTF8.GetBytesWithPreamble(GetHighlightingDefinitionXml());
			Stream predefinedManifestResourceStream = new MemoryStream(bytes);
			MockAssembly assembly = new MockAssembly();
			assembly.AddManifestResourceStream("ICSharpCode.Highlighting.Resource.xshd", predefinedManifestResourceStream);
			
			List<AddIn> addIns = new List<AddIn>();
			addIns.Add(addIn);
			
			// Create runtimes.
			DerivedRuntime runtime = new DerivedRuntime("XmlEditor.dll", String.Empty, addIns);
			runtime.AssemblyFileNames.Add("XmlEditor.dll", assembly);
			
			addIn.Runtimes.Clear();
			addIn.Runtimes.Add(runtime);
			
			highlightingManager = new HighlightingManager();
			doozer = new SyntaxModeDoozer(highlightingManager);
			doozer.BuildItem(null, codon, null);
			
			highlightingDefinition = highlightingManager.GetDefinition("XML");
		}
		
		AddIn CreateAddIn()
		{
			string xml =
				"<AddIn name        = \"XML Editor\"\r\n" +
				"       author      = \"\"\r\n" +
				"       copyright   = \"prj:///doc/copyright.txt\"\r\n" +
				"       description = \"XML Editor\"\r\n" +
				"       addInManagerHidden = \"preinstalled\">\r\n" +
				"\r\n" +
				"    <Manifest>\r\n" +
				"        <Identity name = \"ICSharpCode.XmlEditor\"/>\r\n" +
				"    </Manifest>\r\n" +
				"\r\n" +
				"    <Runtime>\r\n" +
				"        <Import assembly = \"XmlEditor.dll\"/>\r\n" +
				"    </Runtime>\r\n" +
				"</AddIn>";
			return  AddIn.Load(new StringReader(xml));
		}
		
		string GetHighlightingDefinitionXml()
		{
			return 
				"<SyntaxDefinition name = \"XML\" extensions = \".xml\">\r\n" +
				"    <Environment>\r\n" +
				"        <Default color = \"Yellow\" bgcolor = \"Black\"/>\r\n" +
				"        <Selection color = \"White\" bgcolor = \"Purple\"/>\r\n" +
				"        <InvalidLines color = \"Red\"/>\r\n" +
				"        <LineNumbers color = \"Gray\" bgcolor = \"Black\"/>\r\n" +
				"        <SelectedFoldLine color = \"Green\" bgcolor=\"Black\"/>\r\n" +
				"    </Environment>\r\n" +
				"\r\n" +
				"    <Digits name = \"Digits\" bold = \"false\" italic = \"false\" color = \"Yellow\"/>\r\n" +
				"\r\n" +
				"    <RuleSets>\r\n" +
				"        <RuleSet ignorecase = \"false\">\r\n" +
				"            <Delimiters> </Delimiters>\r\n" +
				"        </RuleSet>\r\n" +
				"    </RuleSets>\r\n" +
				"</SyntaxDefinition>";
		}
		
		[Test]
		public void HighlightingManagerHasHighlightingDefinition()
		{
			Assert.AreEqual("XML", highlightingDefinition.Name);
		}
		
		[Test]
		public void XsdFileExtensionRegisteredWithHighlightingManager()
		{
			IHighlightingDefinition highlightingDefinition = highlightingManager.GetDefinitionByExtension(".xsd");
			Assert.AreEqual("XML", highlightingDefinition.Name);
		}
	}
	*/
}
