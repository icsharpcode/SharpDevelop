// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;

namespace ICSharpCode.WpfDesign.AddIn
{
	public class PropertyDescriptionService : IPropertyDescriptionService
	{
		OpenedFile file;
		
		public PropertyDescriptionService(OpenedFile file)
		{
			if (file == null)
				throw new ArgumentNullException("file");
			this.file = file;
		}
		
		public object GetDescription(DesignItemProperty property)
		{
			IProjectContent pc = MyTypeFinder.GetProjectContent(file);
			if (pc != null) {
				IClass c = pc.GetClassByReflectionName(property.DeclaringType.FullName, true);
				if (c != null) {
					IMember m = DefaultProjectContent.GetMemberByReflectionName(c, property.Name);
					if (m != null)
						return CodeCompletionData.ConvertDocumentation(m.Documentation);
				}
			}
			return null;
		}
	}
}
