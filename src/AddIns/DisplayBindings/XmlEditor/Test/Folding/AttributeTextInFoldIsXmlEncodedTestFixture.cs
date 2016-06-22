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
