// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using Debugger;
using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Debugging;

namespace ICSharpCode.SharpDevelop.Services
{
	public class DebuggingOptions: Debugger.Options
	{
		public static DebuggingOptions Instance {
			get {
				return PropertyService.Get("DebuggingOptions", new DebuggingOptions());
			}
		}
		
		bool iCorDebugVisualizerEnabled;
		bool showValuesInHexadecimal;
		bool showArgumentNames;
		bool showArgumentValues;
		bool showExternalMethods;
		
		public bool ICorDebugVisualizerEnabled {
			get { return iCorDebugVisualizerEnabled; }
			set { iCorDebugVisualizerEnabled = value; }
		}
		
		public bool ShowValuesInHexadecimal {
			get { return showValuesInHexadecimal; }
			set { showValuesInHexadecimal = value; }
		}
		
		public bool ShowArgumentNames {
			get { return showArgumentNames; }
			set { showArgumentNames = value; }
		}
		
		public bool ShowArgumentValues {
			get { return showArgumentValues; }
			set { showArgumentValues = value; }
		}
		
		public bool ShowExternalMethods {
			get { return showExternalMethods; }
			set { showExternalMethods = value; }
		}
	}
}
