// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
			NavigationService.NavigateTo(this.Class);
		}
	}
}
