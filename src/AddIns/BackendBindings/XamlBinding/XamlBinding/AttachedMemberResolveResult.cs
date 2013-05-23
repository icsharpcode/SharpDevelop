// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.XamlBinding
{
	/// <summary>
	/// Description of AttachedMemberResolveResult.
	/// </summary>
	public class AttachedMemberResolveResult : MemberResolveResult
	{
		IMember attachedMember;
		
		public AttachedMemberResolveResult(ResolveResult targetResult, IMember baseMember, IMember attachedMember)
			: base(targetResult, baseMember)
		{
			this.attachedMember = attachedMember;
		}
		
		public IMember UnderlyingMember {
			get { return base.Member; }
		}
		
		new public IMember Member {
			get { return attachedMember; }
		}
	}
}
