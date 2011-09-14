// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;

namespace ICSharpCode.FormsDesigner.Services
{

	
	public interface ITypeLocator
	{
		AssemblyInfo LocateType(string name, out AssemblyInfo[] referencedAssemblies);
	}
	
	public interface IGacWrapper
	{
		bool IsGacAssembly(string path);
	}
	
	public interface IImageResourceEditorDialogWrapper
	{
		object GetValue(IProjectResourceInfo projectResource, object value, IProjectResourceService prs, Type propertyType, string propertyName, IWindowsFormsEditorService edsvc, IDictionaryService dictService);
	}
}
