// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel.Design.Serialization;

//#define WFDESIGN_LOG_MEMBERRELATIONSHIPSERVICE

namespace ICSharpCode.FormsDesigner.Services
{
	public class DefaultMemberRelationshipService : MemberRelationshipService
	{
		IServiceProvider services;
		
		public DefaultMemberRelationshipService(IServiceProvider services)
		{
			this.services = services;
		}
		
		public override bool SupportsRelationship(MemberRelationship source, MemberRelationship relationship)
		{
			#if WFDESIGN_LOG_MEMBERRELATIONSHIPSERVICE
			((IFormsDesignerLoggingService)services.GetService(typeof(IFormsDesignerLoggingService))).Debug("MemberRelationshipService: SupportsRelationship called, source=" + ToString(source) + ", relationship=" + ToString(relationship));
			#endif
			return true;
		}
		
		#if WFDESIGN_LOG_MEMBERRELATIONSHIPSERVICE
		protected override MemberRelationship GetRelationship(MemberRelationship source)
		{
			((IFormsDesignerLoggingService)services.GetService(typeof(IFormsDesignerLoggingService))).Debug("MemberRelationshipService: GetRelationship called, source=" + ToString(source));
			var mrs = base.GetRelationship(source);
			((IFormsDesignerLoggingService)services.GetService(typeof(IFormsDesignerLoggingService))).Debug("MemberRelationshipService: -> returning " + ToString(mrs));
			return mrs;
		}
		
		static string ToString(MemberRelationship mrs)
		{
			return "[MR: IsEmpty=" + mrs.IsEmpty + ", Owner=[" + (mrs.Owner == null ? "<null>" : mrs.Owner.ToString()) + "], Member=[" + (mrs.Member == null ? "<null>" : mrs.Member.Name) + "]]";
		}
		#endif
	}
}
