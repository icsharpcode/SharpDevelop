// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com" />
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;

namespace SharpRefactoring
{
	public static class Extensions
	{
		public static IMember GetInnermostMember(this ICompilationUnit unit, int caretLine, int caretColumn)
		{
			IClass c = unit.GetInnermostClass(caretLine, caretColumn);
			
			if (c == null)
				return null;
			
			return c.AllMembers
				.SingleOrDefault(m => m.BodyRegion.IsInside(caretLine, caretColumn));
		}
		
		public static IMember GetInnermostMember(this IClass instance, int caretLine, int caretColumn)
		{
			instance = instance.GetInnermostClass(caretLine, caretColumn);
			
			if (instance == null)
				return null;
			
			return instance.AllMembers
				.SingleOrDefault(m => m.BodyRegion.IsInside(caretLine, caretColumn));
		}
		
		/// <summary>
		/// Gets all interfaces that this class declares to implement, but its implementations are missing in the class body.
		/// </summary>
		public static List<IClass> GetInterfacesMissingImplementation(this IClass @class)
		{
			return null;
		}
		
		public static bool ClassImplementsClass(IClass targetClass, IClass baseClass)
		{
			bool requireAlternativeImplementation;
			var targetClassType = targetClass.DefaultReturnType;
			var baseClassType = baseClass.DefaultReturnType;
			
			var targetClassMethods = targetClassType.GetMethods();
			foreach (var m in baseClassType.GetMethods()) {
				if (!CodeGenerator.InterfaceMemberAlreadyImplemented(targetClassMethods, m, out requireAlternativeImplementation)) {
					return false;
				}
			}
			var targetClassProperties = targetClassType.GetProperties();
			foreach (var p in baseClassType.GetProperties()) {
				if (!CodeGenerator.InterfaceMemberAlreadyImplemented(targetClassProperties, p, out requireAlternativeImplementation)) {
					return false;
				}
			}
			var targetClassEvents = targetClassType.GetEvents();
			foreach (var e in baseClassType.GetEvents()) {
				if (!CodeGenerator.InterfaceMemberAlreadyImplemented(targetClassEvents, e, out requireAlternativeImplementation)) {
					return false;
				}
			}
			return true;
		}
	}
}
