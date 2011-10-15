// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.TextTemplating;

namespace TextTemplating.Tests.Helpers
{
	public class FakeTextTemplatingStringParser : ITextTemplatingStringParser
	{
		public Dictionary<string, string> Properties = new Dictionary<string, string>();
		
		public string GetValue(string propertyName)
		{
			string propertyValue = null;
			Properties.TryGetValue(propertyName, out propertyValue);
			return propertyValue;
		}
		
		public void AddProperty(string name, string value)
		{
			Properties.Add(name, value);
		}
	}
}
