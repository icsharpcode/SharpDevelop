// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Editor.Commands
{
	public class GoToDefinition : ResolveResultMenuCommand
	{
		public override void Run(ResolveResult symbol)
		{
			if (symbol == null)
				return;
			
			DomRegion pos = symbol.GetDefinitionRegion();
			if (string.IsNullOrEmpty(pos.FileName)) {
				IEntity entity = GetEntity(symbol);
				if (entity != null) {
					NavigationService.NavigateTo(entity);
				}
			} else {
				try {
					if (pos.Begin.IsEmpty)
						FileService.OpenFile(pos.FileName);
					else
						FileService.JumpToFilePosition(pos.FileName, pos.BeginLine, pos.BeginColumn);
				} catch (Exception ex) {
					MessageService.ShowException(ex, "Error jumping to '" + pos.FileName + "'.");
				}
			}
		}
	}
}
