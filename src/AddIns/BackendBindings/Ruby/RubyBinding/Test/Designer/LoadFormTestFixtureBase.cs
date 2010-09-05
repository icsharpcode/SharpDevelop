// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using ICSharpCode.RubyBinding;
using ICSharpCode.Scripting;
using ICSharpCode.Scripting.Tests.Designer;
using ICSharpCode.Scripting.Tests.Utils;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Designer
{
	/// <summary>
	/// Base class for all LoadFormTestFixture classes.
	/// </summary>
	public class LoadFormTestFixtureBase : LoadFormTestsBase
	{
		protected override IComponentWalker CreateComponentWalker(IComponentCreator componentCreator)
		{
			return RubyComponentWalkerHelper.CreateComponentWalker(componentCreator);
		}
		
		/// <summary>
		/// Gets the Ruby code that will be loaded.
		/// </summary>
		public virtual string RubyCode {
			get { return String.Empty; }
		}
		
		public override string Code {
			get { return RubyCode; }
		}
	}
}
