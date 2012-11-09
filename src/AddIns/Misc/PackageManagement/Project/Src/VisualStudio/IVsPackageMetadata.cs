// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NuGet;

namespace NuGet.VisualStudio
{
	public interface IVsPackageMetadata
	{
		string Id { get; }
		SemanticVersion Version { get; }
		string InstallPath { get; }
	}
}
