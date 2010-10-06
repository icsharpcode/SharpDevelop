// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;

namespace ICSharpCode.NRefactory.TypeSystem.Implementation
{
	/// <summary>
	/// Default implementation of <see cref="IMethod" /> interface.
	/// </summary>
	public class DefaultMethod : AbstractMember, IMethod
	{
		IList<IAttribute> returnTypeAttributes;
		IList<ITypeParameter> typeParameters;
		IList<IParameter> parameters;
		
		const ushort FlagExtensionMethod = 0x1000;
		
		protected override void FreezeInternal()
		{
			returnTypeAttributes = FreezeList(returnTypeAttributes);
			typeParameters = FreezeList(typeParameters);
			parameters = FreezeList(parameters);
			base.FreezeInternal();
		}
		
		public DefaultMethod(ITypeDefinition declaringTypeDefinition, string name)
			: base(declaringTypeDefinition, name, EntityType.Method)
		{
		}
		
		public IList<IAttribute> ReturnTypeAttributes {
			get {
				if (returnTypeAttributes == null)
					returnTypeAttributes = new List<IAttribute>();
				return returnTypeAttributes;
			}
		}
		
		public IList<ITypeParameter> TypeParameters {
			get {
				if (typeParameters == null)
					typeParameters = new List<ITypeParameter>();
				return typeParameters;
			}
		}
		
		public bool IsExtensionMethod {
			get { return flags[FlagExtensionMethod]; }
			set {
				CheckBeforeMutation();
				flags[FlagExtensionMethod] = value;
			}
		}
		
		public bool IsConstructor {
			get { return this.EntityType == EntityType.Constructor; }
		}
		
		public bool IsDestructor {
			get { return this.EntityType == EntityType.Destructor; }
		}
		
		public bool IsOperator {
			get { return this.EntityType == EntityType.Operator; }
		}
		
		public IList<IParameter> Parameters {
			get {
				if (parameters == null)
					parameters = new List<IParameter>();
				return parameters;
			}
		}
	}
}
