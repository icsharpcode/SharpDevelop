// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
