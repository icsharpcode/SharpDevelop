// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Parser;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	public class GoToMemberAction : IContextAction
	{
		public string Title { get; private set; }
		public IMember Member { get; private set; }
		
		public GoToMemberAction(IMember member, IAmbience ambience)
		{
			if (ambience == null)
				throw new ArgumentNullException("ambience");
			if (member == null)
				throw new ArgumentNullException("member");
			this.Member = member;
			this.Title = ambience.ConvertEntity(member);
		}
		
		public void Execute(EditorContext context)
		{
			NavigationService.NavigateTo(this.Member);
		}
	}
}
