// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode.RubyBinding;
using IronRuby.Compiler.Ast;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Designer
{
	[TestFixture]
	public class DeserializeDateTimeArrayTestFixture : DeserializeAssignmentTestFixtureBase
	{		
		public override string GetRubyCode()
		{
			return "self.Items = System::Array[System::DateTime].new(\r\n" +
				"    [System::DateTime.new(2010, 2, 3, 0, 0, 0, 0),\r\n" + 
				"    System::DateTime.new(0)])";
		}
		
		[Test]
		public void DeserializedObjectIsExpectedArray()
		{
			DateTime[] expectedArray = new DateTime[] {new DateTime(2010, 2, 3), new DateTime(0)};
			Assert.AreEqual(expectedArray, deserializedObject);
		}
	}
}
