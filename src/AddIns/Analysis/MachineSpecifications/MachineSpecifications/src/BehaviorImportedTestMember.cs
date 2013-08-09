// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.MachineSpecifications
{
	/// <summary>
	/// Description of BehaviorImportedClass.
	/// </summary>
	public class BehaviorImportedTestMember : BaseTestMember
	{
		public BehaviorImportedTestMember(IClass testClass, IMember behaviorMember)
			: base(testClass, behaviorMember, behaviorMember.Name)
		{
		}
	}
}
