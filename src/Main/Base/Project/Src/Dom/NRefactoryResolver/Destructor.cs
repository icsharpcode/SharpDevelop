// created on 06.08.2003 at 12:35

using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.NRefactory.Parser.AST;

namespace ICSharpCode.SharpDevelop.Dom.NRefactoryResolver
{
	public class Destructor : AbstractMethod
	{
		public void AddModifier(ModifierEnum m)
		{
			modifiers = modifiers | m;
		}
		
		public Destructor(string className, Modifier m, IRegion region, IRegion bodyRegion, IClass declaringType) : base(declaringType)
		{
			FullyQualifiedName = "~" + className;
			this.region     = region;
			this.bodyRegion = bodyRegion;
			modifiers = (ModifierEnum)m;
		}
	}
}
