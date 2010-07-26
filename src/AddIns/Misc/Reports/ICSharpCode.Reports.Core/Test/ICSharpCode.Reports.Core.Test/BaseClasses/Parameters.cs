/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 27.06.2009
 * Zeit: 19:49
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

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
