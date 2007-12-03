// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.Core;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Holds the options for the PythonBinding AddIn
	/// </summary>
	public class AddInOptions
	{
		/// <summary>
		/// The name of the options as read from the PropertyService.
		/// </summary>
		public static readonly string AddInOptionsName = "PythonBinding.Options";
		
		/// <summary>
		/// The default python console filename.
		/// </summary>
		public static readonly string DefaultPythonFileName = "ipy.exe";
		
		#region Property names
		public static readonly string PythonFileNameProperty = "PythonFileName";
		#endregion
		
		Properties properties;
		
		public AddInOptions()
			: this(PropertyService.Get(AddInOptionsName, new Properties()))
		{
		}
		
		/// <summary>
		/// Creates the addin options class which will use
		/// the options from the properties class specified.
		/// </summary>
		public AddInOptions(Properties properties)
		{
			this.properties = properties;
		}
		
		/// <summary>
		/// Gets or sets the python console filename.
		/// </summary>
		public string PythonFileName {
			get {
				return properties.Get<string>(PythonFileNameProperty, GetDefaultPythonFileName());
			}
			set {
				if (String.IsNullOrEmpty(value)) {
					properties.Set(PythonFileNameProperty, GetDefaultPythonFileName());
				} else {
					properties.Set(PythonFileNameProperty, value);
				}
			}
		}
		
		/// <summary>
		/// Gets the path to the specified addin.
		/// </summary>
		/// <param name="addIn">The addin name: "${addin:ICSharpCode.PythonBinding}"</param>
		protected virtual string GetAddInPath(string addIn)
		{
			return StringParser.Parse(addIn);
		}
		
		/// <summary>
		/// Returns the full path to ipyw.exe which is installed in the
		/// Python addin folder.
		/// </summary>
		string GetDefaultPythonFileName()
		{
			string path = GetAddInPath("${addinpath:ICSharpCode.PythonBinding}");
			return Path.Combine(path, DefaultPythonFileName);
		}
	}
}
