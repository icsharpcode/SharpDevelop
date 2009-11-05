// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Utils.Tests
{
	[TestFixture]
	public class ExceptionErrorMessageAreEqualTests
	{
		[Test]
		public void SameMessageAndExceptionAreEqual()
		{
			Exception ex = new Exception("ex");
			ExceptionErrorMessage lhs = new ExceptionErrorMessage(ex, "message");
			ExceptionErrorMessage rhs = new ExceptionErrorMessage(ex, "message");
		
			Assert.AreEqual(lhs, rhs);
		}
		
		[Test]
		public void FormattedErrorMessageToStringShowsMessageAndParameter()
		{
			Exception ex = new Exception("ex");
			ExceptionErrorMessage lhs = new ExceptionErrorMessage(ex, "message");
			Assert.AreEqual("Message \"message\" Exception \"ex\"", lhs.ToString());
		}
	}
}
