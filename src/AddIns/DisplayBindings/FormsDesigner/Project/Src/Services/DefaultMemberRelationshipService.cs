// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Christian Hornung" email="chhornung@googlemail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel.Design.Serialization;

using ICSharpCode.Core;

//#define WFDESIGN_LOG_MEMBERRELATIONSHIPSERVICE

namespace ICSharpCode.FormsDesigner.Services
{
	public class DefaultMemberRelationshipService : MemberRelationshipService
	{
		public DefaultMemberRelationshipService()
		{
		}
		
		public override bool SupportsRelationship(MemberRelationship source, MemberRelationship relationship)
		{
			#if WFDESIGN_LOG_MEMBERRELATIONSHIPSERVICE
			LoggingService.Debug("MemberRelationshipService: SupportsRelationship called, source=" + ToString(source) + ", relationship=" + ToString(relationship));
			#endif
			return true;
		}
		
		#if WFDESIGN_LOG_MEMBERRELATIONSHIPSERVICE
		protected override MemberRelationship GetRelationship(MemberRelationship source)
		{
			LoggingService.Debug("MemberRelationshipService: GetRelationship called, source=" + ToString(source));
			var mrs = base.GetRelationship(source);
			LoggingService.Debug("MemberRelationshipService: -> returning " + ToString(mrs));
			return mrs;
		}
		
		static string ToString(MemberRelationship mrs)
		{
			return "[MR: IsEmpty=" + mrs.IsEmpty + ", Owner=[" + (mrs.Owner == null ? "<null>" : mrs.Owner.ToString()) + "], Member=[" + (mrs.Member == null ? "<null>" : mrs.Member.Name) + "]]";
		}
		#endif
	}
}
