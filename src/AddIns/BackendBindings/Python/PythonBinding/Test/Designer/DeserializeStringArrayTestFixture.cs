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
	/// <summary>
	/// Tests that the string "System.Array" can be converted to an array.
	/// </summary>
	[TestFixture]
	public class DeserializeStringArrayTestFixture : DeserializeAssignmentTestFixtureBase
	{		
		public override string GetPythonCode()
		{
			return "self.Items = System.Array[System.String](\r\n" +
				"    [\"a\",\r\n" + 
				"    \"b\"])";
		}
		
		[Test]
		public void DeserializedObjectIsExpectedArray()
		{
			string[] expectedArray = new string[] {"a", "b"};
			Assert.AreEqual(expectedArray, deserializedObject);
		}
		
		[Test]
		public void StringTypeResolved()
		{
			Assert.AreEqual("System.String", componentCreator.LastTypeNameResolved);
		}
	}
}
