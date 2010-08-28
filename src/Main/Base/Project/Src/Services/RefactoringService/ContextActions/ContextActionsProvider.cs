// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Konicek" email="martin.konicek@gmail.com"/>
//     <version>$Revision: $</version>
// </file>
using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Refactoring;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	/// <summary>
	/// Description of ContextActionsProvider.
	/// </summary>
	public abstract class ContextActionsProvider : IContextActionsProvider
	{
		public bool IsVisible { get; set; }
		
		public abstract IEnumerable<IContextAction> GetAvailableActions(EditorContext context);
	}
}
