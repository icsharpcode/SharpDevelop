// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.SharpDevelop.Project.Converter;

namespace ICSharpCode.SharpDevelop.Project.PortableLibrary
{
	public class PortableLibraryProjectBehavior : ProjectBehavior
	{
		public readonly static TargetFramework PortableTargetFramework = new TargetFramework("v4.0Portable", "Portable Library") {
			MinimumMSBuildVersion = new Version(4, 0)
		};
		
		public override IEnumerable<CompilerVersion> GetAvailableCompilerVersions()
		{
			return base.GetAvailableCompilerVersions().Where(c => c.MSBuildVersion == new Version(4, 0));
		}
		
		public override IEnumerable<TargetFramework> GetAvailableTargetFrameworks()
		{
			return new[] { PortableTargetFramework };
		}
		
		public override TargetFramework CurrentTargetFramework {
			get { return PortableTargetFramework; }
		}
		
		public override void UpgradeProject(CompilerVersion newVersion, TargetFramework newFramework)
		{
			// can't upgrade portable libraries
		}
	}
}
