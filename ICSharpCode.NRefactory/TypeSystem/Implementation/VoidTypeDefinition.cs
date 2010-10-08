
using System;
using System.Collections.Generic;

namespace ICSharpCode.NRefactory.TypeSystem.Implementation
{
	/// <summary>
	/// Special type definition for 'void'.
	/// </summary>
	public class VoidTypeDefinition : DefaultTypeDefinition
	{
		public VoidTypeDefinition(IProjectContent projectContent)
			: base(projectContent, "System", "Void")
		{
			this.ClassType = ClassType.Struct;
			this.Accessibility = Accessibility.Public;
			this.IsSealed = true;
		}
		
		public override IEnumerable<IMethod> GetConstructors(ITypeResolveContext context)
		{
			return EmptyList<IMethod>.Instance;
		}
		
		public override IEnumerable<IEvent> GetEvents(ITypeResolveContext context)
		{
			return EmptyList<IEvent>.Instance;
		}
		
		public override IEnumerable<IField> GetFields(ITypeResolveContext context)
		{
			return EmptyList<IField>.Instance;
		}
		
		public override IEnumerable<IMethod> GetMethods(ITypeResolveContext context)
		{
			return EmptyList<IMethod>.Instance;
		}
		
		public override IEnumerable<IProperty> GetProperties(ITypeResolveContext context)
		{
			return EmptyList<IProperty>.Instance;
		}
	}
}
