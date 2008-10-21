// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision: 3506 $</version>
// </file>

using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.WpfDesign.Designer.XamlBackend;

namespace ICSharpCode.WpfDesign.AddIn
{
	public class PropertyDescriptionService : IPropertyDescriptionService
	{
		public object GetDescription(DesignItemProperty property)
		{
			var p = property as XamlDesignItemProperty;
			var m = XamlMapper.GetDomMember(p.XamlProperty.Member);
			return CodeCompletionData.GetDocumentation(m.Documentation);
		}
	}
}
