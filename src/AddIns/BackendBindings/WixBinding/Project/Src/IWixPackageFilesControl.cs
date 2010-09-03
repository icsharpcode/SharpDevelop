// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.WixBinding
{
	public interface IWixPackageFilesControl : IDisposable
	{
		event EventHandler DirtyChanged;
		bool IsDirty { get; set; }
		
		void Save();
		
		void AddElement(string name);
		void RemoveSelectedElement();
		
		void AddFiles();
		void AddDirectory();
		void ShowFiles(WixProject project, ITextFileReader fileReader, IWixDocumentWriter documentWriter);
		
		void CalculateDiff();
		bool IsDiffVisible { get; set; }
		
		WixDocument Document { get; }
	}
}
