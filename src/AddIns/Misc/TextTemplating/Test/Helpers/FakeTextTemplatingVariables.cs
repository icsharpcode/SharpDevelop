// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.TextTemplating;

namespace TextTemplating.Tests.Helpers
{
	public class FakeTextTemplatingVariables : ITextTemplatingVariables
	{
		public Dictionary<string, string> Variables = new Dictionary<string, string>();
		
		public void AddVariable(string name, string value)
		{
			name = "$(" + name + ")";
			Variables.Add(name, value);
		}
		
		public string ExpandVariables(string name)
		{
			foreach (KeyValuePair<string, string> variable in Variables) {
				name = name.Replace(variable.Key, variable.Value);
			}
			return name;
		}
		
		public string GetValue(string name)
		{
			string variableValue = null;
			Variables.TryGetValue(name, out variableValue);
			return variableValue;
		}
	}
}
