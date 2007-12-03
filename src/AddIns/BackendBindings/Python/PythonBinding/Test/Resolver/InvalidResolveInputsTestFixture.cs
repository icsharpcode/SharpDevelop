// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Resolver
{
	/// <summary>
	/// Tests that the PythonResolver.Resolve method handles 
	/// invalid input parameters.
	/// </summary>
	[TestFixture]
	public class InvalidResolveInputsTestFixture
	{
		PythonResolver resolver;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			resolver = new PythonResolver();
		}
			
		[Test]
		public void NullFileContent()
		{
			ExpressionResult result = new ExpressionResult("test", new DomRegion(0, 0), null, null);
			Assert.IsNull(resolver.Resolve(result, new ParseInformation(), null));
		}
		
		[Test]
		public void EmptyFileContent()
		{
			ExpressionResult result = new ExpressionResult("test", new DomRegion(0, 0), null, null);
			Assert.IsNull(resolver.Resolve(result, new ParseInformation(), String.Empty));
		}
		
		[Test]
		public void NullParseInfo()
		{
			ExpressionResult result = new ExpressionResult("test", new DomRegion(0, 0), null, null);
			Assert.IsNull(resolver.Resolve(result, null, "test"));
		}
	}
}
