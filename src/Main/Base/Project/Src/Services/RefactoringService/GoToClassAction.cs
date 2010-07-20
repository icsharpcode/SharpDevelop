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
	/// Description of GoToClassAction.
	/// </summary>
	public class GoToClassAction : IContextAction
	{
		public string Title { get; private set; }
		public IClass Class { get; private set; }
		
		public GoToClassAction(IClass c, IAmbience ambience)
		{
			if (ambience == null)
				throw new ArgumentNullException("ambience");
			if (c == null)
				throw new ArgumentNullException("c");
			this.Class = c;
			this.Title = ambience.Convert(c);
		}
		
		public void Execute()
		{
			var cu = this.Class.CompilationUnit;
			var region = this.Class.Region;
			if (cu == null || cu.FileName == null || region == null || region.IsEmpty)
				return;
			FileService.JumpToFilePosition(cu.FileName, region.BeginLine, region.BeginColumn);
		}
	}
}
