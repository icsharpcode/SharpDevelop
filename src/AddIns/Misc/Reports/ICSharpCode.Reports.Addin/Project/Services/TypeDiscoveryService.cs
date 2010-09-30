// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Reflection;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.Reports.Addin
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
			
			if (baseType == null) {
				baseType = typeof(object);
			}
			
			LoggingService.Debug("TypeDiscoveryService.GetTypes for " + baseType.FullName
			                     + "excludeGlobalTypes=" + excludeGlobalTypes.ToString());
			//seek in all assemblies
			//allow to work designers like columns editor in datagridview
			// Searching types can cause additional assemblies to be loaded, so we need to use
			// ToArray to prevent an exception if the collection changes.
			foreach (Assembly asm in TypeResolutionService.DesignerAssemblies.ToArray()) {
				if (excludeGlobalTypes) {
					if (GacInterop.IsWithinGac(asm.Location)) {
						continue;
					}
				}
				AddDerivedTypes(baseType, asm, types);
			}
			LoggingService.Debug("TypeDiscoveryService returns " + types.Count + " types");
			
			// TODO - Don't look in all assemblies.
			// Should use the current project and its referenced assemblies
			// as well as System.Windows.Forms.
			
			return types;
		}
		
		/// <summary>
		/// Gets the types derived from baseType from the assembly and adds them to the list.
		/// </summary>
		void AddDerivedTypes(Type baseType, Assembly assembly, IList<Type> list)
		{
			foreach (Type t in assembly.GetExportedTypes()) {
				if (t.IsSubclassOf(baseType)) {
					//LoggingService.Debug("TypeDiscoveryService.  Adding type=" + t.FullName);
					list.Add(t);
				}
			}
		}
	}
}
