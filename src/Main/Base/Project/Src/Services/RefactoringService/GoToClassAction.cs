// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	public class GoToClassAction : IContextAction
	{
		public string Title { get; private set; }
		public ITypeDefinition Class { get; private set; }
		
		public GoToClassAction(ITypeDefinition c, string title)
		{
			if (c == null)
				throw new ArgumentNullException("c");
			if (title == null)
				throw new ArgumentNullException("title");
			this.Class = c;
			this.Title = title;
		}
		
		public void Execute(EditorContext context)
		{
			NavigationService.NavigateTo(this.Class);
		}
	}
}
