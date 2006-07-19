// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="none" email=""/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.NRefactory.Parser.Ast;

namespace ICSharpCode.NRefactory.Parser.VB
{
	internal class ParamModifiers
	{
		ParamModifier cur;
		Parser   parser;
		
		public ParamModifier Modifier {
			get {
				return cur;
			}
		}
		
		public ParamModifiers(Parser parser)
		{
			this.parser = parser;
			cur         = ParamModifier.None;
		}
		
		public bool isNone { get { return cur == ParamModifier.None; } }
		
		public void Add(ParamModifier m) 
		{
			if ((cur & m) == 0) {
				cur |= m;
			} else {
				parser.Error("param modifier " + m + " already defined");
			}
		}
		
		public void Add(ParamModifiers m)
		{
			Add(m.cur);
		}
		
		public void Check()
		{
			if((cur & ParamModifier.In) != 0 && 
			   (cur & ParamModifier.Ref) != 0) {
				parser.Error("ByRef and ByVal are not allowed at the same time.");
			}
		}
	}
}
