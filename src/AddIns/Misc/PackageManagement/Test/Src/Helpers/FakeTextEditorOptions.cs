// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.EnvDTE;

namespace PackageManagement.Tests.Helpers
{
	public class FakeTextEditorOptions : ITextEditorOptions
	{
		public double FontSize { get; set; }
	}
}
