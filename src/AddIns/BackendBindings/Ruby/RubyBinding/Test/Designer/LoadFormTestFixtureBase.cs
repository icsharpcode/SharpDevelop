// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

using ICSharpCode.RubyBinding;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Designer
{
	/// <summary>
	/// Base class for all LoadFormTestFixture classes.
	/// </summary>
	public class LoadFormTestFixtureBase
	{		
		MockComponentCreator componentCreator = new MockComponentCreator();
		Form form;

		public LoadFormTestFixtureBase()
		{
		}
				
		[TestFixtureSetUp]
		public void SetUpFixture()
		{			
			BeforeSetUpFixture();
			RubyComponentWalker walker = new RubyComponentWalker(componentCreator);
			form = walker.CreateComponent(RubyCode) as Form;
		}

		[TestFixtureTearDown]
		public void TearDownFixture()
		{
			form.Dispose();
		}		

		/// <summary>
		/// Called at the start of SetUpFixture method before anything is setup.
		/// </summary>
		public virtual void BeforeSetUpFixture()
		{
		}
		
		/// <summary>
		/// Gets the Ruby code that will be loaded.
		/// </summary>
		public virtual string RubyCode {
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
