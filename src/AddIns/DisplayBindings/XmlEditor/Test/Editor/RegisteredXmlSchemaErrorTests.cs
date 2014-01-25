// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
