// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Refactoring;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using System.Windows.Forms;

namespace ICSharpCode.FormsDesigner.Services
{
	public class TypeDiscoveryService : ITypeDiscoveryService
	{
		public TypeDiscoveryService()
		{
		}
		
		/// <summary>
		/// Returns the list of available types.
		/// </summary>
		/// <param name="baseType">The base type to match.  Can be null.</param>
		/// <param name="excludeGlobalTypes">Determines whether types
		/// from all referenced assemblies should be checked.</param>
		public ICollection GetTypes(Type baseType, bool excludeGlobalTypes)
		{
			List<Type> types = new List<Type>();
			if (baseType != null) {
				LoggingService.Debug("TypeDiscoveryService.GetTypes baseType=" + baseType.FullName);
				LoggingService.Debug("TypeDiscoveryService.GetTypes excludeGlobalTypes=" + excludeGlobalTypes.ToString());
				//seek in all assemblies
				//allow to work designers like columns editor in datagridview
				foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies()) {
					AddDerivedTypes(baseType, asm, types);
				}
				
				// TODO - Don't look in all assemblies.
				// Should use the current project and its referenced assemblies
				// as well as System.Windows.Forms.
			}
			
			return types;
		}
		
		/// <summary>
		/// Gets the types derived from baseType from the assembly and adds them to the list.
		/// </summary>
		void AddDerivedTypes(Type baseType, Assembly assembly, IList<Type> list)
		{
			foreach (Type t in assembly.GetExportedTypes()) {
				if (t.IsSubclassOf(baseType)) {
					LoggingService.Debug("TypeDiscoveryService.  Adding type=" + t.FullName);
					list.Add(t);
				}
			}
		}
	}
}
