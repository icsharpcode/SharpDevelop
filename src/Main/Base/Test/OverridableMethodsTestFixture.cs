// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Tests.Utils;
using NUnit.Framework;

namespace ICSharpCode.SharpDevelop.Tests
{
	/// <summary>
	/// Tests the OverrideCompletionDataProvider GetOverridableMethods.
	/// This method should be added to the IClass interface.
	/// </summary>
	[TestFixture]
	public class OverridableMethodsTestFixture
	{
		MockClass c;
		MockDefaultReturnType returnType;
		List<IMethod> expectedMethods;
		MockClass declaringType;
		
		[SetUp]
		public void SetUp()
		{
			expectedMethods = new List<IMethod>();
			c = new MockClass("MyClass");
			declaringType = new MockClass("MyDeclaringType");
			returnType = new MockDefaultReturnType();
			c.DefaultReturnType = returnType;
		}
		
		IMethod[] GetOverridableMethods(IClass baseClass)
		{
			return OverrideCompletionItemProvider.GetOverridableMethods(new MockClass("DerivedClass") { BaseType = baseClass.DefaultReturnType });
		}
		
		/// <summary>
		/// Add one overridable method to the return type and this
		/// should be returned in the list of overridable methods.
		/// </summary>
		[Test]
		public void OneOverridableMethodReturned()
		{
			MockMethod method = new MockMethod("Run");
			method.DeclaringType = declaringType;
			method.IsOverridable = true;
			returnType.Methods.Add(method);
			
			expectedMethods.Add(method);

			IMethod[] methods = GetOverridableMethods(c);
			
			AssertAreMethodsEqual(expectedMethods, methods);
		}
		
		/// <summary>
		/// Make sure that an overridable method is not returned when
		/// it is part of the class being considered.
		/// </summary>
		[Test]
		public void OverridableMethodPartOfClass()
		{
			MockMethod method = new MockMethod("Run");
			method.DeclaringType = c;
			method.IsOverridable = true;
			returnType.Methods.Add(method);
			
			IMethod[] methods = OverrideCompletionItemProvider.GetOverridableMethods(c);
			
			AssertAreMethodsEqual(expectedMethods, methods);
		}
		
		/// <summary>
		/// An overridable but const method should not be returned.
		/// </summary>
		[Test]
		public void OverridableConstMethodNotReturned()
		{
			MockMethod method = new MockMethod("Run");
			method.DeclaringType = declaringType;
			method.IsOverridable = true;
			method.IsConst = true;
			returnType.Methods.Add(method);
			
			IMethod[] methods = GetOverridableMethods(c);
			
			AssertAreMethodsEqual(expectedMethods, methods);
		}
		
		/// <summary>
		/// An overridable but private method should not be returned.
		/// </summary>
		[Test]
		public void OverridablePrivateMethodNotReturned()
		{
			MockMethod method = new MockMethod("Run");
			method.DeclaringType = declaringType;
			method.IsOverridable = true;
			method.IsPrivate = true;
			returnType.Methods.Add(method);
			
			IMethod[] methods = GetOverridableMethods(c);
			
			AssertAreMethodsEqual(expectedMethods, methods);
		}
		
		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void NullArgument()
		{
			OverrideCompletionItemProvider.GetOverridableMethods(null);
		}
		
		void AssertAreMethodsEqual(List<IMethod> expectedMethods, IMethod[] methods)
		{
			// Get a list of expected method names.
			List<string> expectedMethodNames = new List<string>();
			foreach (IMethod expectedMethod in expectedMethods) {
				expectedMethodNames.Add(expectedMethod.Name);
			}
			
			// Get a list of actual method names.
			List<string> methodNames = new List<string>();
			foreach (IMethod m in methods) {
				methodNames.Add(m.Name);
			}
			
			// Compare the two arrays.
			Assert.AreEqual(expectedMethodNames.ToArray(), methodNames.ToArray());
		}
	}
}
