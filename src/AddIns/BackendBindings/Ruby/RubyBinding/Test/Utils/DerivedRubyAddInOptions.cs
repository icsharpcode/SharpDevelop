// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.RubyBinding;

namespace RubyBinding.Tests.Utils
{
	/// <summary>
	/// Overrides the AddInOptions GetAddInPath method to return
	/// some dummy data used for testing.
	/// </summary>
	public class DerivedRubyAddInOptions : RubyAddInOptions
	{
		string addInPath = String.Empty;
		string addInPathRequested = String.Empty;
		
		public DerivedRubyAddInOptions(Properties properties) : base(properties)
		{
		}
		
		/// <summary>
		/// Gets the addin path string passed to the GetAddInPath method.
		/// </summary>
		public string AddInPathRequested {
			get { return addInPathRequested; }
		}
		
		/// <summary>
		/// Gets or sets the addin path that should be returned from the
		/// GetAddInPath method.
		/// </summary>
		public string AddInPath {
			get { return addInPath; }
			set { addInPath = value; }
		}
		
		/// <summary>
		/// Returns our dummy AddInPath.
		/// </summary>
		protected override string GetAddInPath(string addIn)
		{
			addInPathRequested = addIn;
			return addInPath;
		}
	}
}
