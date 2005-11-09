// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
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

namespace ICSharpCode.FormDesigner.Services
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
				
				// TODO - Look in more than just System.Windows.Forms.  
				// Should use the current project and its referenced assemblies
				// as well as System.Windows.Forms.
				types.AddRange(GetDerivedTypesFromWindowsForms(baseType));	
			}
			
			return types;
		}
		
		/// <summary>
		/// Gets the derived types from the System.Windows.Forms assembly.
		/// </summary>
		IList<Type> GetDerivedTypesFromWindowsForms(Type baseType)
		{
			List<Type> types = new List<Type>();
			
			Assembly asm = typeof(System.Windows.Forms.Control).Assembly;
			
			foreach (Module m in asm.GetModules()) {
				foreach (Type t in m.GetTypes()) {
					if (t.IsSubclassOf(baseType)) {
						LoggingService.Debug("TypeDiscoveryService.  Adding type=" + t.FullName);
						types.Add(t);
					}
				}
			}
		
			return types;
		}
	}
}
