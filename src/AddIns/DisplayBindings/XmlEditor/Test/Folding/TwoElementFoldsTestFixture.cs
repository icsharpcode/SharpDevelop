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
using ICSharpCode.NRefactory.TypeSystem;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Folding
{
	[TestFixture]
	public class TwoElementFoldsTestFixture
	{
		XmlFoldParserHelper helper;
		
		[SetUp]
		public void Init()
		{
			string xml = 
				"<root>\r\n" +
				"    <child>\r\n" +
				"    </child>\r\n" +
				"</root>";
			
			helper = new XmlFoldParserHelper();
			helper.CreateParser();
			helper.GetFolds(xml);
		}
		
		[Test]
		public void GetFolds_TwoElements_FirstFoldRegionCoversRootElement()
		{
			DomRegion region = helper.GetFirstFoldRegion();
			
			int beginLine = 1;
			int endLine = 4;
			int beginCol = 1;
			int endCol = 8;
			DomRegion expectedRegion = new DomRegion(beginLine, beginCol, endLine, endCol);
			
			Assert.AreEqual(expectedRegion, region);
		}
		
		[Test]
		public void GetFolds_TwoElements_SecondFoldRegionCoversChildElement()
		{
			DomRegion region = helper.GetSecondFoldRegion();
			
			int beginLine = 2;
			int endLine = 3;
			int beginCol = 5;
			int endCol = 13;
			DomRegion expectedRegion = new DomRegion(beginLine, beginCol, endLine, endCol);
			
			Assert.AreEqual(expectedRegion, region);
		}
	}
}
