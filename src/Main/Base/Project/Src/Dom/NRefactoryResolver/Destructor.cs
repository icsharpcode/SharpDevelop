// created on 06.08.2003 at 12:35

using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.NRefactory.Parser.AST;

namespace ICSharpCode.SharpDevelop.Dom.NRefactoryResolver
{
	public class Destructor : AbstractMethod
	{
		public Destructor(IRegion region, IRegion bodyRegion, IClass declaringType) : base(declaringType, "~" + declaringType.Name)
		{
			this.Region     = region;
			this.bodyRegion = bodyRegion;
		}
	}
}
