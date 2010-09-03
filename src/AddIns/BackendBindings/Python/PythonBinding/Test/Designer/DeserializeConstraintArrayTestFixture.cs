// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode.PythonBinding;
using IronPython.Compiler.Ast;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	[TestFixture]
	public class DeserializeConstraintArrayTestFixture : DeserializeAssignmentTestFixtureBase
	{		
		public override string GetPythonCode()
		{
			return "self.Items = System.Array[System.Data.Constraint](\r\n" +
				"    [System.Data.UniqueConstraint(\"Constraint1\", System.Array[System.String](\r\n" +
				"        [\"Column1\"]), False)]))";
		}
		
		[Test]
		public void DeserializedObjectIsExpectedArray()
		{
			UniqueConstraint constraint = new UniqueConstraint("Constraint1", new string[] { "Column1" }, false);
			Constraint[] expectedArray = new Constraint[] { constraint };
			Assert.AreEqual(expectedArray, deserializedObject);
		}
	}
}
