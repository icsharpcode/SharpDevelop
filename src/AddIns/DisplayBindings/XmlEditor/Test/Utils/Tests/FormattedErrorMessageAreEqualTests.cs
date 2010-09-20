// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Utils.Tests
{
	[TestFixture]
	public class FormattedErrorMessageAreEqualTests
	{
		[Test]
		public void SameMessageFormatAndParameterAreEqual()
		{
			FormattedErrorMessage lhs = new FormattedErrorMessage("message", "param");
			FormattedErrorMessage rhs = new FormattedErrorMessage("message", "param");
		
			Assert.AreEqual(lhs, rhs);
		}
		
		[Test]
		public void FormattedErrorMessageToStringShowsMessageAndParameter()
		{
			FormattedErrorMessage lhs = new FormattedErrorMessage("message", "param");
			Assert.AreEqual("Message \"message\" Parameter \"param\"", lhs.ToString());
		}
	}
}
