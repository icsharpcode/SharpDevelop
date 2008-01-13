// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

// created on 22.08.2003 at 19:02

using System;
using System.Collections.Generic;
using System.Text;

using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.Visitors;

namespace ICSharpCode.SharpDevelop.Dom.NRefactoryResolver
{
	// TODO: Rename this class, the visitor functionality was moved to ResolveVisitor
	public static class TypeVisitor
	{
		public static IReturnType CreateReturnType(TypeReference reference, NRefactoryResolver resolver)
		{
			return CreateReturnType(reference,
			                        resolver.CallingClass, resolver.CallingMember,
			                        resolver.CaretLine, resolver.CaretColumn,
			                        resolver.ProjectContent, false);
		}
		
		public static IReturnType CreateReturnType(TypeReference reference, IClass callingClass,
		                                           IMember callingMember, int caretLine, int caretColumn,
		                                           IProjectContent projectContent,
		                                           bool useLazyReturnType)
		{
			if (reference == null) return null;
			if (reference.IsNull) return null;
			if (reference is InnerClassTypeReference) {
				reference = ((InnerClassTypeReference)reference).CombineToNormalTypeReference();
			}
			LanguageProperties languageProperties = projectContent.Language;
			IReturnType t = null;
			if (callingClass != null && !reference.IsGlobal) {
				foreach (ITypeParameter tp in callingClass.TypeParameters) {
					if (languageProperties.NameComparer.Equals(tp.Name, reference.SystemType)) {
						t = new GenericReturnType(tp);
						break;
					}
				}
				if (t == null && callingMember is IMethod && (callingMember as IMethod).TypeParameters != null) {
					foreach (ITypeParameter tp in (callingMember as IMethod).TypeParameters) {
						if (languageProperties.NameComparer.Equals(tp.Name, reference.SystemType)) {
							t = new GenericReturnType(tp);
							break;
						}
					}
				}
			}
			if (t == null) {
				if (reference.Type != reference.SystemType) {
					// keyword-type like void, int, string etc.
					IClass c = projectContent.GetClass(reference.SystemType, 0);
					if (c != null)
						t = c.DefaultReturnType;
					else
						t = new GetClassReturnType(projectContent, reference.SystemType, 0);
				} else {
					int typeParameterCount = reference.GenericTypes.Count;
					if (useLazyReturnType) {
						if (reference.IsGlobal)
							t = new GetClassReturnType(projectContent, reference.SystemType, typeParameterCount);
						else if (callingClass != null)
							t = new SearchClassReturnType(projectContent, callingClass, caretLine, caretColumn, reference.SystemType, typeParameterCount);
					} else {
						IClass c;
						if (reference.IsGlobal) {
							c = projectContent.GetClass(reference.SystemType, typeParameterCount);
							t = (c != null) ? c.DefaultReturnType : null;
						} else if (callingClass != null) {
							t = projectContent.SearchType(new SearchTypeRequest(reference.SystemType, typeParameterCount, callingClass, caretLine, caretColumn)).Result;
						}
						if (t == null) {
							return null;
						}
					}
				}
			}
			if (reference.GenericTypes.Count > 0) {
				List<IReturnType> para = new List<IReturnType>(reference.GenericTypes.Count);
				for (int i = 0; i < reference.GenericTypes.Count; ++i) {
					para.Add(CreateReturnType(reference.GenericTypes[i], callingClass, callingMember, caretLine, caretColumn, projectContent, useLazyReturnType));
				}
				t = new ConstructedReturnType(t, para);
			}
			return WrapArray(projectContent, t, reference);
		}
		
		static IReturnType WrapArray(IProjectContent pc, IReturnType t, TypeReference reference)
		{
			if (reference.IsArrayType) {
				for (int i = reference.RankSpecifier.Length - 1; i >= 0; --i) {
					int dimensions = reference.RankSpecifier[i] + 1;
					if (dimensions > 0) {
						t = new ArrayReturnType(pc, t, dimensions);
					}
				}
			}
			return t;
		}
	}
}
