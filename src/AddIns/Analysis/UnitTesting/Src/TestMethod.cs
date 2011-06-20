// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.UnitTesting
{
	/// <summary>
	/// Represents a method that can be tested. 
	/// </summary>
	public class TestMethod
	{
		string prefix = String.Empty;
		IMember method;
		TestResultType testResultType = TestResultType.None;
		
		/// <summary>
		/// Raised when the test result has changed.
		/// </summary>
		public event EventHandler ResultChanged;
		
		public TestMethod(IMember method)
		{
			this.method = method;
		}
		
		/// <summary>
		/// Creates a new TestMethod instance.
		/// </summary>
		/// <param name="prefix">The prefix to be added to the
		/// method name with a dot character. This is specified
		/// when the test method refers to a base class method. NUnit
		/// refers to base class test methods by prefixing the
		/// class name to it:
		///
		/// [TestFixture]
		/// public class DerivedClass : BaseClass
		/// 
		/// Test method name:
		/// 
		/// RootNamespace.DerivedClass.BaseClass.BaseClassMethod
		/// </param>
		public TestMethod(string prefix, IMethod method)
		{
			this.method = method;
			this.prefix = prefix;
		}
		
		/// <summary>
		/// Gets the underlying IMethod for this TestMethod.
		/// </summary>
		public IMember Method {
			get { return method; }
		}
		
		/// <summary>
		/// Updates the test method based on new information 
		/// in the specified IMethod.
		/// </summary>
		public void Update(IMember method)
		{
			this.method = method;
		}
		
		/// <summary>
		/// Gets the test result for this method.
		/// </summary>
		public TestResultType Result {
			get { return testResultType; }
			set {
				TestResultType previousTestResultType = testResultType;
				testResultType = value;
				if (previousTestResultType != testResultType) {
					OnResultChanged();
				}
			}
		}
		
		/// <summary>
		/// Gets the method's name.
		/// </summary>
		public string Name {
			get {
				if (prefix.Length > 0) {
					return String.Concat(prefix, ".", method.Name);
				}
				return method.Name;
			}
		}
		
		/// <summary>
		/// Returns the method name from a fully qualified name
		/// of the form:
		/// 
		/// Namespace.Class.Method
		/// </summary>
		public static string GetMethodName(string qualifiedName)
		{
			if (qualifiedName != null) {
				int index = qualifiedName.LastIndexOf('.');
				if (index >= 0) {
					return qualifiedName.Substring(index + 1);
				}
			}
			return null;
		}
		
		/// <summary>
		/// Returns the namespace and class name from a fully qualified 
		/// name of the form:
		/// 
		/// Namespace.Class.Method
		/// </summary>
		public static string GetQualifiedClassName(string qualifiedName)
		{
			if (qualifiedName != null) {
				int methodIndex = qualifiedName.LastIndexOf('.');
				if (methodIndex >= 0) {
					return qualifiedName.Substring(0, methodIndex);
				}
			}
			return null;
		}
		
		/// <summary>
		/// Raises the ResultChanged event.
		/// </summary>
		void OnResultChanged()
		{
			if (ResultChanged != null) {
				ResultChanged(this, new EventArgs());
			}
		}
	}
}
