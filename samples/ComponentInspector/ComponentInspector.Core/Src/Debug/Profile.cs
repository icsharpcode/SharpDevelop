// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Runtime.InteropServices;
using CORDBLib_1_0;
using NoGoop.Util;
using NoGoop.Win32;

namespace NoGoop.Debug
{
	// From FrameworkSDK/Include/corprof.idl
	[ComImport()]
	[GuidAttribute("28B5557D-3F3F-48b4-90B2-5F9EEA2F6C48")]
	public interface ICorProfilerInfo
	{
		ICorDebug GetInprocInspectionInterface();
		ICorDebugThread GetInprocInspectionIThisThread();

		// returns profiler context
		int BeginInprocDebugging(bool thisThreadOnly);
		void EndInprocDebugging(int profilerContext);
	}

	[ComImport()]
	[GuidAttribute("176FBED1-A55C-4796-98CA-A9DA0EF883E7")]
	public interface ICorProfilerCallback
	{
		void Initialize(ICorProfilerInfo pICorProfilerInfoUnk);
		void Shutdown();
	}

	// This is a hook to the profiler so we can get a handle to the
	// in process debugger
	[ProgId(Profile.PROGID)]
	[GuidAttribute(Profile.GUIDSTR)]
	[ComVisible(true)]    
	public class Profile : ICorProfilerCallback
	{
		public const String                 PROGID = 
			Constants.COMPINSP_PROGID + ".Profile";

		public const String                 GUIDSTR = 
			"96CFA810-7D30-422d-A011-110241DA566A";

		internal Profile()
		{
			if (!Windows.SetEnvironmentVariable("Cor_Enable_Profiling", "true"))
				throw new Exception("Error setting environment variable");
			if (!Windows.SetEnvironmentVariable("Cor_Profiler", PROGID))
				throw new Exception("Error setting environment variable");
		}

		public void Initialize(ICorProfilerInfo pICorProfilerInfoUnk)
		{
			TraceUtil.WriteLineInfo(this, "Profiling initialized");
		}

		public void Shutdown()
		{
		}
	}
}




