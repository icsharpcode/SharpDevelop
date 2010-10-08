
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
		
		public override IList<IMethod> GetConstructors(ITypeResolveContext context)
		{
			return new List<IMethod>();
		}
		
		public override IList<IEvent> GetEvents(ITypeResolveContext context)
		{
			return new List<IEvent>();
		}
		
		public override IList<IField> GetFields(ITypeResolveContext context)
		{
			return new List<IField>();
		}
		
		public override IList<IMethod> GetMethods(ITypeResolveContext context)
		{
			return new List<IMethod>();
		}
		
		public override IList<IProperty> GetProperties(ITypeResolveContext context)
		{
			return new List<IProperty>();
		}
	}
}
