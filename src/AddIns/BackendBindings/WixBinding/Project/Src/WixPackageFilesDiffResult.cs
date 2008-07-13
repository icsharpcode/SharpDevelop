// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.WixBinding
{
	public enum WixPackageFilesDiffResultType
	{
		/// <summary>
		/// The file in the Wix document is missing from the file system.
		/// </summary>
		MissingFile,
		
		/// <summary>
		/// The file is new and does not exist in the WixDocument.
		/// </summary>
		NewFile,
		
		/// <summary>
		/// The file is new and does not exist in the WixDocument.
		/// </summary>
		NewDirectory
	}
	
	/// <summary>
	/// A difference between what is defined in the WixDocument and the
	/// files on the local file system.
	/// </summary>
	public class WixPackageFilesDiffResult
	{
		string fileName = String.Empty;
		WixPackageFilesDiffResultType diffType;
		
		public WixPackageFilesDiffResult(string fileName, WixPackageFilesDiffResultType diffType)
		{
			this.fileName = fileName;
			this.diffType = diffType;
		}
		
		public WixPackageFilesDiffResultType DiffType {
			get { return diffType; }
		}
		
		public string FileName {
			get { return fileName; }
		}
	}
}
