// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Text;

namespace ICSharpCode.SharpDevelop.Workbench
{
	/// <summary>
	/// Extra information about a TextDocument that was loaded from an OpenedFile.
	/// </summary>
	public class DocumentFileModelInfo
	{
		public DocumentFileModelInfo()
		{
			this.Encoding = SD.FileService.DefaultFileEncoding;
		}
		
		/// <summary>
		/// Gets whether the model is (re)loading.
		/// </summary>
		internal bool isLoading;
		
		/// <summary>
		/// Gets whether the model is stale.
		/// </summary>
		public bool IsStale { get; internal set; }
		
		/// <summary>
		/// Gets the encoding of the model.
		/// </summary>
		public Encoding Encoding { get; set; }
	}
}
