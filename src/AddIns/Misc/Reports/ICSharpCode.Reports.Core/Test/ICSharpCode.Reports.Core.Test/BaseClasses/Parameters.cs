// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NUnit.Framework;
using ICSharpCode.Reports.Core;

namespace ICSharpCode.Reports.Core.Test.BaseClasses
{
	[TestFixture]
	public class ParametersFixture
	{
		[Test]
		public void CreateEmptyParameter()
		{
			BasicParameter b = new BasicParameter();
			Assert.IsNotNull(b);
		}
		
		
		[Test]
		public void ParameterWithnameAndValue()
		{
			BasicParameter b = new BasicParameter("par1","value1");
			Assert.AreEqual("par1",b.ParameterName);
			Assert.AreEqual("value1",b.ParameterValue);
		}
		
		
		[Test]
		public void CreateParameter_ValueShouldBeNull()
		{
			BasicParameter b = new BasicParameter();
			b.ParameterName = "name1";
			Assert.IsNotNull(b);
			Assert.AreEqual("name1",b.ParameterName);
			Assert.AreEqual(null,b.ParameterValue);
		}
		
		[Test]
		public void ConvertToString_ValueShouldBeStringEmpty()
		{
			BasicParameter b = new BasicParameter();
			b.ParameterName = "name1";
			Assert.IsNotNull(b);
			Assert.AreEqual("name1",b.ParameterName);
		}
	}
}
