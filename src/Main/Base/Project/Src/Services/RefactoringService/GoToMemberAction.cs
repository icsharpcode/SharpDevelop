// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Konicek" email="martin.konicek@gmail.com"/>
//     <version>$Revision: $</version>
// </file>
using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	/// <summary>
	/// Description of GoToMemberAction.
	/// </summary>
	public class GoToMemberAction : IContextAction
	{
		public IMember Member { get; private set; }
		
		public GoToMemberAction(IMember member)
		{
			if (member == null)
				throw new ArgumentNullException("member");
			this.Member = member;
		}
		
		public void Execute()
		{
			var cu = this.Member.CompilationUnit;
			var region = this.Member.Region;
			if (cu == null || cu.FileName == null || region.IsEmpty)
				return;
			FileService.JumpToFilePosition(cu.FileName, region.BeginLine, region.BeginColumn);
		}
	}
}
