// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel.Design.Serialization;
using ICSharpCode.FormsDesigner;

namespace ICSharpCode.WixBinding
{
	public class WixDialogDesignerLoaderProvider : IDesignerLoaderProvider
	{
		IWixDialogDesigner designer;
		
		public WixDialogDesignerLoaderProvider()
		{
		}
		
		public DesignerLoader CreateLoader(IDesignerGenerator generator)
		{
			return new WixDialogDesignerLoader(designer, generator as IWixDialogDesignerGenerator);
		}
		
		/// <summary>
		/// Gets or sets the designer that the loader provider should use.
		/// </summary>
		public IWixDialogDesigner Designer {
			get {
				return designer;
			}
			set {
				designer = value;
			}
		}
	}
}
