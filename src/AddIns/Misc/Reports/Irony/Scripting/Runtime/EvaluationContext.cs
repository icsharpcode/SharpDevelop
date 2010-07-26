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
using System.Text;
using Irony.CompilerServices;
using Irony.Scripting.Ast;

namespace Irony.Scripting.Runtime {
  public enum JumpType {
    None = 0,
    Break,
    Continue,
    Return,
    Goto,
    Exception,
  }

  public class EvaluationContext  {
    public LanguageRuntime Runtime;

    public Frame CurrentFrame;
    public object CurrentResult;
    //Two slots reserved for arguments of binary and unary operators
    public object Arg1;
    public object Arg2;

    //The following are not used yet
    public JumpType Jump = JumpType.None;
    public AstNode GotoTarget;

    public Closure Tail;

    //contains call args for a function call; it is passed to the new frame when the call is made. 
    // CallArgs are created by the caller based on number of arguments in the call.
    // Additionally, we reserve extra space for local variables so that this array can be used directly as local variables 
    // space in a new frame. 
    public object[] CallArgs; 

    public EvaluationContext(LanguageRuntime runtime, AstNode rootNode) {
      Runtime = runtime;
      ResizeUnassignedArray(64); 
      CallArgs = CreateCallArgs(rootNode.Scope.Slots.Count);
      PushFrame("root", rootNode, null);
    }

    public void PushFrame(string methodName, AstNode node, Frame parent) {
      CurrentFrame = new Frame(methodName, node, CurrentFrame, parent, CallArgs);
    }
    public void PopFrame() {
      CurrentFrame = CurrentFrame.Caller;
    }

    //Used exclusively for debugging
    public StackTrace CallStack {
      get { return new StackTrace(this); }
    }

    #region FrameData/CallArgs initialization
    public int LocalsPreallocateSize = 8;
    // We use Array.Copy as a fast way to initialize local data with Unassigned value
    //  "When copying elements between arrays of the same types, Array.Copy performs a single range check before the transfer 
    //    followed by a ultrafast memmove byte transfer." (from http://www.codeproject.com/KB/dotnet/arrays.aspx)
    public object[] CreateCallArgs(int argCount) {
      int count = argCount + LocalsPreallocateSize;
      if (count > _arrayOfUnassigned.Length)
        ResizeUnassignedArray(count);
      object[] args = new object[count];
      Array.Copy(_arrayOfUnassigned, args, count);
      return args;
    }

    //This array is used for initializing parameters/local variables arrays, see EvaluationContext.CreateCallArgs method
    private object[] _arrayOfUnassigned;
    private void ResizeUnassignedArray(int newSize) {
      object[] tmp = new object[newSize];
      for (int i = 0; i < tmp.Length; i++)
        tmp[i] = Unassigned.Value;
      lock (this) {
        //check if we still need to resize it - other thread may have already done the job while this thread was waiting for the lock
        if (_arrayOfUnassigned != null && newSize <= _arrayOfUnassigned.Length) return;
        _arrayOfUnassigned = tmp;
      } //lock
    }

    #endregion

  }//class

}
