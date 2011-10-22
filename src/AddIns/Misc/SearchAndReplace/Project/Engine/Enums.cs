// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;

namespace SearchAndReplace
{
	public enum SearchTarget
	{
		[Description("${res:Dialog.NewProject.SearchReplace.LookIn.CurrentDocument}")]
		CurrentDocument,
		[Description("${res:Dialog.NewProject.SearchReplace.LookIn.CurrentSelection}")]
		CurrentSelection,
		[Description("${res:Dialog.NewProject.SearchReplace.LookIn.AllOpenDocuments}")]
		AllOpenFiles,
		[Description("${res:Dialog.NewProject.SearchReplace.LookIn.WholeProject}")]
		WholeProject,
		[Description("${res:Dialog.NewProject.SearchReplace.LookIn.WholeSolution}")]
		WholeSolution,
		Directory
	}
}
