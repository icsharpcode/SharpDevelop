// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Gallio.SharpDevelop
{
	public class TestRunnerExtensionCommandLineArgument
	{
		string type;
		string parameters = String.Empty;
		
		public TestRunnerExtensionCommandLineArgument(string type)
		{
			this.type = type;
		}
		
		public string Type {
			get { return type; }
		}
		
		public string Parameters {
			get { return parameters; }
			set { parameters = value; }
		}
		
		public override string ToString()
		{
			return String.Format("/re:\"{0}{1}\"",
				GetTypeName(),
				GetParameters());
		}
		
		string GetTypeName()
		{
			return type;
		}
		
		string GetParameters()
		{
			if (String.IsNullOrEmpty(parameters)) {
				return String.Empty;
			}
			return String.Format(";{0}", parameters);
		}
	}
}
