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
using System.Collections.Generic;
using ICSharpCode.Scripting;

namespace ICSharpCode.Scripting.Tests.Utils
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
