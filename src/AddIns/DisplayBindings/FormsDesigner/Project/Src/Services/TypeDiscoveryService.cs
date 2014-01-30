// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Reflection;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Designer;

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
					if (SD.GlobalAssemblyCache.IsGacAssembly(asm.Location)) {
						continue;
					}
				}
				try {
					AddDerivedTypes(baseType, asm, types);
				} catch (FileNotFoundException) {
				} catch (FileLoadException) {
				} catch (BadImageFormatException) {
					// ignore assembly load errors
				}
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
