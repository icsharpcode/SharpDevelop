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
