// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using HexEditor.Util;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using System.IO;

namespace HexEditor.View
{
	public class HexEditDisplayBinding : IDisplayBinding
	{
		public bool CanCreateContentForFile(string fileName)
		{
			return true;
		}
		
		public bool IsPreferredBindingForFile(string fileName)
		{
			return false;
		}
		
		public double AutoDetectFileContent(string fileName, Stream fileContent, string detectedMimeType)
		{
			return 0.1;
		}

		public IViewContent CreateContentForFile(OpenedFile file)
		{
			return new HexEditView(file);
		}
	}
}
