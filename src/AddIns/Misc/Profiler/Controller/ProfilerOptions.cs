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
		/// Defines the default size of the shared memory.
		/// </summary>
		public const int SHARED_MEMORY_SIZE = 64 * 1024 * 1024; // 64 mb
		
		bool enableDC;
		bool dotNotProfileDotNetInternals;
		bool combineRecursiveFunction;
		int sharedMemorySize;
		
		/// <summary>
		/// Gets whether .NET internal calls are profiled or not.
		/// </summary>
		public bool DoNotProfileDotNetInternals {
			get { return dotNotProfileDotNetInternals; }
		}
		
		/// <summary>
		/// Gets whether recursive functions calls are combined or not.
		/// </summary>
		public bool CombineRecursiveFunction {
			get { return combineRecursiveFunction; }
		}
		
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
		public ProfilerOptions(bool enableDC, int sharedMemorySize, bool profileDotNetInternals, bool combineRecursiveFunction)
		{
			this.enableDC = enableDC;
			this.sharedMemorySize = sharedMemorySize;
			this.dotNotProfileDotNetInternals = profileDotNetInternals;
			this.combineRecursiveFunction = combineRecursiveFunction;
		}
		
		/// <summary>
		/// Creates default ProfilerOptions.
		/// </summary>
		public ProfilerOptions()
			: this(true, SHARED_MEMORY_SIZE, false, false)
		{
		}
	}
}
