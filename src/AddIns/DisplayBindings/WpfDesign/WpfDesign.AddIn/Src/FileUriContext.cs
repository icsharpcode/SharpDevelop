// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

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
