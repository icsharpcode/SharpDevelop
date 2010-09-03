// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.Core;
using ICSharpCode.Core.Tests.Utils;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Tests.Utils;
using NUnit.Framework;

namespace ICSharpCode.SharpDevelop.Tests.Highlighting
{
	[TestFixture]
	public class AddInHighlightingResourceTests
	{
		AddInHighlightingResource highlightingResource;
		Stream predefinedManifestResourceStream;
		MockAssembly assembly;
		DerivedRuntime testRuntime;
		DerivedRuntime unloadedRuntime;
		
		[SetUp]
		public void Init()
		{
			// Create assembly.
			byte[] bytes = UnicodeEncoding.UTF8.GetBytesWithPreamble(GetHighlightingDefinitionXml());
			predefinedManifestResourceStream = new MemoryStream(bytes);
			assembly = new MockAssembly();
			assembly.AddManifestResourceStream("ICSharpCode.Xml.xshd", predefinedManifestResourceStream);
			
			// Create addins.
			AddIn addIn = AddIn.Load(new StringReader(GetAddInXml()));
			addIn.FileName = @"D:\SharpDevelop\AddIns\MyAddIn.addin";
			addIn.Enabled = true;
			
			List<AddIn> addIns = new List<AddIn>();
			addIns.Add(addIn);
			
			// Create runtimes.
			testRuntime = new DerivedRuntime("MyAddIn.dll", String.Empty, addIns);
			testRuntime.AssemblyFileNames.Add("MyAddIn.dll", assembly);
			
			unloadedRuntime = new DerivedRuntime("UnLoadedAssembly.dll", String.Empty, addIns);
			unloadedRuntime.AssemblyFileNames.Add("UnLoadedAssembly.dll", null);
			unloadedRuntime.LoadAssemblyFromExceptionToThrow = new FileNotFoundException("UnloadedAssembly.dll not found.");
			
			List<Runtime> runtimes = new List<Runtime>();
			runtimes.Add(testRuntime);
			runtimes.Add(unloadedRuntime);
			
			// Create addin highlighting resource.
			highlightingResource = new AddInHighlightingResource(runtimes.ToArray());
		}
		
		string GetHighlightingDefinitionXml()
		{
			return 
				"<SyntaxDefinition name = \"BAT\" extensions = \".bat\">\r\n" +
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
		
		string GetAddInXml()
		{
			return 
				"<AddIn name        = \"My AddIn\"\r\n" +
				"       author      = \"\"\r\n" +
				"       copyright   = \"prj:///doc/copyright.txt\"\r\n" +
				"       description = \"\"\r\n" +
				"       addInManagerHidden = \"preinstalled\">\r\n" +
				"\r\n" +
				"    <Manifest>\r\n" +
				"        <Identity name = \"ICSharpCode.MyAddIn\"/>\r\n" +
				"    </Manifest>\r\n" +
				"\r\n" +
				"    <Runtime>\r\n" +
				"        <Import assembly = \":ICSharpCode.SharpDevelop\"/>\r\n" +
				"        <Import assembly = \"UnLoadedAssembly.dll\"/>\r\n" +
				"        <Import assembly = \"MyAddIn.dll\"/>\r\n" +
				"    </Runtime>\r\n" +
				"</AddIn>";
		}
		
		[TearDown]
		public void TearDown()
		{
			predefinedManifestResourceStream.Dispose();
		}
		
		[Test]
		public void OpenStreamThrowsFileNotFoundExceptionForUnknownManifestResourceName()
		{
			string expectedMessage = "The resource file 'Unknown' was not found.";
			FileNotFoundException ex =
				Assert.Throws<FileNotFoundException>(delegate { highlightingResource.OpenStream("Unknown"); });
			Assert.AreEqual(expectedMessage, ex.Message);
		}
		
		[Test]
		public void OpenStreamReturnsAssemblyManifestResourceStreamForKnownManifestResourceName()
		{
			Stream stream = highlightingResource.OpenStream("ICSharpCode.Xml.xshd");
			Assert.AreSame(predefinedManifestResourceStream, stream);
		}
		
		[Test]
		public void MockAssemblyReturnedFromMyAddInRuntime()
		{
			Assert.IsInstanceOf(typeof(MockAssembly), testRuntime.LoadedAssembly);
		}
		
		[Test]
		public void UnloadedRunTimeReturnsNullFromLoadedAssembly()
		{
			Assert.IsNull(unloadedRuntime.LoadedAssembly);
		}
		
		[Test]
		public void HighlightingDefinitionCanBeLoadedFromXmlViaMemoryStream()
		{
			using (XmlTextReader reader = new XmlTextReader(predefinedManifestResourceStream)) {
				XshdSyntaxDefinition xshd = HighlightingLoader.LoadXshd(reader);
				Assert.AreEqual("BAT", xshd.Name);
			}
		}
		
		[Test]
		public void HighlightingDefinitionReturnedFromLoadHighlightingMethod()
		{
			HighlightingManager manager = new HighlightingManager();
			IHighlightingDefinition highlighting = highlightingResource.LoadHighlighting("ICSharpCode.Xml.xshd", manager);
			Assert.AreEqual("BAT", highlighting.Name);
		}
		
		[Test]
		public void LoadHighlightingThrowsArgumentNullExceptionIfResolverParameterIsNull()
		{
			ArgumentNullException ex = 
				Assert.Throws<ArgumentNullException>(
					delegate { highlightingResource.LoadHighlighting("ICSharpCode.Xml.xshd", null); });
			Assert.AreEqual("resolver", ex.ParamName);
		}
	}
}
