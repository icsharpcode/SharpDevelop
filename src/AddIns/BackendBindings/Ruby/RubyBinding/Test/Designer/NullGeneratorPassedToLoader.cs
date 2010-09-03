// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.RubyBinding;
using NUnit.Framework;

namespace RubyBinding.Tests.Designer
{
	/// <summary>
	/// Tests that the RubyDesignerLoader throws an exception
	/// when created with a null IDesignerGenerator.
	/// </summary>
	[TestFixture]
	public class NullGeneratorPassedToLoader
	{
		[Test]
		public void ThrowsException()
		{
			try {
				RubyDesignerLoader loader = new RubyDesignerLoader(null);
				Assert.Fail("Expected an argument exception before this line.");
			} catch (ArgumentException ex) {
				Assert.AreEqual("generator", ex.ParamName);
			}
		}		
	}	
}
