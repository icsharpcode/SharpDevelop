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
	/// Tests that the string "System.Drawing.Color.FromArgb(0, 192, 10)" can be converted to an object by the 
	/// PythonCodeDeserializer.
	/// </summary>
	[TestFixture]
	public class DeserializeColorFromArgbTestFixture : DeserializeAssignmentTestFixtureBase
	{		
		public override string GetPythonCode()
		{
			return "self.BackColor = System.Drawing.Color.FromArgb(0, 192, 10)";
		}
		
		[Test]
		public void DeserializedObjectIsExpectedCustomColor()
		{
			Color customColor = Color.FromArgb(0, 192, 10);
			Assert.AreEqual(customColor, deserializedObject);
		}
		
		[Test]
		public void ColorTypeResolved()
		{
			Assert.AreEqual("System.Drawing.Color", componentCreator.LastTypeNameResolved);
		}
	}
}
