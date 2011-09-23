/*
 * Created by SharpDevelop.
 * User: trecio
 * Date: 2011-09-23
 * Time: 19:40
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
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
        		filter.Add(tests.Class.FullyQualifiedName);
            if (tests.NamespaceFilter != null)
                foreach (var projectClass in projectContent.Classes)
                    if (projectClass.FullyQualifiedName.StartsWith(tests.NamespaceFilter + "."))
                        filter.Add(projectClass.FullyQualifiedName);
            
            return filter;
		}
	}
}
