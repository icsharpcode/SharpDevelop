// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

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
            	 // For attached Properties
                if (property.DependencyFullName != null && property.Name.Contains(".")) {
                    IClass c = pc.GetClassByReflectionName(property.DependencyProperty.OwnerType.FullName, true);
                    if (c != null) {
                        IMember m = DefaultProjectContent.GetMemberByReflectionName(c, property.DependencyProperty.Name + "Property");
                        if (m != null)
                            return CodeCompletionItem.ConvertDocumentation(m.Documentation);
                    }
                } else {
                    IClass c = pc.GetClassByReflectionName(property.DeclaringType.FullName, true);
                    if (c != null) {
                        IMember m = DefaultProjectContent.GetMemberByReflectionName(c, property.Name);
                        if (m != null)
                            return CodeCompletionItem.ConvertDocumentation(m.Documentation);
                    }
                }
            }
            return null;
        }
    }
}
