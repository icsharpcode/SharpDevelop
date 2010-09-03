// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.CodeCoverage
{
	public class PartCoverSettingsFactory
	{
		IFileSystem fileSystem;
		
		public PartCoverSettingsFactory(IFileSystem fileSystem)
		{
			this.fileSystem = fileSystem;
		}
		
		public PartCoverSettingsFactory()
			: this(new FileSystem())
		{
		}
		
		public PartCoverSettings CreatePartCoverSettings(IProject project)
		{
			string fileName = PartCoverSettings.GetFileName(project);
			if (fileSystem.FileExists(fileName)) {
				return CreatePartCoverSettingsFromFile(fileName);
			}
			return new PartCoverSettings();
		}
		
		PartCoverSettings CreatePartCoverSettingsFromFile(string fileName)
		{
			TextReader reader = fileSystem.CreateTextReader(fileName);
			return new PartCoverSettings(reader);
		}
	}
}
