// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

using ICSharpCode.Scripting;
using ICSharpCode.Scripting.Tests.Utils;
using NUnit.Framework;

namespace ICSharpCode.Scripting.Tests.Designer
{
	public abstract class LoadFormTestsBase
	{		
		MockComponentCreator componentCreator = new MockComponentCreator();
		Form form;
				
		[TestFixtureSetUp]
		public void SetUpFixture()
		{			
			BeforeSetUpFixture();
			IComponentWalker walker = CreateComponentWalker(componentCreator);
			form = walker.CreateComponent(Code) as Form;
		}

		[TestFixtureTearDown]
		public void TearDownFixture()
		{
			if (form != null) {
				form.Dispose();
			}
		}

		/// <summary>
		/// Called at the start of SetUpFixture method before anything is setup.
		/// </summary>
		public virtual void BeforeSetUpFixture()
		{
		}
		
		protected abstract IComponentWalker CreateComponentWalker(IComponentCreator componentCreator);
		
		/// <summary>
		/// Gets the code that will be loaded.
		/// </summary>
		public virtual string Code {
			get { return String.Empty; }
		}
		
		protected MockComponentCreator ComponentCreator {
			get { return componentCreator; }
		}
				
		protected Form Form {
			get { return form; }
		}		
	}
}
