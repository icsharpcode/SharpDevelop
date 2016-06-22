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

namespace ICSharpCode.WixBinding
{
	/// <summary>
	/// Represents a name value pair of the form 'name=value'.
	/// </summary>
	public class NameValuePair
	{
		string name = String.Empty;
		string value = String.Empty;
		
		public NameValuePair(string name, string value)
		{
			this.name = name;
			this.value = value;
		}
		
		public NameValuePair(string name) : this(name, String.Empty)
		{
		}
		
		public string Name {
			get {
				return name;
			}
		}
		
		public string Value {
			get {
				return value;
			}
		}
		
		public override string ToString()
		{
			return String.Concat(name, "=", value);
		}
	}
}
