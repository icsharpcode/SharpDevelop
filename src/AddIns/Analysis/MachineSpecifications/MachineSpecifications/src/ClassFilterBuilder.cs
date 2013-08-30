// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.MachineSpecifications
{
	/// <summary>
	/// Creates class list filter for tests which should be run.
	/// </summary>
	public class ClassFilterBuilder
	{
		public IList<string> BuildFilterFor(SelectedTests tests, IProjectContent @using) {
			var projectContent = @using;
			
			var filter = new List<string>();
			if (tests.Class != null)
				filter.Add(tests.Class.DotNetName);
			if (tests.NamespaceFilter != null)
				foreach (var projectClass in projectContent.Classes)
					if (projectClass.FullyQualifiedName.StartsWith(tests.NamespaceFilter + "."))
						Add(projectClass, to: filter);
			
			return filter;
		}

		static void Add(IClass @class, IList<string> to)
		{
			var list = to;
			to.Add(@class.DotNetName);
			foreach (var innerClass in @class.InnerClasses)
				Add(innerClass, to: list);
		}
	}
}
