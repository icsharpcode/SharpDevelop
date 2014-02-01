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
using System.Diagnostics;
using ICSharpCode.Core;

namespace SharpRefactoring
{
	/// <summary>
	/// Description of Options.
	/// </summary>
	public static class Options
	{
		static Properties properties;
		
		public static Properties Properties {
			get {
				Debug.Assert(properties != null);
				return properties;
			}
		}
		
		static Options()
		{
			properties = PropertyService.Get("SharpRefactoringOptions", new Properties());
		}
		
		public static bool AddIEquatableInterface {
			get { return properties.Get("AddIEquatableInterface", false); }
			set { properties.Set("AddIEquatableInterface", value); }
		}
		
		public static bool AddOtherMethod {
			get { return properties.Get("AddOtherMethod", true); }
			set { properties.Set("AddOtherMethod", value); }
		}
		
		public static bool SurroundWithRegion {
			get { return properties.Get("SurroundWithRegion", true); }
			set { properties.Set("SurroundWithRegion", value); }
		}
		
		public static bool AddOperatorOverloads {
			get { return properties.Get("AddOperatorOverloads", true); }
			set { properties.Set("AddOperatorOverloads", value); }
		}
	}
}
