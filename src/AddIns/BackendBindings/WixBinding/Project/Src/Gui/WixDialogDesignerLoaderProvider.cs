// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using ICSharpCode.FormsDesigner;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.WixBinding
{
	public class WixDialogDesignerLoaderProvider : IDesignerLoaderProvider
	{
		public DesignerLoader CreateLoader(FormsDesignerViewContent viewContent)
		{
			return new WixDialogDesignerLoader((IWixDialogDesigner)viewContent);
		}

		public IReadOnlyList<OpenedFile> GetSourceFiles(FormsDesignerViewContent viewContent, out OpenedFile designerCodeFile)
		{
			designerCodeFile = viewContent.PrimaryFile;
			return new[] { viewContent.PrimaryFile };
		}
	}
}
