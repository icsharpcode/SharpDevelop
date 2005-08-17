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

using System;
using System.Collections;
using System.ComponentModel;

namespace NUnit.Framework
{
	/// <summary>
	/// A set of Assert methods
	/// </summary>
	public class Assert
	{
		#region Assert Counting

		private static int counter = 0;
		
		/// <summary>
		/// Gets the number of assertions executed so far and 
		/// resets the counter to zero.
		/// </summary>
		public static int Counter
		{
			get
			{
				int cnt = counter;
				counter = 0;
				return cnt;
			}
		}

		private static void IncrementAssertCount()
		{
			++counter;
		}

		#endregion

		#region Constructor

		/// <summary>
		/// We don't actually want any instances of this object, but some people
		/// like to inherit from it to add other static methods. Hence, the
		/// protected constructor disallows any instances of this object. 
		/// </summary>
		protected Assert() {}

		#endregion

		#region Equals and ReferenceEquals

		/// <summary>
		/// The Equals method throws an AssertionException. This is done 
		/// to make sure there is no mistake by calling this function.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static new bool Equals(object a, object b)
		{
			throw new AssertionException("Assert.Equals should not be used for Assertions");
		}

		/// <summary>
		/// override the default ReferenceEquals to throw an AssertionException. This 
		/// implementation makes sure there is no mistake in calling this function 
		/// as part of Assert. 
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		public static new void ReferenceEquals(object a, object b)
		{
			throw new AssertionException("Assert.ReferenceEquals should not be used for Assertions");
		}

		#endregion
				
		#region IsTrue

		/// <summary>
		/// Asserts that a condition is true. If the condition is false the method throws
		/// an <see cref="AssertionException"/>.
		/// </summary> 
		/// <param name="condition">The evaluated condition</param>
		/// <param name="message">The message to display if the condition is false</param>
		/// <param name="args">Arguments to be used in formatting the message</param>
		static public void IsTrue(bool condition, string message, params object[] args) 
		{
			DoAssert( new TrueAsserter( condition, message, args ) );
		}
    
		/// <summary>
		/// Asserts that a condition is true. If the condition is false the method throws
		/// an <see cref="AssertionException"/>.
		/// </summary>
		/// <param name="condition">The evaluated condition</param>
		/// <param name="message">The message to display if the condition is false</param>
		static public void IsTrue(bool condition, string message) 
		{
			Assert.IsTrue(condition, message, null);
		}

		/// <summary>
		/// Asserts that a condition is true. If the condition is false the method throws
		/// an <see cref="AssertionException"/>.
		/// </summary>
		/// <param name="condition">The evaluated condition</param>
		static public void IsTrue(bool condition) 
		{
			Assert.IsTrue(condition, string.Empty, null);
		}

		#endregion

		#region IsFalse

		/// <summary>
		/// Asserts that a condition is false. If the condition is true the method throws
		/// an <see cref="AssertionException"/>.
		/// </summary>
		/// <param name="condition">The evaluated condition</param>
		/// <param name="message">The message to display if the condition is true</param>
		/// <param name="args">Arguments to be used in formatting the message</param>
		static public void IsFalse(bool condition, string message, params object[] args) 
		{
			DoAssert( new FalseAsserter( condition, message, args ) );
		}
		
		/// <summary>
		/// Asserts that a condition is false. If the condition is true the method throws
		/// an <see cref="AssertionException"/>.
		/// </summary>
		/// <param name="condition">The evaluated condition</param>
		/// <param name="message">The message to display if the condition is true</param>
		static public void IsFalse(bool condition, string message) 
		{
			Assert.IsFalse( condition, message, null );
		}
		
		/// <summary>
		/// Asserts that a condition is false. If the condition is true the method throws
		/// an <see cref="AssertionException"/>.
		/// </summary>
		/// <param name="condition">The evaluated condition</param>
		static public void IsFalse(bool condition) 
		{
			Assert.IsFalse(condition, string.Empty, null);
		}

		#endregion

		#region IsNotNull

		/// <summary>
		/// Verifies that the object that is passed in is not equal to <code>null</code>
		/// If the object is not <code>null</code> then an <see cref="AssertionException"/>
		/// is thrown.
		/// </summary>
		/// <param name="anObject">The object that is to be tested</param>
		/// <param name="message">The message to be printed when the object is null</param>
		/// <param name="args">Arguments to be used in formatting the message</param>
		static public void IsNotNull(Object anObject, string message, params object[] args) 
		{
			DoAssert( new NotNullAsserter( anObject, message, args ) );
		}

		/// <summary>
		/// Verifies that the object that is passed in is not equal to <code>null</code>
		/// If the object is not <code>null</code> then an <see cref="AssertionException"/>
		/// is thrown.
		/// </summary>
		/// <param name="anObject">The object that is to be tested</param>
		/// <param name="message"></param>
		static public void IsNotNull(Object anObject, string message) 
		{
			Assert.IsNotNull(anObject, message, null);
		}
    
		/// <summary>
		/// Verifies that the object that is passed in is not equal to <code>null</code>
		/// If the object is not <code>null</code> then an <see cref="AssertionException"/>
		/// is thrown.
		/// </summary>
		/// <param name="anObject">The object that is to be tested</param>
		static public void IsNotNull(Object anObject) 
		{
			Assert.IsNotNull(anObject, string.Empty, null);
		}
    
		#endregion
		    
		#region IsNull

		/// <summary>
		/// Verifies that the object that is passed in is equal to <code>null</code>
		/// If the object is <code>null</code> then an <see cref="AssertionException"/>
		/// is thrown.
		/// </summary>
		/// <param name="anObject">The object that is to be tested</param>
		/// <param name="message">The message to be printed when the object is not null</param>
		/// <param name="args">Arguments to be used in formatting the message</param>
		static public void IsNull(Object anObject, string message, params object[] args) 
		{
			DoAssert( new NullAsserter( anObject, message, args ) );
		}

		/// <summary>
		/// Verifies that the object that is passed in is equal to <code>null</code>
		/// If the object is <code>null</code> then an <see cref="AssertionException"/>
		/// is thrown.
		/// </summary>
		/// <param name="anObject">The object that is to be tested</param>
		/// <param name="message"></param>
		static public void IsNull(Object anObject, string message) 
		{
			Assert.IsNull(anObject, message, null);
		}
    
		/// <summary>
		/// Verifies that the object that is passed in is equal to <code>null</code>
		/// If the object is <code>null</code> then an <see cref="AssertionException"/>
		/// is thrown.
		/// </summary>
		/// <param name="anObject">The object that is to be tested</param>
		static public void IsNull(Object anObject) 
		{
			Assert.IsNull(anObject, string.Empty, null);
		}
    
		#endregion

		#region AreEqual

		#region Doubles

		/// <summary>
		/// Verifies that two doubles are equal considering a delta. If the
		/// expected value is infinity then the delta value is ignored. If 
		/// they are not equals then an <see cref="AssertionException"/> is
		/// thrown.
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		/// <param name="delta">The maximum acceptable difference between the
		/// the expected and the actual</param>
		/// <param name="message">The message that will be printed on failure</param>
		/// <param name="args">Arguments to be used in formatting the message</param>
		static public void AreEqual(double expected, 
			double actual, double delta, string message, params object[] args) 
		{
			DoAssert( new EqualAsserter( expected, actual, delta, message, args ) );
		}

		/// <summary>
		/// Verifies that two doubles are equal considering a delta. If the
		/// expected value is infinity then the delta value is ignored. If 
		/// they are not equals then an <see cref="AssertionException"/> is
		/// thrown.
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		/// <param name="delta">The maximum acceptable difference between the
		/// the expected and the actual</param>
		/// <param name="message">The message that will be printed on failure</param>
		static public void AreEqual(double expected, 
			double actual, double delta, string message) 
		{
			Assert.AreEqual( expected, actual, delta, message, null );
		}

		/// <summary>
		/// Verifies that two doubles are equal considering a delta. If the
		/// expected value is infinity then the delta value is ignored. If 
		/// they are not equals then an <see cref="AssertionException"/> is
		/// thrown.
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		/// <param name="delta">The maximum acceptable difference between the
		/// the expected and the actual</param>
		static public void AreEqual(double expected, double actual, double delta) 
		{
			Assert.AreEqual(expected, actual, delta, string.Empty, null);
		}

		#endregion

		#region Floats

		/// <summary>
		/// Verifies that two floats are equal considering a delta. If the
		/// expected value is infinity then the delta value is ignored. If 
		/// they are not equals then an <see cref="AssertionException"/> is
		/// thrown.
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		/// <param name="delta">The maximum acceptable difference between the
		/// the expected and the actual</param>
		/// <param name="message">The message printed out upon failure</param>
		/// <param name="args">Arguments to be used in formatting the message</param>
		static public void AreEqual(float expected, 
			float actual, float delta, string message, params object[] args) 
		{
			DoAssert( new EqualAsserter( expected, actual, delta, message, args ) );
		}

		/// <summary>
		/// Verifies that two floats are equal considering a delta. If the
		/// expected value is infinity then the delta value is ignored. If 
		/// they are not equals then an <see cref="AssertionException"/> is
		/// thrown.
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		/// <param name="delta">The maximum acceptable difference between the
		/// the expected and the actual</param>
		/// <param name="message">The message printed out upon failure</param>
		static public void AreEqual(float expected, float actual, float delta, string message) 
		{
			Assert.AreEqual(expected, actual, delta, message, null);
		}

		/// <summary>
		/// Verifies that two floats are equal considering a delta. If the
		/// expected value is infinity then the delta value is ignored. If 
		/// they are not equals then an <see cref="AssertionException"/> is
		/// thrown.
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		/// <param name="delta">The maximum acceptable difference between the
		/// the expected and the actual</param>
		static public void AreEqual(float expected, float actual, float delta) 
		{
			Assert.AreEqual(expected, actual, delta, string.Empty, null);
		}

		#endregion

		#region Objects
		
		/// <summary>
		/// Verifies that two objects are equal.  Two objects are considered
		/// equal if both are null, or if both have the same value.  All
		/// non-numeric types are compared by using the <c>Equals</c> method.
		/// Arrays are compared by comparing each element using the same rules.
		/// If they are not equal an <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The value that is expected</param>
		/// <param name="actual">The actual value</param>
		/// <param name="message">The message to display if objects are not equal</param>
		/// <param name="args">Arguments to be used in formatting the message</param>
		static public void AreEqual(Object expected, Object actual, string message, params object[] args)
		{
			DoAssert( new EqualAsserter(expected, actual, message, args) );
		}

		/// <summary>
		/// Verifies that two objects are equal.  Two objects are considered
		/// equal if both are null, or if both have the same value.  All
		/// non-numeric types are compared by using the <c>Equals</c> method.
		/// If they are not equal an <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The value that is expected</param>
		/// <param name="actual">The actual value</param>
		/// <param name="message">The message to display if objects are not equal</param>
		static public void AreEqual(Object expected, Object actual, string message) 
		{
			Assert.AreEqual(expected, actual, message, null);
		}

		/// <summary>
		/// Verifies that two objects are equal.  Two objects are considered
		/// equal if both are null, or if both have the same value.  All
		/// non-numeric types are compared by using the <c>Equals</c> method.
		/// If they are not equal an <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The value that is expected</param>
		/// <param name="actual">The actual value</param>
		static public void AreEqual(Object expected, Object actual) 
		{
			Assert.AreEqual(expected, actual, string.Empty, null);
		}

		#endregion

		#endregion

		#region AreNotEqual

		/// <summary>
		/// Asserts that two objects are not equal. If they are equal
		/// an <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The expected object</param>
		/// <param name="actual">The actual object</param>
		/// <param name="message">The message to be printed when the two objects are the same object.</param>
		/// <param name="args">Arguments to be used in formatting the message</param>
		static public void AreNotEqual( Object expected, Object actual, string message, params object[] args)
		{
			DoAssert( new NotEqualAsserter( expected, actual, message, args ) );
		}

		/// <summary>
		/// Asserts that two objects are not equal. If they are equal
		/// an <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The expected object</param>
		/// <param name="actual">The actual object</param>
		/// <param name="message">The message to be printed when the objects are the same</param>
		static public void AreNotEqual(Object expected, Object actual, string message) 
		{
			Assert.AreNotEqual(expected, actual, message, null);
		}
   
		/// <summary>
		/// Asserts that two objects are not equal. If they are equal
		/// an <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The expected object</param>
		/// <param name="actual">The actual object</param>
		static public void AreNotEqual(Object expected, Object actual) 
		{
			Assert.AreNotEqual(expected, actual, string.Empty, null);
		}
   
		#endregion

		#region AreSame

		/// <summary>
		/// Asserts that two objects refer to the same object. If they
		/// are not the same an <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The expected object</param>
		/// <param name="actual">The actual object</param>
		/// <param name="message">The message to be printed when the two objects are not the same object.</param>
		/// <param name="args">Arguments to be used in formatting the message</param>
		static public void AreSame(Object expected, Object actual, string message, params object[] args)
		{
			DoAssert( new SameAsserter( expected, actual, message, args ) );
		}

		/// <summary>
		/// Asserts that two objects refer to the same object. If they
		/// are not the same an <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The expected object</param>
		/// <param name="actual">The actual object</param>
		/// <param name="message">The message to be printed when the object is null</param>
		static public void AreSame(Object expected, Object actual, string message) 
		{
			Assert.AreSame(expected, actual, message, null);
		}
   
		/// <summary>
		/// Asserts that two objects refer to the same object. If they
		/// are not the same an <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The expected object</param>
		/// <param name="actual">The actual object</param>
		static public void AreSame(Object expected, Object actual) 
		{
			Assert.AreSame(expected, actual, string.Empty, null);
		}
   
		#endregion

		#region AreNotSame

		/// <summary>
		/// Asserts that two objects do not refer to the same object. If they
		/// are the same an <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The expected object</param>
		/// <param name="actual">The actual object</param>
		/// <param name="message">The message to be printed when the two objects are the same object.</param>
		/// <param name="args">Arguments to be used in formatting the message</param>
		static public void AreNotSame(Object expected, Object actual, string message, params object[] args)
		{
			DoAssert( new NotSameAsserter( expected, actual, message, args ) );
		}

		/// <summary>
		/// Asserts that two objects do not refer to the same object. If they
		/// are the same an <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The expected object</param>
		/// <param name="actual">The actual object</param>
		/// <param name="message">The message to be printed when the objects are the same</param>
		static public void AreNotSame(Object expected, Object actual, string message) 
		{
			Assert.AreNotSame(expected, actual, message, null);
		}
   
		/// <summary>
		/// Asserts that two objects do not refer to the same object. If they
		/// are the same an <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The expected object</param>
		/// <param name="actual">The actual object</param>
		static public void AreNotSame(Object expected, Object actual) 
		{
			Assert.AreNotSame(expected, actual, string.Empty, null);
		}
   
		#endregion

		#region Fail

		/// <summary>
		/// Throws an <see cref="AssertionException"/> with the message and arguments 
		/// that are passed in. This is used by the other Assert functions. 
		/// </summary>
		/// <param name="message">The message to initialize the <see cref="AssertionException"/> with.</param>
		/// <param name="args">Arguments to be used in formatting the message</param>
		static public void Fail(string message, params object[] args ) 
		{
			if (message == null) message = string.Empty;
			else if ( args != null && args.Length > 0 )
				message = string.Format( message, args );

			throw new AssertionException(message);
		}

		/// <summary>
		/// Throws an <see cref="AssertionException"/> with the message that is 
		/// passed in. This is used by the other Assert functions. 
		/// </summary>
		/// <param name="message">The message to initialize the <see cref="AssertionException"/> with.</param>
		static public void Fail(string message) 
		{
			Assert.Fail(message, null);
		}
    
		/// <summary>
		/// Throws an <see cref="AssertionException"/>. 
		/// This is used by the other Assert functions. 
		/// </summary>
		static public void Fail() 
		{
			Assert.Fail(string.Empty, null);
		}

		#endregion 

		#region Ignore

		/// <summary>
		/// Throws an <see cref="IgnoreException"/> with the message and arguments 
		/// that are passed in.  This causes the test to be reported as ignored.
		/// </summary>
		/// <param name="message">The message to initialize the <see cref="AssertionException"/> with.</param>
		/// <param name="args">Arguments to be used in formatting the message</param>
		static public void Ignore( string message, object[] args )
		{
			if (message == null) message = string.Empty;
			else if ( args != null && args.Length > 0 )
				message = string.Format( message, args );

			throw new IgnoreException(message);
		}

		/// <summary>
		/// Throws an <see cref="IgnoreException"/> with the message that is 
		/// passed in. This causes the test to be reported as ignored. 
		/// </summary>
		/// <param name="message">The message to initialize the <see cref="AssertionException"/> with.</param>
		static public void Ignore( string message )
		{
			Assert.Ignore( message, null );
		}
    
		/// <summary>
		/// Throws an <see cref="IgnoreException"/>. 
		/// This causes the test to be reported as ignored. 
		/// </summary>
		static public void Ignore()
		{
			Assert.Ignore( string.Empty, null );
		}
    
		#endregion

		#region DoAssert

		static public void DoAssert( IAsserter asserter )
		{
			Assert.IncrementAssertCount();
			asserter.Assert();
		}

		#endregion
	}
}