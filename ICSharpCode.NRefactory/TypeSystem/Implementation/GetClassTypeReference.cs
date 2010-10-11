// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.Utils;

namespace ICSharpCode.NRefactory.TypeSystem.Implementation
{
	/// <summary>
	/// Type Reference used when the fully qualified type name is known.
	/// </summary>
	public sealed class GetClassTypeReference : ITypeReference, ISupportsInterning
	{
		string fullTypeName;
		int typeParameterCount;
		//volatile CachedResult v_cachedResult;
		
		public GetClassTypeReference(string fullTypeName, int typeParameterCount)
		{
			if (fullTypeName == null)
				throw new ArgumentNullException("fullTypeName");
			this.fullTypeName = fullTypeName;
			this.typeParameterCount = typeParameterCount;
		}
		
		/*
		sealed class CachedResult
		{
			public readonly CacheManager CacheManager;
			public readonly IType Result;
			
			public CachedResult(CacheManager cacheManager, IType result)
			{
				this.CacheManager = cacheManager;
				this.Result = result;
			}
		}
		 */
		
		public IType Resolve(ITypeResolveContext context)
		{
			if (context == null)
				throw new ArgumentNullException("context");
			
			/*  TODO PERF: caching disabled until we measure how much of an advantage it is
			 * (and whether other approaches like caching only the last N resolve calls in a thread-static cache would work better)
			 * Maybe even make a distinction between the really common type references (e.g. primitiveTypeReferences) and
			 * normal GetClassTypeReferences?
			CacheManager cacheManager = context.CacheManager;
			if (cacheManager != null) {
				CachedResult result = this.v_cachedResult;
				if (result != null && result.CacheManager == cacheManager)
					return result.Result;
				IType newResult = DoResolve(context);
				this.v_cachedResult = new CachedResult(cacheManager, newResult);
				cacheManager.Disposed += delegate { v_cachedResult = null; }; // maybe optimize this to use interface call instead of delegate?
				return newResult;
			} else {
				return DoResolve(context);
			}
			
		}
		
		IType DoResolve(ITypeResolveContext context)
		{
			 */
			return context.GetClass(fullTypeName, typeParameterCount, StringComparer.Ordinal) ?? SharedTypes.UnknownType;
		}
		
		public override string ToString()
		{
			if (typeParameterCount == 0)
				return fullTypeName;
			else
				return fullTypeName + "`" + typeParameterCount;
		}
		
		void ISupportsInterning.PrepareForInterning(IInterningProvider provider)
		{
			fullTypeName = provider.Intern(fullTypeName);
		}
		
		int ISupportsInterning.GetHashCodeForInterning()
		{
			return fullTypeName.GetHashCode() ^ typeParameterCount;
		}
		
		bool ISupportsInterning.EqualsForInterning(ISupportsInterning other)
		{
			GetClassTypeReference o = other as GetClassTypeReference;
			return o != null && fullTypeName == o.fullTypeName && typeParameterCount == o.typeParameterCount;
		}
	}
}
