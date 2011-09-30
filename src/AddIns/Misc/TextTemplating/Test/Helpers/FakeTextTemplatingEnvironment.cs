// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.TextTemplating;

namespace TextTemplating.Tests.Helpers
{
	public class FakeTextTemplatingEnvironment : ITextTemplatingEnvironment
	{
		public Dictionary<string, string> Variables = new Dictionary<string, string>();
		
		public string ExpandEnvironmentVariables(string name)
		{
			string value = null;
			if (Variables.TryGetValue(name, out value)) {
				return value;
			}
			return name;
		}
		
		public void AddVariable(string name, string value)
		{
			name = "%" + name + "%";
			Variables.Add(name, value);
		}
	}
}
