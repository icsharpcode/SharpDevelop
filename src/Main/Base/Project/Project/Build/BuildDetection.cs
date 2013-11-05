// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Project
{
	public enum BuildDetection
	{
		[Description("${res:Dialog.Options.IDEOptions.ProjectAndSolutionOptions.WhenRunning.DoNotBuild}")]
		DoNotBuild,
		[Description("${res:Dialog.Options.IDEOptions.ProjectAndSolutionOptions.WhenRunning.BuildOnlyModified}")]
		BuildOnlyModified,
		[Description("${res:Dialog.Options.IDEOptions.ProjectAndSolutionOptions.WhenRunning.BuildModifiedAndDependent}")]
		BuildModifiedAndDependent,
		[Description("${res:Dialog.Options.IDEOptions.ProjectAndSolutionOptions.WhenRunning.RegularBuild}")]
		RegularBuild
	}
}
