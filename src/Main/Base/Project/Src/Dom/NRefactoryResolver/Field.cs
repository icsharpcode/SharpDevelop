// created on 04.08.2003 at 18:06

using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.NRefactory.Parser.AST;

namespace ICSharpCode.SharpDevelop.Dom.NRefactoryResolver
{
	public class Field : AbstractField
	{
		public void AddModifier(ModifierEnum m)
		{
			modifiers = modifiers | m;
		}
		
		public Field(ReturnType type, string fullyQualifiedName, Modifier m, IRegion region, IClass declaringType) : base(declaringType)
		{
			this.returnType = type;
			this.FullyQualifiedName = fullyQualifiedName;
			this.region = region;
			modifiers = (ModifierEnum)m;
			if (modifiers == ModifierEnum.None) {
				modifiers = ModifierEnum.Private;
			}
		}
		public void SetModifiers(ModifierEnum m)
		{
			modifiers = m;
		}
	}
}
