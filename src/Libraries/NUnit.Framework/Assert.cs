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
		/// If the object is <code>null</code> then an <see cref="AssertionException"/>
		/// is thrown.
		/// </summary>
		/// <param name="anObject">The object that is to be tested</param>
		/// <param name="message">The message to be displayed when the object is null</param>
		/// <param name="args">Arguments to be used in formatting the message</param>
		static public void IsNotNull(Object anObject, string message, params object[] args) 
		{
			DoAssert( new NotNullAsserter( anObject, message, args ) );
		}

		/// <summary>
		/// Verifies that the object that is passed in is not equal to <code>null</code>
		/// If the object is <code>null</code> then an <see cref="AssertionException"/>
		/// is thrown.
		/// </summary>
		/// <param name="anObject">The object that is to be tested</param>
		/// <param name="message">The message to be displayed when the object is null</param>
		static public void IsNotNull(Object anObject, string message) 
		{
			Assert.IsNotNull(anObject, message, null);
		}
    
		/// <summary>
		/// Verifies that the object that is passed in is not equal to <code>null</code>
		/// If the object is <code>null</code> then an <see cref="AssertionException"/>
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
		/// If the object is not <code>null</code> then an <see cref="AssertionException"/>
		/// is thrown.
		/// </summary>
		/// <param name="anObject">The object that is to be tested</param>
		/// <param name="message">The message to be displayed when the object is not null</param>
		/// <param name="args">Arguments to be used in formatting the message</param>
		static public void IsNull(Object anObject, string message, params object[] args) 
		{
			DoAssert( new NullAsserter( anObject, message, args ) );
		}

		/// <summary>
		/// Verifies that the object that is passed in is equal to <code>null</code>
		/// If the object is not <code>null</code> then an <see cref="AssertionException"/>
		/// is thrown.
		/// </summary>
		/// <param name="anObject">The object that is to be tested</param>
		/// <param name="message">The message to be displayed when the object is not null</param>
		static public void IsNull(Object anObject, string message) 
		{
			Assert.IsNull(anObject, message, null);
		}
    
		/// <summary>
		/// Verifies that the object that is passed in is equal to <code>null</code>
		/// If the object is not null <code>null</code> then an <see cref="AssertionException"/>
		/// is thrown.
		/// </summary>
		/// <param name="anObject">The object that is to be tested</param>
		static public void IsNull(Object anObject) 
		{
			Assert.IsNull(anObject, string.Empty, null);
		}
    
		#endregion

		#region IsNaN

		/// <summary>
		/// Verifies that the double is passed is an <code>NaN</code> value.
		/// If the object is not <code>NaN</code> then an <see cref="AssertionException"/>
		/// is thrown.
		/// </summary>
		/// <param name="aDouble">The value that is to be tested</param>
		/// <param name="message">The message to be displayed when the object is not null</param>
		/// <param name="args">Arguments to be used in formatting the message</param>
		static public void IsNaN(double aDouble, string message, params object[] args) 
		{
			DoAssert( new NaNAsserter( aDouble, message, args ) );
		}

		/// <summary>
		/// Verifies that the double is passed is an <code>NaN</code> value.
		/// If the object is not <code>NaN</code> then an <see cref="AssertionException"/>
		/// is thrown.
		/// </summary>
		/// <param name="aDouble">The object that is to be tested</param>
		/// <param name="message">The message to be displayed when the object is not null</param>
		static public void IsNaN(double aDouble, string message) 
		{
			Assert.IsNaN(aDouble, message, null);
		}
    
		/// <summary>
		/// Verifies that the double is passed is an <code>NaN</code> value.
		/// If the object is not <code>NaN</code> then an <see cref="AssertionException"/>
		/// is thrown.
		/// </summary>
		/// <param name="aDouble">The object that is to be tested</param>
		static public void IsNaN(double aDouble) 
		{
			Assert.IsNaN(aDouble, string.Empty, null);
		}
    
		#endregion

		#region IsEmpty

		/// <summary>
		/// Assert that a string is empty - that is equal to string.Emtpy
		/// </summary>
		/// <param name="aString">The string to be tested</param>
		/// <param name="message">The message to be displayed on failure</param>
		/// <param name="args">Arguments to be used in formatting the message</param>
		public static void IsEmpty( string aString, string message, params object[] args )
		{
			DoAssert( new EmptyAsserter( aString, message, args ) );
		}

		/// <summary>
		/// Assert that a string is empty - that is equal to string.Emtpy
		/// </summary>
		/// <param name="aString">The string to be tested</param>
		/// <param name="message">The message to be displayed on failure</param>
		public static void IsEmpty( string aString, string message )
		{
			IsEmpty( aString, message, null );
		}

		/// <summary>
		/// Assert that a string is empty - that is equal to string.Emtpy
		/// </summary>
		/// <param name="aString">The string to be tested</param>
		public static void IsEmpty( string aString )
		{
			IsEmpty( aString, string.Empty, null );
		}

		/// <summary>
		/// Assert that an array, list or other collection is empty
		/// </summary>
		/// <param name="collection">An array, list or other collection implementing ICollection</param>
		/// <param name="message">The message to be displayed on failure</param>
		/// <param name="args">Arguments to be used in formatting the message</param>
		public static void IsEmpty( ICollection collection, string message, params object[] args )
		{
			DoAssert( new EmptyAsserter( collection, message, args ) );
		}

		/// <summary>
		/// Assert that an array, list or other collection is empty
		/// </summary>
		/// <param name="collection">An array, list or other collection implementing ICollection</param>
		/// <param name="message">The message to be displayed on failure</param>
		public static void IsEmpty( ICollection collection, string message )
		{
			IsEmpty( collection, message, null );
		}

		/// <summary>
		/// Assert that an array,list or other collection is empty
		/// </summary>
		/// <param name="collection">An array, list or other collection implementing ICollection</param>
		public static void IsEmpty( ICollection collection )
		{
			IsEmpty( collection, string.Empty, null );
		}
		#endregion

		#region IsNotEmpty
		/// <summary>
		/// Assert that a string is empty - that is equal to string.Emtpy
		/// </summary>
		/// <param name="aString">The string to be tested</param>
		/// <param name="message">The message to be displayed on failure</param>
		/// <param name="args">Arguments to be used in formatting the message</param>
		public static void IsNotEmpty( string aString, string message, params object[] args )
		{
			DoAssert( new NotEmptyAsserter( aString, message, args ) );
		}

		/// <summary>
		/// Assert that a string is empty - that is equal to string.Emtpy
		/// </summary>
		/// <param name="aString">The string to be tested</param>
		/// <param name="message">The message to be displayed on failure</param>
		public static void IsNotEmpty( string aString, string message )
		{
			IsNotEmpty( aString, message, null );
		}

		/// <summary>
		/// Assert that a string is empty - that is equal to string.Emtpy
		/// </summary>
		/// <param name="aString">The string to be tested</param>
		public static void IsNotEmpty( string aString )
		{
			IsNotEmpty( aString, string.Empty, null );
		}

		/// <summary>
		/// Assert that an array, list or other collection is empty
		/// </summary>
		/// <param name="collection">An array, list or other collection implementing ICollection</param>
		/// <param name="message">The message to be displayed on failure</param>
		/// <param name="args">Arguments to be used in formatting the message</param>
		public static void IsNotEmpty( ICollection collection, string message, params object[] args )
		{
			DoAssert( new NotEmptyAsserter( collection, message, args ) );
		}

		/// <summary>
		/// Assert that an array, list or other collection is empty
		/// </summary>
		/// <param name="collection">An array, list or other collection implementing ICollection</param>
		/// <param name="message">The message to be displayed on failure</param>
		public static void IsNotEmpty( ICollection collection, string message )
		{
			IsNotEmpty( collection, message, null );
		}

		/// <summary>
		/// Assert that an array,list or other collection is empty
		/// </summary>
		/// <param name="collection">An array, list or other collection implementing ICollection</param>
		public static void IsNotEmpty( ICollection collection )
		{
			IsNotEmpty( collection, string.Empty, null );
		}
		#endregion

		#region IsAssignableFrom
		/// <summary>
		/// Asserts that an object may be assigned a  value of a given Type.
		/// </summary>
		/// <param name="expected">The expected Type.</param>
		/// <param name="actual">The object under examination</param>
		static public void IsAssignableFrom( System.Type expected, object actual )
		{
			IsAssignableFrom(expected, actual, "");
		}

		/// <summary>
		/// Asserts that an object may be assigned a  value of a given Type.
		/// </summary>
		/// <param name="expected">The expected Type.</param>
		/// <param name="actual">The object under examination</param>
		/// <param name="message">The messge to display in case of failure</param>
		static public void IsAssignableFrom( System.Type expected, object actual, string message )
		{
			IsAssignableFrom(expected, actual, message, null);
		}
		
		/// <summary>
		/// Asserts that an object may be assigned a  value of a given Type.
		/// </summary>
		/// <param name="expected">The expected Type.</param>
		/// <param name="actual">The object under examination</param>
		/// <param name="message">The message to display in case of failure</param>
		/// <param name="args">Array of objects to be used in formatting the message</param>
		static public void IsAssignableFrom( System.Type expected, object actual, string message, params object[] args )
		{
			Assert.DoAssert( new AssignableFromAsserter( expected, actual, message, args ) );
		}
		#endregion
		
		#region IsNotAssignableFrom
		/// <summary>
		/// Asserts that an object may not be assigned a  value of a given Type.
		/// </summary>
		/// <param name="expected">The expected Type.</param>
		/// <param name="actual">The object under examination</param>
		static public void IsNotAssignableFrom( System.Type expected, object actual )
		{
			IsNotAssignableFrom(expected, actual, "");
		}
		
		/// <summary>
		/// Asserts that an object may not be assigned a  value of a given Type.
		/// </summary>
		/// <param name="expected">The expected Type.</param>
		/// <param name="actual">The object under examination</param>
		/// <param name="message">The messge to display in case of failure</param>
		static public void IsNotAssignableFrom( System.Type expected, object actual, string message )
		{
			IsNotAssignableFrom(expected, actual, message, null);
		}
		
		/// <summary>
		/// Asserts that an object may not be assigned a  value of a given Type.
		/// </summary>
		/// <param name="expected">The expected Type.</param>
		/// <param name="actual">The object under examination</param>
		/// <param name="message">The message to display in case of failure</param>
		/// <param name="args">Array of objects to be used in formatting the message</param>
		static public void IsNotAssignableFrom( System.Type expected, object actual, string message, params object[] args )
		{
			Assert.DoAssert( new NotAssignableFromAsserter( expected, actual, message, args ) );
		}
		#endregion
		
		#region IsInstanceOfType
		/// <summary>
		/// Asserts that an object is an instance of a given type.
		/// </summary>
		/// <param name="expected">The expected Type</param>
		/// <param name="actual">The object being examined</param>
		public static void IsInstanceOfType( System.Type expected, object actual )
		{
			IsInstanceOfType( expected, actual, string.Empty, null );
		}

		/// <summary>
		/// Asserts that an object is an instance of a given type.
		/// </summary>
		/// <param name="expected">The expected Type</param>
		/// <param name="actual">The object being examined</param>
		/// <param name="message">A message to display in case of failure</param>
		public static void IsInstanceOfType( System.Type expected, object actual, string message )
		{
			IsInstanceOfType( expected, actual, message, null );
		}

		/// <summary>
		/// Asserts that an object is an instance of a given type.
		/// </summary>
		/// <param name="expected">The expected Type</param>
		/// <param name="actual">The object being examined</param>
		/// <param name="message">A message to display in case of failure</param>
		/// <param name="args">An array of objects to be used in formatting the message</param>
		public static void IsInstanceOfType( System.Type expected, object actual, string message, params object[] args )
		{
			Assert.DoAssert( new InstanceOfTypeAsserter( expected, actual, message, args ) );
		}
		#endregion

		#region IsNotInstanceOfType
		/// <summary>
		/// Asserts that an object is not an instance of a given type.
		/// </summary>
		/// <param name="expected">The expected Type</param>
		/// <param name="actual">The object being examined</param>
		public static void IsNotInstanceOfType( System.Type expected, object actual )
		{
			IsNotInstanceOfType( expected, actual, string.Empty, null );
		}

		/// <summary>
		/// Asserts that an object is not an instance of a given type.
		/// </summary>
		/// <param name="expected">The expected Type</param>
		/// <param name="actual">The object being examined</param>
		/// <param name="message">A message to display in case of failure</param>
		public static void IsNotInstanceOfType( System.Type expected, object actual, string message )
		{
			IsNotInstanceOfType( expected, actual, message, null );
		}

		/// <summary>
		/// Asserts that an object is not an instance of a given type.
		/// </summary>
		/// <param name="expected">The expected Type</param>
		/// <param name="actual">The object being examined</param>
		/// <param name="message">A message to display in case of failure</param>
		/// <param name="args">An array of objects to be used in formatting the message</param>
		public static void IsNotInstanceOfType( System.Type expected, object actual, string message, params object[] args )
		{
			Assert.DoAssert( new NotInstanceOfTypeAsserter( expected, actual, message, args ) );
		}
		#endregion

		#region AreEqual

		#region Ints

		/// <summary>
		/// Verifies that two ints are equal. If they are not, then an 
		/// <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		/// <param name="message">The message that will be displayed on failure</param>
		/// <param name="args">Arguments to be used in formatting the message</param>
		static public void AreEqual(int expected, 
			int actual, string message, params object[] args) 
		{
			DoAssert( new EqualAsserter( expected, actual, message, args ) );
		}

		/// <summary>
		/// Verifies that two ints are equal. If they are not, then an 
		/// <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		/// <param name="message">The message that will be displayed on failure</param>
		static public void AreEqual(int expected, int actual, string message) 
		{
			Assert.AreEqual( expected, actual, message, null );
		}

		/// <summary>
		/// Verifies that two ints are equal. If they are not, then an 
		/// <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		static public void AreEqual(int expected, int actual ) 
		{
			Assert.AreEqual( expected, actual, string.Empty, null );
		}

		#endregion

		#region Decimals

		/// <summary>
		/// Verifies that two decimals are equal. If they are not, then an 
		/// <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		/// <param name="message">The message that will be displayed on failure</param>
		/// <param name="args">Arguments to be used in formatting the message</param>
		static public void AreEqual(decimal expected, 
			decimal actual, string message, params object[] args) 
		{
			DoAssert( new EqualAsserter( expected, actual, message, args ) );
		}

		/// <summary>
		/// Verifies that two decimal are equal. If they are not, then an 
		/// <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		/// <param name="message">The message that will be displayed on failure</param>
		static public void AreEqual(decimal expected, decimal actual, string message) 
		{
			Assert.AreEqual( expected, actual, message, null );
		}

		/// <summary>
		/// Verifies that two decimals are equal. If they are not, then an 
		/// <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		static public void AreEqual(decimal expected, decimal actual ) 
		{
			Assert.AreEqual( expected, actual, string.Empty, null );
		}

		#endregion

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
		/// <param name="message">The message that will be displayed on failure</param>
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
		/// <param name="message">The message that will be displayed on failure</param>
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
		/// <param name="message">The message displayed upon failure</param>
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
		/// <param name="message">The message displayed upon failure</param>
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

		#region Objects
		/// <summary>
		/// Asserts that two objects are not equal. If they are equal
		/// an <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The expected object</param>
		/// <param name="actual">The actual object</param>
		/// <param name="message">The message to be displayed when the two objects are the same object.</param>
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
		/// <param name="message">The message to be displayed when the objects are the same</param>
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

		#region Ints
		/// <summary>
		/// Asserts that two ints are not equal. If they are equal
		/// an <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The expected object</param>
		/// <param name="actual">The actual object</param>
		/// <param name="message">The message to be displayed when the two objects are the same object.</param>
		/// <param name="args">Arguments to be used in formatting the message</param>
		static public void AreNotEqual( int expected, int actual, string message, params object[] args)
		{
			DoAssert( new NotEqualAsserter( expected, actual, message, args ) );
		}

		/// <summary>
		/// Asserts that two ints are not equal. If they are equal
		/// an <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The expected object</param>
		/// <param name="actual">The actual object</param>
		/// <param name="message">The message to be displayed when the objects are the same</param>
		static public void AreNotEqual(int expected, int actual, string message) 
		{
			Assert.AreNotEqual(expected, actual, message, null);
		}
   
		/// <summary>
		/// Asserts that two ints are not equal. If they are equal
		/// an <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The expected object</param>
		/// <param name="actual">The actual object</param>
		static public void AreNotEqual(int expected, int actual) 
		{
			Assert.AreNotEqual(expected, actual, string.Empty, null);
		}
		#endregion

		#region Decimals
		/// <summary>
		/// Asserts that two decimals are not equal. If they are equal
		/// an <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The expected object</param>
		/// <param name="actual">The actual object</param>
		/// <param name="message">The message to be displayed when the two objects are the same object.</param>
		/// <param name="args">Arguments to be used in formatting the message</param>
		static public void AreNotEqual( decimal expected, decimal actual, string message, params object[] args)
		{
			DoAssert( new NotEqualAsserter( expected, actual, message, args ) );
		}

		/// <summary>
		/// Asserts that two decimals are not equal. If they are equal
		/// an <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The expected object</param>
		/// <param name="actual">The actual object</param>
		/// <param name="message">The message to be displayed when the objects are the same</param>
		static public void AreNotEqual(decimal expected, decimal actual, string message) 
		{
			Assert.AreNotEqual(expected, actual, message, null);
		}
   
		/// <summary>
		/// Asserts that two decimals are not equal. If they are equal
		/// an <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The expected object</param>
		/// <param name="actual">The actual object</param>
		static public void AreNotEqual(decimal expected, decimal actual) 
		{
			Assert.AreNotEqual(expected, actual, string.Empty, null);
		}
		#endregion

		#region Floats
		/// <summary>
		/// Asserts that two floats are not equal. If they are equal
		/// an <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The expected object</param>
		/// <param name="actual">The actual object</param>
		/// <param name="message">The message to be displayed when the two objects are the same object.</param>
		/// <param name="args">Arguments to be used in formatting the message</param>
		static public void AreNotEqual( float expected, float actual, string message, params object[] args)
		{
			DoAssert( new NotEqualAsserter( expected, actual, message, args ) );
		}

		/// <summary>
		/// Asserts that two floats are not equal. If they are equal
		/// an <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The expected object</param>
		/// <param name="actual">The actual object</param>
		/// <param name="message">The message to be displayed when the objects are the same</param>
		static public void AreNotEqual(float expected, float actual, string message) 
		{
			Assert.AreNotEqual(expected, actual, message, null);
		}
   
		/// <summary>
		/// Asserts that two floats are not equal. If they are equal
		/// an <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The expected object</param>
		/// <param name="actual">The actual object</param>
		static public void AreNotEqual(float expected, float actual) 
		{
			Assert.AreNotEqual(expected, actual, string.Empty, null);
		}
		#endregion

		#region Doubles
		/// <summary>
		/// Asserts that two doubles are not equal. If they are equal
		/// an <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The expected object</param>
		/// <param name="actual">The actual object</param>
		/// <param name="message">The message to be displayed when the two objects are the same object.</param>
		/// <param name="args">Arguments to be used in formatting the message</param>
		static public void AreNotEqual( double expected, double actual, string message, params object[] args)
		{
			DoAssert( new NotEqualAsserter( expected, actual, message, args ) );
		}

		/// <summary>
		/// Asserts that two doubles are not equal. If they are equal
		/// an <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The expected object</param>
		/// <param name="actual">The actual object</param>
		/// <param name="message">The message to be displayed when the objects are the same</param>
		static public void AreNotEqual(double expected, double actual, string message) 
		{
			Assert.AreNotEqual(expected, actual, message, null);
		}
   
		/// <summary>
		/// Asserts that two doubles are not equal. If they are equal
		/// an <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The expected object</param>
		/// <param name="actual">The actual object</param>
		static public void AreNotEqual(double expected, double actual) 
		{
			Assert.AreNotEqual(expected, actual, string.Empty, null);
		}
		#endregion

		#endregion

		#region AreSame

		/// <summary>
		/// Asserts that two objects refer to the same object. If they
		/// are not the same an <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The expected object</param>
		/// <param name="actual">The actual object</param>
		/// <param name="message">The message to be displayed when the two objects are not the same object.</param>
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
		/// <param name="message">The message to be displayed when the object is null</param>
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
		/// <param name="message">The message to be displayed when the two objects are the same object.</param>
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
		/// <param name="message">The message to be displayed when the objects are the same</param>
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

		#region Greater

		#region Ints

		/// <summary>
		/// Verifies that the actual value is greater than the expected
		/// value. If they are not, then an 
		/// <see cref="AssertionException"/> is thrown. For this method
		/// expected and actual are reversed from their normal order to 
		/// more closely fit the way this operation would read in english.
		/// For instance 5 > 4 would be written Assert.Greater(5,4).
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		/// <param name="message">The message that will be displayed on failure</param>
		/// <param name="args">Arguments to be used in formatting the message</param>
		static public void Greater(int actual, 
			int expected, string message, params object[] args) 
		{
			DoAssert( new GreaterAsserter( expected, actual, message, args ) );
		}

		/// <summary>
		/// Verifies that the actual value is greater than the expected
		/// value. If they are not, then an 
		/// <see cref="AssertionException"/> is thrown. For this method
		/// expected and actual are reversed from their normal order to 
		/// more closely fit the way this operation would read in english.
		/// For instance 5 > 4 would be written Assert.Greater(5,4).
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		/// <param name="message">The message that will be displayed on failure</param>
		static public void Greater(int actual, int expected, string message) 
		{
			Assert.Greater( actual, expected, message, null );
		}

		/// <summary>
		/// Verifies that the actual value is greater than the expected
		/// value. If they are not, then an 
		/// <see cref="AssertionException"/> is thrown. For this method
		/// expected and actual are reversed from their normal order to 
		/// more closely fit the way this operation would read in english.
		/// For instance 5 > 4 would be written Assert.Greater(5,4).
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		static public void Greater(int actual, int expected ) 
		{
			Assert.Greater( actual, expected, string.Empty, null );
		}

		#endregion

		#region Decimals

		/// <summary>
		/// Verifies that the actual value is greater than the expected
		/// value. If they are not, then an 
		/// <see cref="AssertionException"/> is thrown. For this method
		/// expected and actual are reversed from their normal order to 
		/// more closely fit the way this operation would read in english.
		/// For instance 5 > 4 would be written Assert.Greater(5,4).
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		/// <param name="message">The message that will be displayed on failure</param>
		/// <param name="args">Arguments to be used in formatting the message</param>
		static public void Greater(decimal actual, 
			decimal expected, string message, params object[] args) 
		{
			DoAssert( new GreaterAsserter( expected, actual, message, args ) );
		}

		/// <summary>
		/// Verifies that the actual value is greater than the expected
		/// value. If they are not, then an 
		/// <see cref="AssertionException"/> is thrown. For this method
		/// expected and actual are reversed from their normal order to 
		/// more closely fit the way this operation would read in english.
		/// For instance 5 > 4 would be written Assert.Greater(5,4).
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		/// <param name="message">The message that will be displayed on failure</param>
		static public void Greater(decimal actual, decimal expected, string message) 
		{
			Assert.Greater( actual, expected, message, null );
		}

		/// <summary>
		/// Verifies that the actual value is greater than the expected
		/// value. If they are not, then an 
		/// <see cref="AssertionException"/> is thrown. For this method
		/// expected and actual are reversed from their normal order to 
		/// more closely fit the way this operation would read in english.
		/// For instance 5 > 4 would be written Assert.Greater(5,4).
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		static public void Greater(decimal actual, decimal expected ) 
		{
			Assert.AreEqual( actual, expected, string.Empty, null );
		}

		#endregion

		#region Doubles

		/// <summary>
		/// Verifies that the actual value is greater than the expected
		/// value. If they are not, then an 
		/// <see cref="AssertionException"/> is thrown. For this method
		/// expected and actual are reversed from their normal order to 
		/// more closely fit the way this operation would read in english.
		/// For instance 5 > 4 would be written Assert.Greater(5,4).
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		/// <param name="message">The message that will be displayed on failure</param>
		/// <param name="args">Arguments to be used in formatting the message</param>
		static public void Greater(double actual, 
			double expected, string message, params object[] args) 
		{
			DoAssert( new GreaterAsserter( expected, actual, message, args ) );
		}

		/// <summary>
		/// Verifies that the actual value is greater than the expected
		/// value. If they are not, then an 
		/// <see cref="AssertionException"/> is thrown. For this method
		/// expected and actual are reversed from their normal order to 
		/// more closely fit the way this operation would read in english.
		/// For instance 5 > 4 would be written Assert.Greater(5,4).
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		/// <param name="message">The message that will be displayed on failure</param>
		static public void Greater(double actual, 
			double expected, string message) 
		{
			Assert.Greater( actual, expected, message, null );
		}

		/// <summary>
		/// Verifies that the actual value is greater than the expected
		/// value. If they are not, then an 
		/// <see cref="AssertionException"/> is thrown. For this method
		/// expected and actual are reversed from their normal order to 
		/// more closely fit the way this operation would read in english.
		/// For instance 5 > 4 would be written Assert.Greater(5,4).
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		static public void Greater(double actual, double expected) 
		{
			Assert.Greater(actual, expected, string.Empty, null);
		}

		#endregion

		#region Floats

		/// <summary>
		/// Verifies that the actual value is greater than the expected
		/// value. If they are not, then an 
		/// <see cref="AssertionException"/> is thrown. For this method
		/// expected and actual are reversed from their normal order to 
		/// more closely fit the way this operation would read in english.
		/// For instance 5 > 4 would be written Assert.Greater(5,4).
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		/// <param name="message">The message that will be displayed on failure</param>
		/// <param name="args">Arguments to be used in formatting the message</param>
		static public void Greater(float actual, 
			float expected, string message, params object[] args) 
		{
			DoAssert( new GreaterAsserter( expected, actual, message, args ) );
		}

		/// <summary>
		/// Verifies that the actual value is greater than the expected
		/// value. If they are not, then an 
		/// <see cref="AssertionException"/> is thrown. For this method
		/// expected and actual are reversed from their normal order to 
		/// more closely fit the way this operation would read in english.
		/// For instance 5 > 4 would be written Assert.Greater(5,4).
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		/// <param name="message">The message that will be displayed on failure</param>
		static public void Greater(float actual, float expected, string message) 
		{
			Assert.Greater(actual, expected, message, null);
		}

		/// <summary>
		/// Verifies that the actual value is greater than the expected
		/// value. If they are not, then an 
		/// <see cref="AssertionException"/> is thrown. For this method
		/// expected and actual are reversed from their normal order to 
		/// more closely fit the way this operation would read in english.
		/// For instance 5 > 4 would be written Assert.Greater(5,4).
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		static public void Greater(float actual, float expected) 
		{
			Assert.Greater(actual, expected, string.Empty, null);
		}

		#endregion

		#region IComparables

		/// <summary>
		/// Verifies that the actual value is greater than the expected
		/// value. If they are not, then an 
		/// <see cref="AssertionException"/> is thrown. For this method
		/// expected and actual are reversed from their normal order to 
		/// more closely fit the way this operation would read in english.
		/// For instance 5 > 4 would be written Assert.Greater(5,4).
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		/// <param name="message">The message that will be displayed on failure</param>
		/// <param name="args">Arguments to be used in formatting the message</param>
		static public void Greater(IComparable actual, 
			IComparable expected, string message, params object[] args) 
		{
			DoAssert( new GreaterAsserter( expected, actual, message, args ) );
		}

		/// <summary>
		/// Verifies that the actual value is greater than the expected
		/// value. If they are not, then an 
		/// <see cref="AssertionException"/> is thrown. For this method
		/// expected and actual are reversed from their normal order to 
		/// more closely fit the way this operation would read in english.
		/// For instance 5 > 4 would be written Assert.Greater(5,4).
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		/// <param name="message">The message that will be displayed on failure</param>
		static public void Greater(IComparable actual, IComparable expected, string message) 
		{
			Assert.Greater(actual, expected, message, null);
		}

		/// <summary>
		/// Verifies that the actual value is greater than the expected
		/// value. If they are not, then an 
		/// <see cref="AssertionException"/> is thrown. For this method
		/// expected and actual are reversed from their normal order to 
		/// more closely fit the way this operation would read in english.
		/// For instance 5 > 4 would be written Assert.Greater(5,4).
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		static public void Greater(IComparable actual, IComparable expected) 
		{
			Assert.Greater(actual, expected, string.Empty, null);
		}

		#endregion

		#endregion

		#region Less

		#region Ints

		/// <summary>
		/// Verifies that the expected value is less than the actual
		/// value. If they are not, then an 
		/// <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		/// <param name="message">The message that will be displayed on failure</param>
		/// <param name="args">Arguments to be used in formatting the message</param>
		static public void Less(int expected, int actual, string message, params object[] args) 
		{
			DoAssert( new LessAsserter( expected, actual, message, args ) );
		}

		/// <summary>
		/// Verifies that the expected value is less than the actual
		/// value. If they are not, then an 
		/// <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		/// <param name="message">The message that will be displayed on failure</param>
		static public void Less(int expected, int actual, string message) 
		{
			Assert.Less(expected, actual, message, null);
		}

		/// <summary>
		/// Verifies that the expected value is less than the actual
		/// value. If they are not, then an 
		/// <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		static public void Less(int expected, int actual) 
		{
			Assert.Less(expected, actual, string.Empty, null);
		}

		#endregion

		#region Decimals

		/// <summary>
		/// Verifies that the expected value is less than the actual
		/// value. If they are not, then an 
		/// <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		/// <param name="message">The message that will be displayed on failure</param>
		/// <param name="args">Arguments to be used in formatting the message</param>
		static public void Less(decimal expected, decimal actual, string message, params object[] args) 
		{
			DoAssert( new LessAsserter( expected, actual, message, args ) );
		}

		/// <summary>
		/// Verifies that the expected value is less than the actual
		/// value. If they are not, then an 
		/// <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		/// <param name="message">The message that will be displayed on failure</param>
		static public void Less(decimal expected, decimal actual, string message) 
		{
			Assert.Less(expected, actual, message, null);
		}

		/// <summary>
		/// Verifies that the expected value is less than the actual
		/// value. If they are not, then an 
		/// <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		static public void Less(decimal expected, decimal actual) 
		{
			Assert.Less(expected, actual, string.Empty, null);
		}

		#endregion

		#region Doubles

		/// <summary>
		/// Verifies that the expected value is less than the actual
		/// value. If they are not, then an 
		/// <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		/// <param name="message">The message that will be displayed on failure</param>
		/// <param name="args">Arguments to be used in formatting the message</param>
		static public void Less(double expected, double actual, string message, params object[] args) 
		{
			DoAssert( new LessAsserter( expected, actual, message, args ) );
		}

		/// <summary>
		/// Verifies that the expected value is less than the actual
		/// value. If they are not, then an 
		/// <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		/// <param name="message">The message that will be displayed on failure</param>
		static public void Less(double expected, double actual, string message) 
		{
			Assert.Less(expected, actual, message, null);
		}

		/// <summary>
		/// Verifies that the expected value is less than the actual
		/// value. If they are not, then an 
		/// <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		static public void Less(double expected, double actual) 
		{
			Assert.Less(expected, actual, string.Empty, null);
		}

		#endregion

		#region Floats

		/// <summary>
		/// Verifies that the expected value is less than the actual
		/// value. If they are not, then an 
		/// <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		/// <param name="message">The message that will be displayed on failure</param>
		/// <param name="args">Arguments to be used in formatting the message</param>
		static public void Less(float expected, float actual, string message, params object[] args) 
		{
			DoAssert( new LessAsserter( expected, actual, message, args ) );
		}

		/// <summary>
		/// Verifies that the expected value is less than the actual
		/// value. If they are not, then an 
		/// <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		/// <param name="message">The message that will be displayed on failure</param>
		static public void Less(float expected, float actual, string message) 
		{
			Assert.Less(expected, actual, message, null);
		}

		/// <summary>
		/// Verifies that the expected value is less than the actual
		/// value. If they are not, then an 
		/// <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		static public void Less(float expected, float actual) 
		{
			Assert.Less(expected, actual, string.Empty, null);
		}

		#endregion

		#region IComparables

		/// <summary>
		/// Verifies that the expected value is less than the actual
		/// value. If they are not, then an 
		/// <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		/// <param name="message">The message that will be displayed on failure</param>
		/// <param name="args">Arguments to be used in formatting the message</param>
		static public void Less(IComparable expected, IComparable actual, string message, params object[] args) 
		{
			DoAssert( new LessAsserter( expected, actual, message, args ) );
		}

		/// <summary>
		/// Verifies that the expected value is less than the actual
		/// value. If they are not, then an 
		/// <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		/// <param name="message">The message that will be displayed on failure</param>
		static public void Less(IComparable expected, IComparable actual, string message) 
		{
			Assert.Less(expected, actual, message, null);
		}

		/// <summary>
		/// Verifies that the expected value is less than the actual
		/// value. If they are not, then an 
		/// <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		static public void Less(IComparable expected, IComparable actual) 
		{
			Assert.Less(expected, actual, string.Empty, null);
		}

		#endregion

		#endregion

		#region List Containment

		/// <summary>
		/// Asserts that an object is contained in a list.
		/// </summary>
		/// <param name="expected">The expected object</param>
		/// <param name="actual">The list to be examined</param>
		/// <param name="message">The message to display in case of failure</param>
		/// <param name="args">Arguments used in formatting the message</param>
		static public void Contains( object expected, IList actual, string message, params object[] args )
		{
			Assert.DoAssert( new ListContentsAsserter( expected, actual, message, args ) );
		}

		/// <summary>
		/// Asserts that an object is contained in a list.
		/// </summary>
		/// <param name="expected">The expected object</param>
		/// <param name="actual">The list to be examined</param>
		/// <param name="message">The message to display in case of failure</param>
		static public void Contains( object expected, IList actual, string message )
		{
			Contains( expected, actual, message, null );
		}

		/// <summary>
		/// Asserts that an object is contained in a list.
		/// </summary>
		/// <param name="expected">The expected object</param>
		/// <param name="actual">The list to be examined</param>
		static public void Contains( object expected, IList actual )
		{
			Contains( expected, actual, string.Empty, null );
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
		static public void Ignore( string message, params object[] args )
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

		/// <summary>
		/// Test the condition asserted by an asserter and throw
		/// an assertion exception using provided message on failure.
		/// </summary>
		/// <param name="asserter">An object that implements IAsserter</param>
		static public void DoAssert( IAsserter asserter )
		{
			Assert.IncrementAssertCount();
			if ( !asserter.Test() )
				throw new AssertionException( asserter.Message );
		}

		#endregion
	}
}