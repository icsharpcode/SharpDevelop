/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 27.07.2006
 * Time: 21:17
 */

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
		bool useTipOfTheDay;
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
		/// Gets/Sets whether the tip of the day is supported.
		/// The default is false.
		/// </summary>
		public bool UseTipOfTheDay {
			get {
				return useTipOfTheDay;
			}
			set {
				useTipOfTheDay = value;
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
