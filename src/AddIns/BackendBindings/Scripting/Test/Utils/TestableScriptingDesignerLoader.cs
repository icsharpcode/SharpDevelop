// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel.Design.Serialization;
using ICSharpCode.Scripting;

namespace ICSharpCode.Scripting.Tests.Utils
{
	public class TestableScriptingDesignerLoader : ScriptingDesignerLoader
	{
		public IComponentCreator ComponentCreatorPassedToCreateComponentWalker;
		public FakeComponentWalker FakeComponentWalker = new FakeComponentWalker();
		
		public TestableScriptingDesignerLoader(IScriptingDesignerGenerator generator)
			: base(generator)
		{
		}
		
		public void CallPerformFlush(IDesignerSerializationManager serializationManager)
		{
			base.PerformFlush(serializationManager);
		}
		
		public void CallPerformLoad(IDesignerSerializationManager serializationManager)
		{
			base.PerformLoad(serializationManager);
		}
		
		protected override IComponentWalker CreateComponentWalker(IComponentCreator componentCreator)
		{
			ComponentCreatorPassedToCreateComponentWalker = componentCreator;
			return FakeComponentWalker;
		}
	}
}
