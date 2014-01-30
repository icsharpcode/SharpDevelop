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

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Split up an expression into a member name and a type name.
	/// </summary>
	/// <example>
	/// "myObject.Field" => "myObject" + "Field"
	/// "System.Console.WriteLine" => "System.Console" + "Console.WriteLine"
	/// </example>
	public class MemberName
	{
		string name = String.Empty;
		string type = String.Empty;
		
		public MemberName(string expression)
		{
			Parse(expression);
		}
		
		public MemberName(string typeName, string memberName)
		{
			this.type = typeName;
			this.name = memberName;
		}
		
		void Parse(string expression)
		{
			if (!String.IsNullOrEmpty(expression)) {
				int index = expression.LastIndexOf('.');
				if (index > 0) {
					type = expression.Substring(0, index);
					name = expression.Substring(index + 1);
				} else {
					type = expression;
				}
			}
		}
		
		public string Name {
			get { return name; }
		}
		
		public bool HasName {
			get { return !String.IsNullOrEmpty(name); }
		}
		
		public string Type {
			get { return type; }
		}
		
		public override string ToString()
		{
			return String.Format("Type: {0}, Member: {1}", type, name);
		}
		
		public override bool Equals(object obj)
		{
			MemberName rhs = obj as MemberName;
			if (rhs != null) {
				return (name == rhs.name) && (type == rhs.type);
			}
			return false;
		}
		
		public override int GetHashCode()
		{
			return name.GetHashCode() ^ type.GetHashCode();
		}
	}
}
