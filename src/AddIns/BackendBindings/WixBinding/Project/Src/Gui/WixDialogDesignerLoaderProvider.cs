// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
