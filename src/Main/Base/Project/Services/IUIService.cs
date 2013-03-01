// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// SharpDevelop UI service.
	/// 
	/// This service provides methods for accessing the dialogs and other UI element built into SharpDevelop.
	/// </summary>
	public interface IUIService
	{
		void ShowSolutionConfigurationEditorDialog(ISolution solution);
	}
}
