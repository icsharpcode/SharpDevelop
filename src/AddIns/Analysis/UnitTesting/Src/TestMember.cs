// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.UnitTesting
{
	/// <summary>
	/// Represents a member that can be tested. 
	/// </summary>
	public class TestMember
	{
#if unused
		string prefix = String.Empty;
#endif
		IMember member;
		TestResultType testResultType = TestResultType.None;
		
		/// <summary>
		/// Raised when the test result has changed.
		/// </summary>
		public event EventHandler ResultChanged;		
		
		public TestMember(IMember member)
		{
			this.member = member;
			this.DeclaringType = member.DeclaringType;
		}
		
#if unused
		/// <summary>
		/// Creates a new TestMember instance.
		/// </summary>
		/// <param name="prefix">The prefix to be added to the
		/// member name with a dot character. This is specified
		/// when the test member refers to a base class method. NUnit
		/// refers to base class test methods by prefixing the
		/// class name to it:
		///
		/// [TestFixture]
		/// public class DerivedClass : BaseClass
		/// 
		/// Test member name:
		/// 
		/// RootNamespace.DerivedClass.BaseClass.BaseClassMethod
		/// </param>
		public TestMember(string prefix, IMember member)
		{
			this.member = member;
			this.prefix = prefix;
		}
#endif

		public TestMember(IClass testClass, IMember member)
		{
			this.DeclaringType = testClass;
			this.member = member;
		}
		
		/// <summary>
		/// Gets the underlying IMember for this TestMember.
		/// </summary>
		public IMember Member {
			get { return member; }
		}

		/// <summary>
		/// Gets the class where the test member is actually declared.
		/// In case of test member from base class it will be the base class not the 
		/// </summary>
		public IClass DeclaringType { get; private set; }
		
		/// <summary>
		/// Updates the test member based on new information 
		/// in the specified IMember.
		/// </summary>
		public void Update(IMember member)
		{
			this.member = member;
		}
		
		/// <summary>
		/// Gets the test result for this member.
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
		/// Gets the member's name.
		/// </summary>
		public string Name {
			get {
#if unused
				if (prefix.Length > 0) {
					return String.Concat(prefix, ".", member.Name);
				}
#endif
				return member.Name;
			}
		}
		
		/// <summary>
		/// Returns the member name from a fully qualified name
		/// of the form:
		/// 
		/// Namespace.Class.Member
		/// </summary>
		public static string GetMemberName(string qualifiedName)
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
		/// Namespace.Class.Member
		/// </summary>
		public static string GetQualifiedClassName(string qualifiedName)
		{
			if (qualifiedName != null) {
				int memberIndex = qualifiedName.LastIndexOf('.');
				if (memberIndex >= 0) {
					return qualifiedName.Substring(0, memberIndex);
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
