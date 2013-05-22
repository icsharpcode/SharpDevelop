// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using HexEditor.Util;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Workbench;

namespace HexEditor.View
{
	public class HexEditDisplayBinding : IDisplayBinding
	{
		public bool CanCreateContentForFile(FileName fileName)
		{
			return true;
		}
		
		public bool IsPreferredBindingForFile(FileName fileName)
		{
			return false;
		}
		
		public double AutoDetectFileContent(FileName fileName, Stream fileContent, string detectedMimeType)
		{
			return 0.1;
		}

		public IViewContent CreateContentForFile(OpenedFile file)
		{
			return new HexEditView(file);
		}
	}
}
