// Copyright (c) AlphaSierraPapa for the SharpDevelop Team
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
using System.Linq;

namespace ICSharpCode.NRefactory.TypeSystem.Implementation
{
	/// <summary>
	/// References an entity by its type and name.
	/// This class can be used to refer to fields, events, and parameterless properties.
	/// </summary>
	[Serializable]
	public sealed class DefaultMemberReference : IMemberReference, ISupportsInterning
	{
		EntityType entityType;
		ITypeReference typeReference;
		string name;
		
		public DefaultMemberReference(EntityType entityType, ITypeReference typeReference, string name)
		{
			if (typeReference == null)
				throw new ArgumentNullException("typeReference");
			if (name == null)
				throw new ArgumentNullException("name");
			this.entityType = entityType;
			this.typeReference = typeReference;
			this.name = name;
		}
		
		public IMember Resolve(ITypeResolveContext context)
		{
			IType type = typeReference.Resolve(context);
			return type.GetMembers(m => m.Name == name && m.EntityType == entityType, GetMemberOptions.IgnoreInheritedMembers).FirstOrDefault();
		}
		
		void ISupportsInterning.PrepareForInterning(IInterningProvider provider)
		{
			typeReference = provider.Intern(typeReference);
			name = provider.Intern(name);
		}
		
		int ISupportsInterning.GetHashCodeForInterning()
		{
			return (int)entityType ^ typeReference.GetHashCode() ^ name.GetHashCode();
		}
		
		bool ISupportsInterning.EqualsForInterning(ISupportsInterning other)
		{
			DefaultMemberReference o = other as DefaultMemberReference;
			return o != null && entityType == o.entityType && typeReference == o.typeReference && name == o.name;
		}
	}
}
