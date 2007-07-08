// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.ObjectModel;

namespace ICSharpCode.SharpDevelop.Sda
{
	/// <summary>
	/// This class contains properties to control how the SharpDevelop
	/// workbench is being run.
	/// </summary>
	[Serializable]
	public sealed class WorkbenchSettings
	{
		bool runOnNewThread = true;
		Collection<string> fileList = new Collection<string>();
		
		/// <summary>
		/// Gets/Sets whether to create a new thread to run the workbench on.
		/// The default value is true.
		/// </summary>
		public bool RunOnNewThread {
			get {
				return runOnNewThread;
			}
			set {
				runOnNewThread = value;
			}
		}
		
		/// <summary>
		/// Put files to open at workbench startup into this collection.
		/// </summary>
		public Collection<string> InitialFileList {
			get {
				return fileList;
			}
		}
	}
}
