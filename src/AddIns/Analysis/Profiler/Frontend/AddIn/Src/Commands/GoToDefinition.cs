// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;

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
				ITypeDefinition c = GetClassFromName(selectedItem.FullyQualifiedClassName);
				if (c != null) {
					IMember member = GetMemberFromName(c, selectedItem.MethodName, selectedItem.Parameters);
					if (!member.Region.IsEmpty && !string.IsNullOrEmpty(member.Region.FileName)) {
						FileName fn = new FileName(member.Region.FileName);
						SD.FileService.JumpToFilePosition(fn, member.Region.BeginLine, member.Region.BeginColumn);
					}
				}
			}
		}
	}
}
