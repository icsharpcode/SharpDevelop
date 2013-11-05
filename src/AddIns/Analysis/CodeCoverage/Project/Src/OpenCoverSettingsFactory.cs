// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.CodeCoverage
{
	public class OpenCoverSettingsFactory
	{
		IFileSystem fileSystem;
		
		public OpenCoverSettingsFactory(IFileSystem fileSystem)
		{
			this.fileSystem = fileSystem;
		}
		
		public OpenCoverSettingsFactory()
			: this(SD.FileSystem)
		{
		}
		
		public OpenCoverSettings CreateOpenCoverSettings(IProject project)
		{
			string fileName = OpenCoverSettings.GetFileName(project);
			if (fileSystem.FileExists(FileName.Create(fileName))) {
				return CreateOpenCoverSettingsFromFile(fileName);
			}
			return new OpenCoverSettings();
		}
		
		OpenCoverSettings CreateOpenCoverSettingsFromFile(string fileName)
		{
			TextReader reader = fileSystem.OpenText(FileName.Create(fileName));
			return new OpenCoverSettings(reader);
		}
	}
}
