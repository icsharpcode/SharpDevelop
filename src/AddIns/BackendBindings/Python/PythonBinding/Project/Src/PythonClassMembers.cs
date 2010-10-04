// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PythonBinding
{
	public class PythonClassMembers
	{
		List<IMember> members;
		
		public PythonClassMembers(IClass declaringType)
		{
			members = GetMembers(declaringType);
		}
		
		public IMember FindMember(string memberName)
		{
			foreach (IMember member in members) {
				if (member.Name == memberName) {
					return member;
				}
			}
			return null;
		}
		
		List<IMember> GetMembers(IClass declaringType)
		{
			List<IMember> members = new List<IMember>();
			members.AddRange(declaringType.Events);
			members.AddRange(declaringType.Fields);
			members.AddRange(declaringType.Properties);
			members.AddRange(declaringType.Methods);
			return members;
		}
	}
}
