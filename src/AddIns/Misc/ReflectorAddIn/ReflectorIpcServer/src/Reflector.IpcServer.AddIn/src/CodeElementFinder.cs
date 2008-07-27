// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Christian Hornung" email="chhornung@googlemail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Text;

using Reflector.CodeModel;

namespace Reflector.IpcServer.AddIn
{
	/// <summary>
	/// Finds a code element in Reflector's DOM.
	/// </summary>
	internal sealed class CodeElementFinder
	{
		readonly CodeElementInfo element;
		
		internal CodeElementFinder(CodeElementInfo element)
		{
			this.element = element;
		}
		
		delegate object Finder<T>(T item);
		
		static object Find<T>(System.Collections.ICollection collection, Finder<T> finder)
			where T : class
		{
			foreach (T item in collection) {
				object result = finder(item);
				if (result != null) return result;
			}
			return null;
		}
		
		// ********************************************************************************************************************************
		
		internal object Find(IAssembly asm)
		{
			return Find<IModule>(asm.Modules, Find) ?? asm;
		}
		
		object Find(IModule mod)
		{
			return Find<ITypeDeclaration>(mod.Types, Find);
		}
		
		object Find(ITypeDeclaration typeDecl)
		{
			if (element.Type == null ||
			    element.Type.Namespace == null ||
			    element.Type.TypeNames == null ||
			    element.Type.TypeArgumentCount == null) {
				return null;
			}
			
			// If this is an outermost type declaration, check the namespace.
			if (!(typeDecl.Owner is ITypeDeclaration)) {
				if (!String.Equals(typeDecl.Namespace, element.Type.Namespace, StringComparison.InvariantCulture)) {
					return null;
				}
			}
			
			// Check the current type
			if (!Is(typeDecl, element.Type)) {
				// Look in nested types
				return Find<ITypeDeclaration>(typeDecl.NestedTypes, Find);
			}
			
			// This is the correct type.
			// Look for members.
			
			object result = null;
			
			switch(element.MemberType) {
				case MemberType.Event:
					result = Find<IEventDeclaration>(typeDecl.Events, Find);
					break;
					
				case MemberType.Field:
					result = Find<IFieldDeclaration>(typeDecl.Fields, Find);
					break;
					
				case MemberType.Method:
				case MemberType.Constructor:
					result = Find<IMethodDeclaration>(typeDecl.Methods, Find);
					break;
					
				case MemberType.Property:
					result = Find<IPropertyDeclaration>(typeDecl.Properties, Find);
					break;
			}
			
			return result ?? typeDecl;
		}
		
		#region Type checking
		
		/// <summary>
		/// Tests whether the specified type declaration is the same type
		/// as the CodeTypeInfo parameter describes
		/// (except for the namespace, which is tested before).
		/// </summary>
		static bool Is(ITypeDeclaration t, CodeTypeInfo cti)
		{
			if (t == null || cti == null) return false;
			
			int index = cti.TypeNames.Length - 1;
			do {
				if (index < 0 ||
				    !String.Equals(t.Name, cti.TypeNames[index], StringComparison.InvariantCulture) ||
				    t.GenericArguments.Count != cti.TypeArgumentCount[index]) {
					return false;
				}
				--index;
			} while ((t = t.Owner as ITypeDeclaration) != null);
			return index == -1;
		}
		
		/// <summary>
		/// Tests whether the specified type is the same type
		/// as the CodeTypeInfo parameter describes.
		/// </summary>
		static bool Is(IType t, CodeTypeInfo cti)
		{
			ITypeReference r = t as ITypeReference;
			if (r != null) return Is(r, cti);
			
			IArrayType a = t as IArrayType;
			if (a != null && cti != null) {
				return Is(a.ElementType, cti.ArrayElementType) &&
					(a.Dimensions.Count == cti.ArrayDimensions || (a.Dimensions.Count == 0 && cti.ArrayDimensions == 1));
			}
			
			IReferenceType rt = t as IReferenceType;
			if (rt != null) {
				return Is(rt.ElementType, cti);
			}
			
			IGenericArgument ga = t as IGenericArgument;
			if (ga != null) {
				return true;
			}
			
			return false;
		}
		
		/// <summary>
		/// Tests whether the specified type is the same type
		/// as the CodeTypeInfo parameter describes.
		/// </summary>
		static bool Is(ITypeReference r, CodeTypeInfo cti)
		{
			if (r == null || cti == null) return false;
			
			int index = cti.TypeNames.Length - 1;
			do {
				if (index < 0 ||
				    !String.Equals(r.Name, cti.TypeNames[index], StringComparison.InvariantCulture) ||
				    r.GenericArguments.Count != cti.TypeArgumentCount[index]) {
					return false;
				}
				--index;
				
				ITypeReference next = r.Owner as ITypeReference;
				if (next == null) {
					if (!String.Equals(r.Namespace, cti.Namespace, StringComparison.InvariantCulture)) {
						return false;
					}
					break;
				} else {
					r = next;
				}
			} while (true);
			return index == -1;
		}
		
		static bool CheckParameters(CodeTypeInfo[] cti, IParameterDeclarationCollection pdc)
		{
			if (cti == null || cti.Length != pdc.Count) return false;
			for (int i = 0; i < cti.Length; ++i) {
				if (cti[i] != null && !Is(pdc[i].ParameterType, cti[i])) return false;
			}
			return true;
		}
		
		#endregion
		
		object Find(IEventDeclaration eventDecl)
		{
			if (String.Equals(element.MemberName, eventDecl.Name, StringComparison.InvariantCulture)) {
				return eventDecl;
			}
			return null;
		}
		
		object Find(IFieldDeclaration fieldDecl)
		{
			if (String.Equals(element.MemberName, fieldDecl.Name, StringComparison.InvariantCulture) &&
			    (element.MemberReturnType == null || Is(fieldDecl.FieldType, element.MemberReturnType))) {
				return fieldDecl;
			}
			return null;
		}
		
		object Find(IMethodDeclaration methodDecl)
		{
			if (String.Equals(element.MemberName, methodDecl.Name, StringComparison.InvariantCulture) &&
			    element.MemberTypeArgumentCount == methodDecl.GenericArguments.Count &&
			    (element.MemberType == MemberType.Constructor || (element.MemberReturnType == null || Is(methodDecl.ReturnType.Type, element.MemberReturnType))) &&
			    CheckParameters(element.MemberParameters, methodDecl.Parameters)) {
				return methodDecl;
			}
			return null;
		}
		
		object Find(IPropertyDeclaration propertyDecl)
		{
			if (String.Equals(element.MemberName, propertyDecl.Name, StringComparison.InvariantCulture) &&
			    (element.MemberReturnType == null || Is(propertyDecl.PropertyType, element.MemberReturnType)) &&
			    CheckParameters(element.MemberParameters, propertyDecl.Parameters)) {
				return propertyDecl;
			}
			return null;
		}
	}
}
