// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace ICSharpCode.NAntAddIn
{
	/// <summary>
	/// The NAnt add-in options.
	/// </summary>
	public class AddInOptions
	{
		public static readonly string OptionsProperty = "NAntAddIn.Options";

		#region Property names
		public static readonly string NAntFileNameProperty = "NAntFileName";
		public static readonly string NAntArgumentsProperty = "NAntArguments";
		public static readonly string VerboseProperty = "Verbose";
		public static readonly string ShowLogoProperty = "ShowLogo";
		public static readonly string QuietProperty = "Quiet";
		public static readonly string DebugModeProperty = "DebugMode";
		#endregion
		
		#region Property defaults	
		public static readonly string DefaultNAntFileName = "nant.exe";
		#endregion
		
		static Properties properties;

		static AddInOptions()
 		{
			properties = PropertyService.Get(OptionsProperty, new Properties());
		}

 		static Properties Properties {
			get {
				Debug.Assert(properties != null);
				return properties;
 			}
		}

		#region Properties
		
		/// <summary>
		/// Gets the NAnt executable filename.  
		/// </summary>
		/// <remarks>
		/// This is either the full filename including path
		/// or just the name of the executable (nant.exe) in which
		/// case it is assumed that NAnt is on the path.
		/// </remarks>
		public static string NAntFileName
		{
			get {
				return (string)Properties.Get(NAntFileNameProperty, DefaultNAntFileName);
			}
			
			set {
				Properties.Set(NAntFileNameProperty, value);
			}
		}
		
		/// <summary>
		/// Gets the NAnt command line arguments.
		/// </summary>
		public static string NAntArguments
		{
			get {
				return (string)Properties.Get(NAntArgumentsProperty, String.Empty);
			}
			
			set {
				Properties.Set(NAntArgumentsProperty, value);
			}
		}		
		
		/// <summary>
		/// Gets the NAnt -verbose setting.
		/// </summary>
		public static bool Verbose
		{
			get {
				return (bool)Properties.Get(VerboseProperty, false);
			}
			
			set {
				Properties.Set(VerboseProperty, value);
			}
		}	
		
		/// <summary>
		/// Gets the NAnt show logo setting.
		/// </summary>
		public static bool ShowLogo
		{
			get {
				return (bool)Properties.Get(ShowLogoProperty, false);
			}
			
			set {
				Properties.Set(ShowLogoProperty, value);
			}
		}
		
		/// <summary>
		/// Gets the NAnt -quiet setting.
		/// </summary>
		public static bool Quiet
		{
			get {
				return (bool)Properties.Get(QuietProperty, false);
			}
			
			set {
				Properties.Set(QuietProperty, value);
			}
		}		
		
		/// <summary>
		/// Gets the NAnt -debug setting.
		/// </summary>
		public static bool DebugMode
		{
			get {
				return (bool)Properties.Get(DebugModeProperty, false);
			}
			
			set {
				Properties.Set(DebugModeProperty, value);
			}
		}
		
		#endregion
	}
}
