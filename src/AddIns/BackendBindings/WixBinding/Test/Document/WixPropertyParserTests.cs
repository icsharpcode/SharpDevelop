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

using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace WixBinding.Tests.Document
{
	[TestFixture]
	public class WixPropertyParserTests : IWixPropertyValueProvider
	{
		NameValuePairCollection tags;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			tags = new NameValuePairCollection();
			tags.Add(new NameValuePair("Test", "Value"));
			tags.Add(new NameValuePair("DATADIR", "Bitmaps/bin"));
		}
		
		[Test]
		public void SingleProperty()
		{
			Assert.AreEqual("Value", WixPropertyParser.Parse("$(Test)", this));
		}
		
		[Test]
		public void EmptyString()
		{
			Assert.AreEqual(String.Empty, WixPropertyParser.Parse(String.Empty, this));
		}
		
		[Test]
		public void SingleWixPropertyVariable()
		{
			Assert.AreEqual("Bitmaps/bin", WixPropertyParser.Parse("$(var.DATADIR)", this));
		}
		
		[Test]
		public void SingleWixPropertyVariableWithTextFollowing()
		{
			Assert.AreEqual("Bitmaps/bin/dialog.bmp", WixPropertyParser.Parse("$(var.DATADIR)/dialog.bmp", this));
		}

		[Test]
		public void SingleWixPropertyVariableWithTextAtStart()
		{
			Assert.AreEqual("C:/Bitmaps/bin", WixPropertyParser.Parse("C:/$(var.DATADIR)", this));
		}		
		
		[Test]
		public void SingleWixPropertyVariableWithSingleCharFollowing()
		{
			Assert.AreEqual("Bitmaps/bin/", WixPropertyParser.Parse("$(var.DATADIR)/", this));
		}

		[Test]
		public void SingleWixPropertyVariableWithSingleCharAtStart()
		{
			Assert.AreEqual("/Bitmaps/bin", WixPropertyParser.Parse("/$(var.DATADIR)", this));
		}	
		
		[Test]
		public void TwoProperties()
		{
			Assert.AreEqual("ValueBitmaps/bin", WixPropertyParser.Parse("$(Test)$(var.DATADIR)", this));
		}
		
		[Test]
		public void TwoPropertiesSeparatedBySingleChar()
		{
			Assert.AreEqual("Value/Bitmaps/bin", WixPropertyParser.Parse("$(Test)/$(var.DATADIR)", this));
		}
		
		string IWixPropertyValueProvider.GetValue(string name)
		{
			return tags.GetValue(name);
		}
	}
}
