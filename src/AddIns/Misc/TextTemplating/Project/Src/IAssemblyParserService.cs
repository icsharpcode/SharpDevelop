// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.TextTemplating
{
	public interface IAssemblyParserService
	{
		IReflectionProjectContent GetReflectionProjectContentForReference(ReferenceProjectItem item);
	}
}
