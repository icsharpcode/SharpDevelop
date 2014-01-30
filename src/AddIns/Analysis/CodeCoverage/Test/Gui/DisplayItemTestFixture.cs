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

using ICSharpCode.CodeCoverage;
using NUnit.Framework;
using System;
using System.Drawing;

namespace ICSharpCode.CodeCoverage.Tests.Gui
{
	[TestFixture]
	public class DisplayItemTestFixture
	{
		CodeCoverageDisplayItem displayItem;
		string itemName = "Code Covered";
		string backColorPropertyName = "BackColor";
		Color backColor = Color.Lime;
		string foreColorPropertyName = "ForeColor";
		Color foreColor = Color.Blue;
		
		[SetUp]
		public void Init()
		{
			displayItem = new CodeCoverageDisplayItem(itemName, backColorPropertyName, backColor, foreColorPropertyName, foreColor);
		}
		
		[Test]
		public void DisplayItemToString()
		{
			Assert.AreEqual(itemName, displayItem.ToString());
		}
		
		[Test]
		public void HasChanged()
		{
			Assert.IsFalse(displayItem.HasChanged);
		}
		
		[Test]
		public void BackColor()
		{
			Assert.AreEqual(backColor, displayItem.BackColor);
		}
		
		[Test]
		public void BackColorPropertyName()
		{
			Assert.AreEqual(backColorPropertyName, displayItem.BackColorPropertyName);
		}
		
		[Test]
		public void ForeColor()
		{
			Assert.AreEqual(foreColor, displayItem.ForeColor);
		}
		
		[Test]
		public void ForeColorPropertyName()
		{
			Assert.AreEqual(foreColorPropertyName, displayItem.ForeColorPropertyName);
		}
		
		[Test]
		public void ChangeBackColor()
		{
			displayItem.BackColor = Color.Red;
			Assert.IsTrue(displayItem.HasChanged);
		}
		
		[Test]
		public void ChangeForeColor()
		{
			displayItem.ForeColor = Color.Yellow;
			Assert.IsTrue(displayItem.HasChanged);
		}
	}
}
