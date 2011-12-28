// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.AspNet.Mvc.Folding;
using NUnit.Framework;

namespace AspNet.Mvc.Tests.Folding
{
	public class WebFormsHtmlReaderTests
	{
		SimpleWebFormsHtmlReader htmlReader;
		
		void CreateHtmlReader(string html)
		{
			htmlReader = new SimpleWebFormsHtmlReader(html);
		}
		
		[Test]
		public void Value_ReadSingleParagraphTag_ReturnsParagraphTagName()
		{
			CreateHtmlReader("<p>");
			htmlReader.Read();
			
			string value = htmlReader.Value;
			
			Assert.AreEqual("p", value);
		}
		
		[Test]
		public void Read_EmptyString_ReturnsFalse()
		{
			CreateHtmlReader(String.Empty);
			
			bool result = htmlReader.Read();
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void Read_ReadSingleParagraphTagHtml_ReturnsTrue()
		{
			CreateHtmlReader("<p>");
			
			bool result = htmlReader.Read();
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void Read_ReadTwiceFromSingleParagraphTag_SecondReadReturnsFalse()
		{
			CreateHtmlReader("<p>");
			
			bool result = htmlReader.Read();
			result = htmlReader.Read();
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void Offset_SingleParagraphTagAtStartOfString_ReturnsZero()
		{
			CreateHtmlReader("<p>");
			htmlReader.Read();
			
			int offset = htmlReader.Offset;
			
			Assert.AreEqual(0, offset);
		}
		
		[Test]
		public void Length_SingleParagraphTagAtStartOfString_ReturnsThree()
		{
			CreateHtmlReader("<p>");
			htmlReader.Read();
			
			int length = htmlReader.Length;
			
			Assert.AreEqual(3, length);
		}
		
		[Test]
		public void Offset_SingleParagraphTagOnSecondLine_IsAfterCarriageReturnAndLineFeedAndReturnsTwo()
		{
			CreateHtmlReader("\r\n<p>");
			htmlReader.Read();
			
			int offset = htmlReader.Offset;
			
			Assert.AreEqual(2, offset);
		}
		
		[Test]
		public void Length_SingleParagraphTagOnSecondLine_ReturnsThreeForParagraphNodeRead()
		{
			CreateHtmlReader("\r\n<p>");
			htmlReader.Read();
			
			int length = htmlReader.Length;
			
			Assert.AreEqual(3, length);
		}
		
		[Test]
		public void Value_ReadSingleLineBreakTag_ReturnsLineBreakTagName()
		{
			CreateHtmlReader("<br>");
			htmlReader.Read();
			
			string value = htmlReader.Value;
			
			Assert.AreEqual("br", value);
		}
		
		[Test]
		public void Length_ReadSingleLineBreakTag_ReturnsFour()
		{
			CreateHtmlReader("<br>");
			htmlReader.Read();
			
			int length = htmlReader.Length;
			
			Assert.AreEqual(4, length);
		}
		
		[Test]
		public void Value_ReadParagraphTagFollowedByBoldTag_ReturnsBoldTagName()
		{
			CreateHtmlReader("<p><b>");
			htmlReader.Read();
			htmlReader.Read();
			
			string value = htmlReader.Value;
			
			Assert.AreEqual("b", value);
		}
		
		[Test]
		public void Offset_ReadParagraphTagFollowedByBoldTag_ReturnsThree()
		{
			CreateHtmlReader("<p><b>");
			htmlReader.Read();
			htmlReader.Read();
			
			int offset = htmlReader.Offset;
			
			Assert.AreEqual(3, offset);
		}
		
		[Test]
		public void Value_ReadParagraphTagFollowedByEndParagraphTag_ReturnsParagraphTagName()
		{
			CreateHtmlReader("<p></p>");
			htmlReader.Read();
			htmlReader.Read();
			
			string value = htmlReader.Value;
			
			Assert.AreEqual("p", value);
		}
		
		[Test]
		public void IsEndElement_ReadParagraphTagFollowedByEndParagraphTag_ReturnsTrue()
		{
			CreateHtmlReader("<p></p>");
			htmlReader.Read();
			htmlReader.Read();
			
			bool result = htmlReader.IsEndElement;
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void IsEmptyElement_ReadEmptyBreakTagElement_ReturnsTrue()
		{
			CreateHtmlReader("<br/>");
			htmlReader.Read();
			
			bool result = htmlReader.IsEmptyElement;
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void IsEmptyElement_ReadParagraphTagFollowedByEndParagraphTag_ReturnsFalse()
		{
			CreateHtmlReader("<p></p>");
			htmlReader.Read();
			htmlReader.Read();
			
			bool result = htmlReader.IsEmptyElement;
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void Length_ReadParagraphTagFollowedByEndParagraphTag_ReturnsFour()
		{
			CreateHtmlReader("<p></p>");
			htmlReader.Read();
			htmlReader.Read();
			
			int length = htmlReader.Length;
			
			Assert.AreEqual(4, length);
		}
		
		[Test]
		public void IsStartElement_ReadSingleParagraphTag_ReturnTrue()
		{
			CreateHtmlReader("<p>");
			htmlReader.Read();
			
			bool result = htmlReader.IsStartElement;
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void IsEmptyElement_ReadEmptyBreakTagFollowedbySingleParagraphTag_ReturnFalse()
		{
			CreateHtmlReader("<br/><p>");
			htmlReader.Read();
			htmlReader.Read();
			
			bool result = htmlReader.IsEmptyElement;
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void IsStartElement_ReadEmptyBreakTag_ReturnsTrue()
		{
			CreateHtmlReader("<br/>");
			htmlReader.Read();
			
			bool result = htmlReader.IsStartElement;
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void IsStartElement_ReadParagraphTagFollowedByEndParagraphTag_ReturnsFalse()
		{
			CreateHtmlReader("<p></p>");
			htmlReader.Read();
			htmlReader.Read();
			
			bool result = htmlReader.IsStartElement;
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void Value_ReadParagraphTagsContainingText_ReturnsParagraphTagName()
		{
			CreateHtmlReader("<p>abcdef</p>");
			htmlReader.Read();
			htmlReader.Read();
			
			string value = htmlReader.Value;
			
			Assert.AreEqual("p", value);
		}
		
		[Test]
		public void Value_ReadDivStartTagWithClassAttribute_ReturnsDivTagName()
		{
			CreateHtmlReader("<div class='header'></div>");
			htmlReader.Read();
			
			string value = htmlReader.Value;
			
			Assert.AreEqual("div", value);
		}
		
		[Test]
		public void Value_ScriptStartTagWithForwardSlashInTypeAttribute_ReturnsScriptTagName()
		{
			CreateHtmlReader("<script type=\"text/javascript\">");
			htmlReader.Read();
			
			string value = htmlReader.Value;
			
			Assert.AreEqual("script", value);
		}
		
		[Test]
		public void IsStartElement_ScriptStartTagWithForwardSlashInTypeAttribute_ReturnsTrue()
		{
			CreateHtmlReader("<script type=\"text/javascript\">");
			htmlReader.Read();
			
			bool result = htmlReader.IsStartElement;
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void IsEmptyElement_ScriptStartTagWithForwardSlashInTypeAttributeWithDoubleQuotes_ReturnsFalse()
		{
			CreateHtmlReader("<script type=\"text/javascript\">");
			htmlReader.Read();
			
			bool result = htmlReader.IsEmptyElement;
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void IsEmptyElement_ScriptStartTagWithForwardSlashInAttributeWithSingleQuotes_ReturnsFalse()
		{
			CreateHtmlReader("<script type='text/javascript'>");
			htmlReader.Read();
			
			bool result = htmlReader.IsEmptyElement;
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void Value_ReadAspContentTagWithAspNamespacePrefix_ReturnsAspContentWithColonCharacter()
		{
			CreateHtmlReader("<asp:Content>");
			htmlReader.Read();
			
			string value = htmlReader.Value;
			
			Assert.AreEqual("asp:Content", value);
		}
	}
}
