// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.NRefactory.TypeSystem.Implementation
{
	/// <summary>
	/// Type Reference used when the fully qualified type name is known.
	/// </summary>
	public sealed class GetClassTypeReference : ITypeReference, ISupportsInterning
	{
		string fullTypeName;
		int typeParameterCount;
		
		public GetClassTypeReference(string fullTypeName, int typeParameterCount)
		{
			if (fullTypeName == null)
				throw new ArgumentNullException("fullTypeName");
			this.fullTypeName = fullTypeName;
			this.typeParameterCount = typeParameterCount;
		}
		
		public IType Resolve(ITypeResolveContext context)
		{
			if (context == null)
				throw new ArgumentNullException("context");
			// TODO: caching idea: if these lookups are a performance problem and the same GetClassTypeReference
			// is asked to resolve lots of times in a row, try the following:
			// Give ITypeResolveContext a property "object CacheToken { get; }" which is non-null if the
			// context supports caching, and returns the same object only as long as the context is unchanged.
			
			// Then store a thread-local array KeyValuePair<GetClassTypeReference, IType> with the last 5 (?) resolved
			// types, and a thread-local reference to the cache token. Subsequent calls with the same cache token
			// do a quick (reference equality) lookup in the array first.
			// This should be faster than any ServiceContainer-based caches.
			// It is worth an idea to make CacheToken implement IServiceContainer, so that other (more expensive)
			// caches can be registered there, but I think it's troublesome as one ITypeResolveContext should be usable
			// on multiple threads.
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
