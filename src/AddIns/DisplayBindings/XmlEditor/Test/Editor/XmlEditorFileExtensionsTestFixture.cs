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

using ICSharpCode.Core;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using Rhino.Mocks;

namespace XmlEditor.Tests.Editor
{
	[TestFixture]
	public class XmlEditorFileExtensionsTestFixture
	{	
		DefaultXmlFileExtensions fileExtensions;
		
		[SetUp]
		public void SetUp()
		{
			string addinXml = "<AddIn name     = 'Xml Editor'\r\n" +
       								"author      = ''\r\n" +
       								"copyright   = 'prj:///doc/copyright.txt'\r\n" +
       								"description = ''\r\n" +
       								"addInManagerHidden = 'preinstalled'>\r\n" +
								"</AddIn>";

			using (StringReader reader = new StringReader(addinXml)) {
				var addInTree = MockRepository.GenerateStrictMock<IAddInTree>();
				AddIn addin = AddIn.Load(addInTree, reader);
				
				AddInTreeNode addinTreeNode = new AddInTreeNode();

				Properties properties = new Properties();
				properties.Set<string>("extensions", " .xml; .xsd ");
				properties.Set<string>("id", "Xml");
				
				addinTreeNode.AddCodons(
					new Codon[] {
						new Codon(addin, "CodeCompletionC#", new Properties(), new ICondition[0]),
						new Codon(addin, "CodeCompletionXml", properties, new ICondition[0])
					});
				
				fileExtensions = new DefaultXmlFileExtensions(addinTreeNode);
			}
		}
		
		[Test]
		public void FirstFileExtensionIsXml()
		{
			Assert.AreEqual(".xml", fileExtensions[0]);
		}
		
		[Test]
		public void SecondFileExtensionIsXsd()
		{
			Assert.AreEqual(".xsd", fileExtensions[1]);		
		}
	}
}
