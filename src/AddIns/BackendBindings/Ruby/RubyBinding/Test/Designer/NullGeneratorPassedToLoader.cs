// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.RubyBinding;
using ICSharpCode.TextEditor.Document;
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
