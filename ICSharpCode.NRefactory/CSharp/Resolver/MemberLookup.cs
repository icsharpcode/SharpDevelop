// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Collections;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.NRefactory.CSharp.Resolver
{
	/// <summary>
	/// Implementation of member lookup (C# 4.0 spec, §7.4).
	/// </summary>
	public class MemberLookup
	{
		#region Static helper methods
		/// <summary>
		/// Gets whether the member is considered to be invocable.
		/// </summary>
		public static bool IsInvocable(IMember member, ITypeResolveContext context)
		{
			if (member == null)
				throw new ArgumentNullException("member");
			// C# 4.0 spec, §7.4 member lookup
			if (member is IEvent || member is IMethod)
				return true;
			if (member.ReturnType == SharedTypes.Dynamic)
				return true;
			return member.ReturnType.Resolve(context).IsDelegate();
		}
		
		#endregion
		
		ITypeResolveContext context;
		ITypeDefinition currentTypeDefinition;
		IProjectContent currentProject;
		
		public MemberLookup(ITypeResolveContext context, ITypeDefinition currentTypeDefinition, IProjectContent currentProject)
		{
			if (context == null)
				throw new ArgumentNullException("context");
			this.context = context;
			this.currentTypeDefinition = currentTypeDefinition;
			this.currentProject = currentProject;
		}
		
		#region IsAccessible
		/// <summary>
		/// Gets whether <paramref name="entity"/> is accessible in the current class.
		/// </summary>
		/// <param name="member">The entity to test</param>
		/// <param name="typeOfReference">The type used to access the member, or null if no target is used (e.g. static method call)</param>
		/// <returns>true if the member is accessible</returns>
		public bool IsAccessible(IEntity entity, IType typeOfReference)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");
			// C# 4.0 spec, §3.5.2 Accessiblity domains
			switch (entity.Accessibility) {
				case Accessibility.None:
					return false;
				case Accessibility.Private:
					return entity.DeclaringTypeDefinition == currentTypeDefinition;
				case Accessibility.Public:
					return true;
				case Accessibility.Protected:
					return IsProtectedAccessible(entity.DeclaringTypeDefinition, typeOfReference);
				case Accessibility.Internal:
					return IsInternalAccessible(entity.ProjectContent);
				case Accessibility.ProtectedOrInternal:
					return IsProtectedAccessible(entity.DeclaringTypeDefinition, typeOfReference)
						|| IsInternalAccessible(entity.ProjectContent);
				case Accessibility.ProtectedAndInternal:
					return IsProtectedAccessible(entity.DeclaringTypeDefinition, typeOfReference)
						&& IsInternalAccessible(entity.ProjectContent);
				default:
					throw new Exception("Invalid value for Accessibility");
			}
		}
		
		bool IsInternalAccessible(IProjectContent declaringProject)
		{
			return declaringProject != null && currentProject != null && declaringProject.InternalsVisibleTo(currentProject, context);
		}
		
		bool IsProtectedAccessible(ITypeDefinition declaringType, IType typeOfReference)
		{
			if (declaringType == currentTypeDefinition)
				return true;
			// PERF: this might hurt performance as this method is called several times (once for each member)
			// make sure resolving base types is cheap (caches?) or cache within the MemberLookup instance
			if (currentTypeDefinition == null || !currentTypeDefinition.IsDerivedFrom(declaringType, context))
				return false;
			if (typeOfReference == null)
				return true; // no restriction on the type of reference
			ITypeDefinition referenceDef = typeOfReference.GetDefinition();
			return referenceDef != null && referenceDef.IsDerivedFrom(currentTypeDefinition, context);
		}
		#endregion
	}
}
