// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode.PythonBinding;
using IronPython.Compiler.Ast;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	[TestFixture]
	public class DeserializeDateTimeArrayTestFixture : DeserializeAssignmentTestFixtureBase
	{		
		public override string GetPythonCode()
		{
			return "self.Items = System.Array[System.DateTime](\r\n" +
				"    [System.DateTime(2010, 2, 3, 0, 0, 0, 0),\r\n" + 
				"    System.DateTime(0)])";
		}
		
		[Test]
		public void DeserializedObjectIsExpectedArray()
		{
			DateTime[] expectedArray = new DateTime[] {new DateTime(2010, 2, 3), new DateTime(0)};
			Assert.AreEqual(expectedArray, deserializedObject);
		}
	}
}
