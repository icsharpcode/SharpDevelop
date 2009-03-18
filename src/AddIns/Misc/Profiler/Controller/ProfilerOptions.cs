// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.Profiler.Controller
{
	/// <summary>
	/// Holds all settings for the profiler chosen by the user.
	/// </summary>
	public class ProfilerOptions
	{
		/// <summary>
		/// Defines a default size of the shared memory.
		/// </summary>
		public const int SHARED_MEMORY_SIZE = 64 * 1024 * 1024; // 64 mb
		
		bool enableDC;
		int sharedMemorySize;
		
		/// <summary>
		/// Gets whether data collection is enabled during profiling sessions.
		/// </summary>
		public bool EnableDC {
			get { return enableDC; }
		}
		
		/// <summary>
		/// Gets the size of the shared memory.
		/// </summary>
		public int SharedMemorySize {
			get { return sharedMemorySize; }
		}
		
		/// <summary>
		/// Creates new ProfilerOptions using the selected settings.
		/// </summary>
		public ProfilerOptions(bool enableDC, int sharedMemorySize)
		{
			this.enableDC = enableDC;
			this.sharedMemorySize = sharedMemorySize;
		}
		
		/// <summary>
		/// Creates default ProfilerOptions.
		/// </summary>
		public ProfilerOptions()
			: this(true, SHARED_MEMORY_SIZE)
		{
		}
	}
}
