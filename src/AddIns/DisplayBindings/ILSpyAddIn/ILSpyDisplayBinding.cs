// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.ILSpyAddIn
{
	public class ILSpyDisplayBinding : IDisplayBinding
	{
		public bool IsPreferredBindingForFile(FileName fileName)
		{
			return fileName.ToString().StartsWith("ilspy://", StringComparison.OrdinalIgnoreCase);
		}
		
		public bool CanCreateContentForFile(FileName fileName)
		{
			return fileName.ToString().StartsWith("ilspy://", StringComparison.OrdinalIgnoreCase);
		}
		
		public double AutoDetectFileContent(FileName fileName, Stream fileContent, string detectedMimeType)
		{
			return 1;
		}
		
		public IViewContent CreateContentForFile(OpenedFile file)
		{
			return new DecompiledViewContent(DecompiledTypeReference.FromFileName(file.FileName), "");
		}
	}
}
