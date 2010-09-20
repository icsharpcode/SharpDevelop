// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Security.Permissions;
using ICSharpCode.Scripting;

namespace ICSharpCode.RubyBinding
{
	/// <summary>
	/// Loads the form or control's code so the forms designer can
	/// display it.
	/// </summary>
	[PermissionSet(SecurityAction.InheritanceDemand, Name="FullTrust")]
	[PermissionSet(SecurityAction.LinkDemand, Name="FullTrust")]
	public class RubyDesignerLoader : ScriptingDesignerLoader
	{
		public RubyDesignerLoader(IScriptingDesignerGenerator generator)
			: base(generator)
		{
		}
		
		protected override IComponentWalker CreateComponentWalker(IComponentCreator componentCreator)
		{
			return new RubyComponentWalker(componentCreator);
		}
	}
}
