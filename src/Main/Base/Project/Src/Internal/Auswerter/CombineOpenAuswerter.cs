// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Xml;


using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.Core
{
	public class CombineOpenAuswerter : IAuswerter
	{
		public bool IsValid(object caller, Condition condition)
		{
			
			bool isCombineOpen = Boolean.Parse(condition.Properties["iscombineopen"]);
			return ProjectService.OpenSolution != null || !isCombineOpen;
		}
	}
}
