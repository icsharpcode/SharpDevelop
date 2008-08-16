// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.PythonBinding;
using ICSharpCode.TextEditor.Document;
using NUnit.Framework;

namespace PythonBinding.Tests.Designer
{
	/// <summary>
	/// Tests that the PythonDesignerLoader throws an exception
	/// when created with a null IDesignerGenerator.
	/// </summary>
	[TestFixture]
	public class NullGeneratorPassedToLoader
	{
		[Test]
		public void ThrowsException()
		{
			try {
				PythonDesignerLoader loader = new PythonDesignerLoader(null);
				Assert.Fail("Expected an argument exception before this line.");
			} catch (ArgumentException ex) {
				Assert.AreEqual("generator", ex.ParamName);
			}
		}		
	}	
}
