// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.MachineSpecifications
{
	public class BehaviorImportedTestMember : MSpecTestMember
	{
		IMember parent;
		
		public BehaviorImportedTestMember(
			MSpecTestProject parentProject,
			IMember parent,
			IMember behavior)
			: base(parentProject, behavior)
		{
			this.parent = parent;
		}
		
		public override string GetTypeName()
		{
			return parent.DeclaringTypeDefinition.FullName;
		}
	}
}
