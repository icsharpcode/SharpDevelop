// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class SolutionGlobals : global::EnvDTE.Globals
	{
		SolutionExtensibilityGlobals extensibilityGlobals;
		SolutionExtensibilityGlobalsPersistence extensibilityGlobalsPersistence;
		
		public SolutionGlobals(Solution solution)
		{
			this.extensibilityGlobals = new SolutionExtensibilityGlobals(solution);
			this.extensibilityGlobalsPersistence = new SolutionExtensibilityGlobalsPersistence(extensibilityGlobals);
		}
		
		protected override object GetVariableValue(string name)
		{
			return extensibilityGlobals[name];
		}
		
		protected override void SetVariableValue(string name, object value)
		{
			extensibilityGlobals[name] = value;
		}
		
		protected override bool GetVariablePersists(string name)
		{
			return extensibilityGlobalsPersistence[name];
		}
		
		protected override void SetVariablePersists(string name, bool value)
		{
			extensibilityGlobalsPersistence[name] = value;
		}
		
		protected override bool GetVariableExists(string name)
		{
			return extensibilityGlobals.ItemExists(name);
		}
	}
}
