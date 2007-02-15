// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Russell Wilkins" email=""/>
//     <version>$Revision$</version>
// </file>

#region Using
using System;
using System.IO;
using System.Reflection;
using System.ComponentModel.Design;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Dom;
using System.Workflow.ComponentModel.Compiler;
#endregion

namespace WorkflowDesigner
{
	/// <summary>
	/// Description of TypeResolutionService.
	/// </summary>
	public class TypeResolutionService : ITypeResolutionService, IServiceProvider
	{
		IProject project;
		
		
		#region Constructors
		public TypeResolutionService(IProject project, IServiceProvider provider)
		{
			this.project = project;
			this.provider = provider;
		}
		#endregion
		
		#region IServiceProvider implementation
		IServiceProvider provider;
		public object GetService(Type serviceType)
		{
			return provider.GetService(serviceType);
		}
		#endregion
		
		#region ITypeResolutionService implementation
		public Assembly GetAssembly(AssemblyName name)
		{
			return GetAssembly(name, true);
		}
		
		public Assembly GetAssembly(AssemblyName name, bool throwOnError)
		{
			throw new NotImplementedException();
		}
		
		public Type GetType(string name)
		{
			return GetType(name, false);
		}
		
		public Type GetType(string name, bool throwOnError)
		{
			return GetType(name, throwOnError, false);
			
		}
		
		public Type GetType(string name, bool throwOnError, bool ignoreCase)
		{
			Type type = null;

			// Ignoring versions numbers on types.
			string[] splitName = name.Split(',');
			string typeName     = splitName[0].Replace('+','.');
			
			// Check for the type ourselves in the projects referenced assemblies
			// as the System.Workflow.ComponentModel.Compiler.TypeProvider does
			// not seem find private types in referenced assemblies!
			TypeProvider typeProvider = provider.GetService(typeof(ITypeProvider)) as TypeProvider;
			if (typeProvider != null) {
				foreach (Assembly asm in typeProvider.ReferencedAssemblies){
					foreach (Module module in asm.GetModules()){
						type = module.GetType(typeName, throwOnError, ignoreCase);
						if (type != null) 
							return type;
					}
				}
			}
			
			if (type == null)
				type = typeProvider.GetType(typeName, throwOnError);
			
			// TODO: Need to check current project see if we can find it!

			if (type == null) {
				LoggingService.WarnFormatted("TypeResolutionService failed to find type {0}", typeName);
				if (throwOnError)
					throw new TypeLoadException(name + " not found by TypeResolutionService");
			}
			return type;
			
		}
		
		public void ReferenceAssembly(AssemblyName name)
		{
			// Check if assembly already exist in project, add it if not.
			TypeProvider typeProvider = provider.GetService(typeof(ITypeProvider)) as TypeProvider;
			if (typeProvider == null)
				return;

			foreach (Assembly asm in typeProvider.ReferencedAssemblies){
				if (asm.FullName == name.FullName)
					return;
			}
			
			LoggingService.DebugFormatted("TypeResolutionService.ReferenceAssembly {0}", name);
			
			// TODO: Not in project so add the reference.
//			IProject project = ProjectService.CurrentProject;
//			if (project != null) {
//				ReferenceProjectItem rpi = new ReferenceProjectItem(project, name.Name);
//				rpi.Include = name.Name;
//				ProjectService.AddProjectItem(project, rpi);
//				project.Save();
//			}
			
			throw new NotImplementedException();
		}
		
		public string GetPathOfAssembly(AssemblyName name)
		{
			throw new NotImplementedException();
		}
		#endregion
		
	}
}
