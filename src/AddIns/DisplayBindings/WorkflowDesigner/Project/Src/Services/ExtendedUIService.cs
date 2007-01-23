// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Russell Wilkins" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Workflow.ComponentModel.Design;

namespace WorkflowDesigner
{
	/// <summary>
	/// Description of ExtendedUIService.
	/// </summary>
	public class ExtendedUIService : IExtendedUIService
	{
		public ExtendedUIService()
		{
		}
		
		public System.Windows.Forms.DialogResult AddWebReference(out Uri url, out Type proxyClass)
		{
			throw new NotImplementedException();
		}
		
		public Uri GetUrlForProxyClass(Type proxyClass)
		{
			throw new NotImplementedException();
		}
		
		public Type GetProxyClassForUrl(Uri url)
		{
			throw new NotImplementedException();
		}
		
		public void AddDesignerActions(DesignerAction[] actions)
		{
			//throw new NotImplementedException();
		}
		
		public void RemoveDesignerActions()
		{
			//throw new NotImplementedException();
		}
		
		public bool NavigateToProperty(string propName)
		{
			throw new NotImplementedException();
		}
		
		public System.ComponentModel.ITypeDescriptorContext GetSelectedPropertyContext()
		{
			throw new NotImplementedException();
		}
		
		public void ShowToolsOptions()
		{
			throw new NotImplementedException();
		}
		
		public System.Collections.Generic.Dictionary<string, Type> GetXsdProjectItemsInfo()
		{
			throw new NotImplementedException();
		}
		
		public void AddAssemblyReference(System.Reflection.AssemblyName assemblyName)
		{
			throw new NotImplementedException();
		}
		
	}
}
