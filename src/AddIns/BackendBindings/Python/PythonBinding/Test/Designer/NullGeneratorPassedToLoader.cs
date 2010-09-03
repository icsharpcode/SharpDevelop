// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PythonBinding;
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
