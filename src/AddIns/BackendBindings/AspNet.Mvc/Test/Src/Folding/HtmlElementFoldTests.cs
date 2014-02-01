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
