// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.XmlEditor;
using NUnit.Framework;

namespace XmlEditor.Tests.Editor
{
	[TestFixture]
	public class RegisteredXmlSchemaErrorTests
	{
		[Test]
		public void ErrorMessageCanBeRetrieved()
		{
			RegisteredXmlSchemaError error = new RegisteredXmlSchemaError("message");
			Assert.AreEqual("message", error.Message);
		}
		
		[Test]
		public void ErrorToStringMatchesMessageProperty()
		{
			RegisteredXmlSchemaError error = new RegisteredXmlSchemaError("message");
			Assert.AreEqual("message", error.ToString());
		}
		
		[Test]
		public void ExceptionCanBeRetrieved()
		{
			ApplicationException ex = new ApplicationException();
			RegisteredXmlSchemaError error = new RegisteredXmlSchemaError("message", ex);
			Assert.AreSame(ex, error.Exception);
		}
		
		[Test]
		public void ErrorsWithSameErrorMessagesAreEqual()
		{
			RegisteredXmlSchemaError lhs = new RegisteredXmlSchemaError("message");
			RegisteredXmlSchemaError rhs = new RegisteredXmlSchemaError("message");
			Assert.IsTrue(lhs.Equals(rhs));
		}
		
		[Test]
		public void ErrorsWithDifferentMessagesAreNotEqual()
		{
			RegisteredXmlSchemaError lhs = new RegisteredXmlSchemaError("message");
			RegisteredXmlSchemaError rhs = new RegisteredXmlSchemaError("different-message");
			Assert.IsFalse(lhs.Equals(rhs));
		}
		
		[Test]
		public void ErrorsWithSameErrorMessageAndExceptionAreEqual()
		{
			ApplicationException ex = new ApplicationException();
			RegisteredXmlSchemaError lhs = new RegisteredXmlSchemaError("message", ex);
			RegisteredXmlSchemaError rhs = new RegisteredXmlSchemaError("message", ex);
			Assert.IsTrue(lhs.Equals(rhs));
		}
		
		[Test]
		public void ErrorsWithSameMessageButDifferentExceptionsAreNotEqual()
		{
			ApplicationException firstEx = new ApplicationException();
			ApplicationException secondEx = new ApplicationException();
			RegisteredXmlSchemaError lhs = new RegisteredXmlSchemaError("message", firstEx);
			RegisteredXmlSchemaError rhs = new RegisteredXmlSchemaError("message", secondEx);
			Assert.IsFalse(lhs.Equals(rhs));
		}
		
		[Test]
		public void HasExceptionReturnsTrueIfErrorHasException()
		{
			ApplicationException ex = new ApplicationException();
			RegisteredXmlSchemaError error = new RegisteredXmlSchemaError("message", ex);
			Assert.IsTrue(error.HasException);
		}
		
		[Test]
		public void HasExceptionReturnsFalseIfErrorHasNoException()
		{
			RegisteredXmlSchemaError error = new RegisteredXmlSchemaError("message");
			Assert.IsFalse(error.HasException);
		}	

		[Test]
		public void ExceptionMessageReturnedInToString()
		{
			ApplicationException ex = new ApplicationException("exception message");
			RegisteredXmlSchemaError error = new RegisteredXmlSchemaError("message", ex);
			Assert.AreEqual("message\r\nexception message", error.ToString());
		}		
	}
}
