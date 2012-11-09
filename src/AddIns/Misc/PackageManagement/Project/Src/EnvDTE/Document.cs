// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class Document : MarshalByRefObject, global::EnvDTE.Document
	{
		IViewContent view;
		
		public Document(string fileName, IViewContent view)
		{
			this.FullName = fileName;
			this.view = view;
		}
		
		public virtual bool Saved {
			get { return !view.IsDirty; }
			set { view.PrimaryFile.IsDirty = !value; }
		}
		
		public string FullName { get; private set; }
		
		public Object Object(string modelKind)
		{
			throw new NotImplementedException();
		}
	}
}
