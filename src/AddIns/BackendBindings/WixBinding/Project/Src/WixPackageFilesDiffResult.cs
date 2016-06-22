// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
