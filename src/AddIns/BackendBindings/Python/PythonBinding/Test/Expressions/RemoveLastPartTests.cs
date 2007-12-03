// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Expressions
{
	/// <summary>
	/// Tests the PythonExpressionFinder's RemoveLastPart method.
	/// </summary>
	[TestFixture]
	public class RemoveLastPartTests
	{
		PythonExpressionFinder expressionFinder;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			expressionFinder = new PythonExpressionFinder();
		}
		
		[Test]
		public void NullString()
		{
			Assert.AreEqual(String.Empty, expressionFinder.RemoveLastPart(null));
		}
		
		[Test]
		public void EmptyString()
		{
			Assert.AreEqual(String.Empty, expressionFinder.RemoveLastPart(String.Empty));
		}
		
		/// <summary>
		/// Should remove the WriteLine part of the string.
		/// </summary>
		[Test]
		public void SystemConsoleWriteLineString()
		{
			string expression = "System.Console.WriteLine";
			Assert.AreEqual("System.Console", expressionFinder.RemoveLastPart(expression));
		}
	}
}
