// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

using ICSharpCode.Core;

namespace ICSharpCode.Core
{
	public interface IStringTagProvider
	{
		string[] Tags {
			get;
		}
		
		string Convert(string tag);
	}
}
