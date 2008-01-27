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
				string fullName = property.DeclaringType.FullName + "." + property.Name;
				IEntity dec = pc.GetElement(fullName);
				if (dec != null)
					return CodeCompletionData.GetDocumentation(dec.Documentation);
				foreach (IProjectContent rpc in pc.ReferencedContents) {
					dec = rpc.GetElement(fullName);
					if (dec != null)
						return CodeCompletionData.GetDocumentation(dec.Documentation);
				}
			}
			return null;
		}
	}
}
