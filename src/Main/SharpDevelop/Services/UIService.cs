// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop
{
	class UIService : IUIService
	{
		public void ShowSolutionConfigurationEditorDialog(ISolution solution)
		{
			using (SolutionConfigurationEditor sce = new SolutionConfigurationEditor(solution)) {
				sce.ShowDialog(SD.WinForms.MainWin32Window);
				if (solution.IsDirty)
					solution.Save();
				foreach (IProject project in solution.Projects) {
					project.Save();
				}
			}
		}
	}
}
