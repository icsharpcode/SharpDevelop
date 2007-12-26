// SharpDevelop samples
// Copyright (c) 2007, AlphaSierraPapa
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are
// permitted provided that the following conditions are met:
//
// - Redistributions of source code must retain the above copyright notice, this list
//   of conditions and the following disclaimer.
//
// - Redistributions in binary form must reproduce the above copyright notice, this list
//   of conditions and the following disclaimer in the documentation and/or other materials
//   provided with the distribution.
//
// - Neither the name of the SharpDevelop team nor the names of its contributors may be used to
//   endorse or promote products derived from this software without specific prior written
//   permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS &AS IS& AND ANY EXPRESS
// OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER
// IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
// OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.Diagnostics;
using ICSharpCode.Core;

namespace ICSharpCode.NAnt
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
		public static string NAntFileName {
			get {
				return (string)Properties.Get(NAntFileNameProperty, DefaultNAntFileName);
			}
			set {
				if (String.IsNullOrEmpty(value)) {
					Properties.Set(NAntFileNameProperty, DefaultNAntFileName);
				} else {
					Properties.Set(NAntFileNameProperty, value);
				}
			}
		}
		
		/// <summary>
		/// Gets the NAnt command line arguments.
		/// </summary>
		public static string NAntArguments {
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
		public static bool Verbose {
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
		public static bool ShowLogo {
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
		public static bool Quiet {
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
		public static bool DebugMode {
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
