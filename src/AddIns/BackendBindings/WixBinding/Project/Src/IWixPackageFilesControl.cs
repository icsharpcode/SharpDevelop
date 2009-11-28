// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
