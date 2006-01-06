#region Copyright (c) 2002-2003, James W. Newkirk, Michael C. Two, Alexei A. Vorontsov, Charlie Poole, Philip A. Craig
/************************************************************************************
'
' Copyright © 2002-2003 James W. Newkirk, Michael C. Two, Alexei A. Vorontsov, Charlie Poole
' Copyright © 2000-2003 Philip A. Craig
'
' This software is provided 'as-is', without any express or implied warranty. In no 
' event will the authors be held liable for any damages arising from the use of this 
' software.
' 
' Permission is granted to anyone to use this software for any purpose, including 
' commercial applications, and to alter it and redistribute it freely, subject to the 
' following restrictions:
'
' 1. The origin of this software must not be misrepresented; you must not claim that 
' you wrote the original software. If you use this software in a product, an 
' acknowledgment (see the following) in the product documentation is required.
'
' Portions Copyright © 2003 James W. Newkirk, Michael C. Two, Alexei A. Vorontsov, Charlie Poole
' or Copyright © 2000-2003 Philip A. Craig
'
' 2. Altered source versions must be plainly marked as such, and must not be 
' misrepresented as being the original software.
'
' 3. This notice may not be removed or altered from any source distribution.
'
'***********************************************************************************/
#endregion

namespace NUnit.Framework
{
	using System;

	/// <summary>
	/// ExpectedExceptionAttribute
	/// </summary>
	/// 
	[AttributeUsage(AttributeTargets.Method, AllowMultiple=false)]
	public sealed class ExpectedExceptionAttribute : Attribute
	{
		private Type expectedException;
		private string expectedExceptionName;
		private string expectedMessage;

		/// <summary>
		/// Constructor for a given type of exception
		/// </summary>
		/// <param name="exceptionType">The type of the expected exception</param>
		public ExpectedExceptionAttribute(Type exceptionType)
		{
			this.expectedException = exceptionType;
			this.expectedExceptionName = exceptionType.FullName;
		}

		/// <summary>
		/// Constructor for a given exception name
		/// </summary>
		/// <param name="exceptionName">The full name of the expected exception</param>
		public ExpectedExceptionAttribute(string exceptionName)
		{
			this.expectedExceptionName = exceptionName;
		}

		/// <summary>
		/// Constructor for a given type of exception and expected message text
		/// </summary>
		/// <param name="exceptionType">The type of the expected exception</param>
		/// <param name="expectedMessage">The expected message text</param>
		public ExpectedExceptionAttribute(Type exceptionType, string expectedMessage)
		{
			this.expectedException = exceptionType;
			this.expectedMessage = expectedMessage;
		}

		/// <summary>
		/// Constructor for a given exception name and expected message text
		/// </summary>
		/// <param name="exceptionName">The full name of the expected exception</param>
		/// <param name="expectedMessage">The expected messge text</param>
		public ExpectedExceptionAttribute(string exceptionName, string expectedMessage)
		{
			this.expectedExceptionName = exceptionName;
			this.expectedMessage = expectedMessage;
		}

		/// <summary>
		/// The expected exception type
		/// </summary>
		public Type ExceptionType 
		{
			get{ return expectedException; }
			set{ expectedException = value; }
		}

		/// <summary>
		/// The full Type name of the expected exception
		/// </summary>
		public string ExceptionName
		{
			get{ return expectedExceptionName; }
			set{ expectedExceptionName = value; }
		}

		/// <summary>
		/// The expected message
		/// </summary>
		public string ExpectedMessage 
		{
			get { return expectedMessage; }
			set { expectedMessage = value; }
		}
	}
}
