// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Xml;
using System.Diagnostics;
using System.ComponentModel;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Internal.Project;

namespace ICSharpCode.WixBinding
{
	/// <summary>
	/// This class handles project specific compiler parameters
	/// </summary>
	public class WixCompilerParameters : AbstractProjectConfiguration
	{

		public WixCompilerParameters()
		{
		}
		public WixCompilerParameters(string name)
		{
			this.name = name;
		}
	}
}
