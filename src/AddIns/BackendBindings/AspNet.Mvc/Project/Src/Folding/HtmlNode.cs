// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Text;

namespace ICSharpCode.AspNet.Mvc.Folding
{
	public class HtmlNode
	{
		StringBuilder nodeValue = new StringBuilder();
		
		public string Value {
			get { return GetElementNameBeforeSpaceCharacter(); }
		}
		
		string GetElementNameBeforeSpaceCharacter()
		{
			string elementName = nodeValue.ToString();
			return GetTextBeforeSpaceCharacter(elementName);
		}
		
		string GetTextBeforeSpaceCharacter(string text)
		{
			int spaceIndex = text.IndexOf(' ');
			if (spaceIndex > 0) {
				return text.Substring(0, spaceIndex);
			}
			return text;
		}
		
		public void Append(int character)
		{
			nodeValue.Append((char)character);
		}
	}
}
