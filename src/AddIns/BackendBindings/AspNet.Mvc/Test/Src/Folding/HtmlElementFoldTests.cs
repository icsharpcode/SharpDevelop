// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.AspNet.Mvc.Folding;
using NUnit.Framework;

namespace AspNet.Mvc.Tests.Folding
{
	[TestFixture]
	public class HtmlElementFoldTests
	{
		HtmlElementFold fold;
		HtmlElementFold lhs;
		HtmlElementFold rhs;
		
		void CreateHtmlElementFoldWithElementName(string elementName)
		{
			fold = new HtmlElementFold() { ElementName = elementName };
		}
		
		void CreateHtmlElementFoldsForComparison()
		{
			lhs = new HtmlElementFold();
			rhs = new HtmlElementFold();
		}
		
		[Test]
		public void Name_HtmlFoldCreatedUsingElementName_ReturnsElementNameInsideTags()
		{
			CreateHtmlElementFoldWithElementName("p");
			
			Assert.AreEqual("<p>", fold.Name);
		}
		
		[Test]
		public void Equals_AllPropertiesAreTheSame_ReturnsTrue()
		{
			CreateHtmlElementFoldsForComparison();
			
			bool result = lhs.Equals(rhs);
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void Equals_NullCompared_ReturnsFalse()
		{
			CreateHtmlElementFoldsForComparison();
			
			bool result = lhs.Equals(null);
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void Equals_DifferentElementNames_ReturnsFalse()
		{
			CreateHtmlElementFoldsForComparison();
			lhs.ElementName = "Test1";
			rhs.ElementName = "Test";
			
			bool result	= lhs.Equals(rhs);
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void Equals_DifferentStartOffsets_ReturnsFalse()
		{
			CreateHtmlElementFoldsForComparison();
			lhs.StartOffset = 0;
			rhs.StartOffset = 100;
			
			bool result = lhs.Equals(rhs);
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void Equals_DifferentEndOffsets_ReturnsFalse()
		{
			CreateHtmlElementFoldsForComparison();
			lhs.EndOffset = 0;
			rhs.EndOffset = 100;
			
			bool result = lhs.Equals(rhs);
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void ToString_ElementNameAndOffsetsSet_ReturnsFoldNameAndOffsets()
		{
			CreateHtmlElementFoldWithElementName("foo");
			fold.StartOffset = 1;
			fold.EndOffset = 10;
			
			string text = fold.ToString();
			
			string expectedText = "[HtmlElementFold Name='<foo>', StartOffset=1, EndOffset=10]";
			Assert.AreEqual(expectedText, text);
		}
	}
}
