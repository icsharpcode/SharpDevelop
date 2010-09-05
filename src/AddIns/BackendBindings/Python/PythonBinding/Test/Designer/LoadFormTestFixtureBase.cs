// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

using ICSharpCode.PythonBinding;
using ICSharpCode.Scripting;
using ICSharpCode.Scripting.Tests.Designer;
using ICSharpCode.Scripting.Tests.Utils;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	/// <summary>
	/// Base class for all LoadFormTestFixture classes.
	/// </summary>
	public class LoadFormTestFixtureBase : LoadFormTestsBase
	{
		protected override IComponentWalker CreateComponentWalker(IComponentCreator componentCreator)
		{
			return PythonComponentWalkerHelper.CreateComponentWalker(componentCreator);
		}
		
		public virtual string PythonCode {
			get { return String.Empty; }
		}
		
		public override string Code {
			get { return PythonCode; }
		}
	}
}
