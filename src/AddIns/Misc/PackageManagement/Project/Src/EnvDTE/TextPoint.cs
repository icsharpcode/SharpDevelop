// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class TextPoint : MarshalByRefObject, global::EnvDTE.TextPoint
	{
		internal TextPoint(FilePosition filePosition, IDocumentLoader documentLoader)
		{
			this.FilePosition = filePosition;
			this.DocumentLoader = documentLoader;
		}
		
		protected IDocumentLoader DocumentLoader { get; private set; }
		protected FilePosition FilePosition { get; private set; }
		
		public int LineCharOffset {
			get { return FilePosition.Column; }
		}
		
		public int Line {
			get { return FilePosition.Line; }
		}
		
		public global::EnvDTE.EditPoint CreateEditPoint()
		{
			return new EditPoint(FilePosition, DocumentLoader);
		}
		
		internal static TextPoint CreateStartPoint(FilePosition position, IDocumentLoader documentLoader)
		{
			return new TextPoint(position, documentLoader);
		}
		
		internal static TextPoint CreateEndPoint(FilePosition position, IDocumentLoader documentLoader)
		{
			return new TextPoint(position, documentLoader);
		}
	}
}
