// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.RubyBinding;

namespace RubyBinding.Tests.Console
{
	public class MockMemberProvider : IMemberProvider
	{
		List<string> memberNames = new List<string>();
		string getMemberNamesParameter;
		List<string> globals = new List<string>();
		string getGlobalsParameter;
		Exception exceptionToThrow;
		
		public MockMemberProvider()
		{
		}

		/// <summary>
		/// Exception that will be thrown if the GetMemberNames method or GetGlobals method is called.
		/// </summary>
		public Exception ExceptionToThrow {
			get { return exceptionToThrow; }
			set { exceptionToThrow = value; }
		}
		
		public void SetMemberNames(string[] names)
		{
			memberNames.AddRange(names);
		}
		
		public IList<string> GetMemberNames(string name)
		{
			getMemberNamesParameter = name;

			if (exceptionToThrow != null) {
				throw exceptionToThrow;
			}
			
			return memberNames;
		}
		
		public void SetGlobals(string[] names)
		{
			globals.AddRange(names);
		}
		
		public IList<string> GetGlobals(string name)
		{
			getGlobalsParameter = name;
			
			if (exceptionToThrow != null) {
				throw exceptionToThrow;
			}
			
			return globals;
		}
		
		/// <summary>
		/// Returns the parameter passed to the GetGlobals method.
		/// </summary>
		public string GetGlobalsParameter {
			get { return getGlobalsParameter; }
		}		

		/// <summary>
		/// Returns the parameter passed to the GetMemberNames method.
		/// </summary>
		public string GetMemberNamesParameter {
			get { return getMemberNamesParameter; }
		}		
	}
}
