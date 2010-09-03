// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Windows.Markup;

using ICSharpCode.SharpDevelop;

namespace ICSharpCode.WpfDesign.AddIn
{
	/// <summary>
	/// Used to support loading Image.ImageSource.
	/// </summary>
	public class FileUriContext : IUriContext
	{
		OpenedFile file;
		
		public FileUriContext(OpenedFile file)
		{
			if (file == null)
				throw new ArgumentNullException("file");
			this.file = file;
		}
		
		public Uri BaseUri {
			get {
				return new Uri(file.FileName);
			}
			set {
				throw new NotSupportedException();
			}
		}
	}
}
