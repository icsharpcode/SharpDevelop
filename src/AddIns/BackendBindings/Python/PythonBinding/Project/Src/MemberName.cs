// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
