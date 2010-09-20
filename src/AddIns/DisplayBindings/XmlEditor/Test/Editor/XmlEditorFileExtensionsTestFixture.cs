// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.XmlEditor;
using NUnit.Framework;

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
				AddIn addin = AddIn.Load(reader);
				
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
