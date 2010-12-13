// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.Profiler.AddIn.Commands
{
	/// <summary>
	/// Description of GoToDefinition
	/// </summary>
	public class GoToDefinition : DomMenuCommand
	{
		/// <summary>
		/// Starts the command
		/// </summary>
		public override void Run()
		{
			var selectedItem = GetSelectedItems().FirstOrDefault();

			if (selectedItem != null) {
				IClass c = GetClassFromName(selectedItem.FullyQualifiedClassName);
				if (c != null) {
					IEntity member = GetMemberFromName(c, selectedItem.MethodName, selectedItem.Parameters);
					FilePosition position = c.ProjectContent.GetPosition(member ?? c);
					if (!position.IsEmpty && !string.IsNullOrEmpty(position.FileName)) {
						FileService.JumpToFilePosition(position.FileName, position.Line, position.Column);
					}
				}
			}
		}
	}
}
