// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Folding
{
	[TestFixture]
	public class AttributeTextInFoldIsXmlEncodedTestFixture
	{
		XmlFoldParserHelper helper;
				
		[Test]
		public void GetFolds_AttributeValueHasSingleQuoteEncoded_FoldNameHasSingleQuoteEncoded()
		{
			string xml = 
				"<root a='Single &apos; Quote'>\r\n" +
				"</root>";
			
			GetFoldsWhenShowAttributesIsSetToTrue(xml);
			string name = helper.GetFirstFoldName();
			
			Assert.AreEqual("<root a='Single &apos; Quote'>", name);
		}
		
		void GetFoldsWhenShowAttributesIsSetToTrue(string xml)
		{
			helper = new XmlFoldParserHelper();
			helper.Options.ShowAttributesWhenFolded = true;
			
			helper.CreateParser();
			helper.GetFolds(xml);
		}

		[Test]
		public void GetFolds_AttributeTextHasDoubleQuoteEncoded_FoldNameHasDoubleQuoteEncoded()
		{
			string xml = 
				"<root a=\"Double &quot; Quote\">\r\n" +
				"</root>";
			
			GetFoldsWhenShowAttributesIsSetToTrue(xml);
			string name = helper.GetFirstFoldName();
			
			Assert.AreEqual("<root a=\"Double &quot; Quote\">", name);
		}
		
		[Test]
		public void GetFolds_AttributeTextHasAmpersandEncoded_FoldNameHasAmpersandEncoded()
		{
			string xml = 
				"<root a='Ampersand &amp;'>\r\n" +
				"</root>";
			
			GetFoldsWhenShowAttributesIsSetToTrue(xml);
			string name = helper.GetFirstFoldName();
			
			Assert.AreEqual("<root a='Ampersand &amp;'>", name);
		}
		
		[Test]
		public void GetFolds_AttributeTextHasLessThanTagEncoded_FoldNameHasLessThanTagEncoded()
		{
			string xml = 
				"<root a='&lt;'>\r\n" +
				"</root>";
			
			GetFoldsWhenShowAttributesIsSetToTrue(xml);
			string name = helper.GetFirstFoldName();
			
			Assert.AreEqual("<root a='&lt;'>", name);
		}
		
		[Test]
		public void GetFolds_AttributeTextHasGreaterThanTagEncoded_FoldNameHasGreaterThanTagEncoded()
		{
			string xml = 
				"<root a='&gt;'>\r\n" +
				"</root>";
			
			GetFoldsWhenShowAttributesIsSetToTrue(xml);
			string name = helper.GetFirstFoldName();
			
			Assert.AreEqual("<root a='&gt;'>", name);
		}
	}
}
