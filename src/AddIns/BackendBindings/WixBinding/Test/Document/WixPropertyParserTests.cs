// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
