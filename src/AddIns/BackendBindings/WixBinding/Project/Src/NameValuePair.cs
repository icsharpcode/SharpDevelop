// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
