// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
