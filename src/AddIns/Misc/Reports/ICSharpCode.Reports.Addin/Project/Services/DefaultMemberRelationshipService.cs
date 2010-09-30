// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel.Design.Serialization;

namespace ICSharpCode.Reports.Addin
{
	/// <summary>
	/// Description of DefaultMemberRelationshipService.
	/// </summary>
	public class DefaultMemberRelationshipService:MemberRelationshipService
	{
		
		public DefaultMemberRelationshipService()
		{
		}
		public override bool SupportsRelationship(MemberRelationship source, MemberRelationship relationship)
		{
			return true;
		}
		protected override MemberRelationship GetRelationship(MemberRelationship source)
		{
			return base.GetRelationship(source);
		}
	}
}