// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace ICSharpCode.PythonBinding
{
	public class PythonImageListComponent : PythonDesignerComponent
	{
		public PythonImageListComponent(IComponent component) : this(null, component)
		{
		}
		
		public PythonImageListComponent(PythonDesignerComponent parent, IComponent component) 
			: base(parent, component)
		{
		}
		
		public override void AppendComponent(PythonCodeBuilder codeBuilder)
		{
			base.AppendComponent(codeBuilder);
			
			// Add image list keys.
			ImageList imageList = (ImageList)Component;
			for (int i = 0; i < imageList.Images.Keys.Count; ++i) {
				codeBuilder.AppendIndented(GetPropertyOwnerName());
				codeBuilder.Append(".Images.SetKeyName(");
				codeBuilder.Append(i.ToString());
				codeBuilder.Append(", \"");
				codeBuilder.Append(imageList.Images.Keys[i]);
				codeBuilder.Append("\")");
				codeBuilder.AppendLine();
			}
		}
	}
}
