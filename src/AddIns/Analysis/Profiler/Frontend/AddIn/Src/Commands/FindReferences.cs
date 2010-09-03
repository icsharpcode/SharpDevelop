// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Refactoring;

namespace ICSharpCode.Profiler.AddIn.Commands
{
	/// <summary>
	/// Description of FindReferences.
	/// </summary>
	public class FindReferences : DomMenuCommand
	{
		public override void Run()
		{
			var selectedItem = GetSelectedItems().FirstOrDefault();

			if (selectedItem != null) {
				IClass c = GetClassFromName(selectedItem.FullyQualifiedClassName);
				if (c != null) {
					IMember member = GetMemberFromName(c, selectedItem.MethodName, selectedItem.Parameters);
					string memberName = member.DeclaringType.Name + "." + member.Name;
					using (AsynchronousWaitDialog monitor = AsynchronousWaitDialog.ShowWaitDialog("${res:SharpDevelop.Refactoring.FindReferences}"))
					{
						FindReferencesAndRenameHelper.ShowAsSearchResults(StringParser.Parse("${res:SharpDevelop.Refactoring.ReferencesTo}",
						                                                                     new string[,] {{ "Name", memberName }}),
						                                                  RefactoringService.FindReferences(member, monitor));
					}
				}
			}
		}
	}
}
