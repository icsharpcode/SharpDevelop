#region License
/* **********************************************************************************
 * Copyright (c) Roman Ivantsov
 * This source code is subject to terms and conditions of the MIT License
 * for Irony. A copy of the license can be found in the License.txt file
 * at the root of this distribution. 
 * By using this source code in any fashion, you are agreeing to be bound by the terms of the 
 * MIT License.
 * You must not remove this notice from this software.
 * **********************************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;

namespace Irony.Scripting.Runtime {
  //Slim, fast locking facility. Based on SpinWaitLock class found here: 
  //  http://msdn.microsoft.com/en-us/magazine/cc163726.aspx
  // NOTE from original author: This is a value type so it works very efficiently when used as
  // a field in a class. Avoid boxing this or you will lose thread safety!
  public struct SlimLock {
    const Int32 StateFree = 0;
    const Int32 StateLocked = 1;
    Int32 _lockState; // Defaults to 0 = StateFree

    public void Enter() {
      // Thread.BeginCriticalRegion(); - needed only for SQL 2005 libraries
      while (true) {
        // If resource available, set it to in-use and return
        if (Interlocked.Exchange(ref _lockState, StateLocked) == StateFree) 
          return;

        // Efficiently spin, until the resource looks like it might 
        // be free. NOTE: Just reading here (as compared to repeatedly 
        // calling Exchange) improves performance because writing 
        // forces all CPUs to update this value
        while (Thread.VolatileRead(ref _lockState) == StateLocked) 
          StallThread();
      }
    }

    public void Exit() {
      // Mark the resource as available
      Interlocked.Exchange(ref _lockState, StateFree);
      //Thread.EndCriticalRegion(); - needed only for SQL 2005 libraries
    }

    private static void StallThread() {
      if (IsSingleCpuMachine)
        SwitchToThread();   // On a single-CPU system, spinning does no good, so yield the thread
      else
        Thread.SpinWait(1);// Multi-CPU system might be hyper-threaded, let other thread run
    }

    [DllImport("kernel32", ExactSpelling = true)]
    private static extern void SwitchToThread();
    public static readonly Boolean IsSingleCpuMachine = (Environment.ProcessorCount == 1);
  }

}//namespace
