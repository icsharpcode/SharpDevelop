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
using Irony.CompilerServices;

namespace Irony.Scripting.Runtime {

  /// <summary>
  /// The StackTrace class provides visualization of the dynamic call stack for debugging purposes.
  /// The instance of this class is available through EvaluationContext's StackTrace property
  /// </summary>
  public class StackTrace : List<StackTraceElement> {
    EvaluationContext _context;
    public StackTrace(EvaluationContext context) {
      _context = context;
      Frame frame = context.CurrentFrame;
      while (frame != null) {
        Add(new StackTraceElement(frame));
        frame = frame.Caller;
      }
    }
    
    public override string ToString() {
      if (Count == 0) return "(empty)";
      return "at " + this[Count - 1].ToString() + ", " + Count + " frames";
    }

    public string AsText {
      get {
        StringBuilder bld = new StringBuilder();
        foreach (StackTraceElement elem in this)
          bld.AppendLine(elem.ToString());
        return bld.ToString();
      }
    }//AsText
  
  }//StackTrace class

  public class StackTraceElement {
    Frame _frame; 

    public StackTraceElement(Frame frame) {
      _frame = frame; 
    }

    public override string ToString() {
      StringBuilder bld = new StringBuilder();
      bld.Append(_frame.MethodName);
      bld.Append("(");
      string paramList = GetLocalList(0, _frame.Node.Scope.ParamCount - 1);
      bld.Append(paramList);
      bld.Append(")");
      return bld.ToString();
    }
    public string Locals {
      get {
        string result = GetLocalList(_frame.Node.Scope.ParamCount, _frame.Node.Scope.Slots.Count - 1);
        return result; 
      }
    }

    private string GetLocalList(int fromIndex, int toIndex) {
      if (toIndex < fromIndex) return string.Empty; 
      StringBuilder bld = new StringBuilder();
      for (int i = fromIndex; i <= toIndex; i++) {
        SlotInfo slot = _frame.Node.Scope.GetSlotByIndex(i);
        bld.Append(slot.Name);
        bld.Append("=");
        bld.Append(GetLocalValueAt(i));
        if (i < toIndex)
          bld.Append(", "); 
      }
      return bld.ToString();
    }//method

    private string GetLocalValueAt(int index) {
      object value = _frame.Locals[index];
      //convert to string
      string svalue = (value == null ? "null" : value.ToString());
      //constrain length so that we don't overflow the line
      if (svalue.Length > 20) {
        svalue = svalue.Substring(0, 15) + "...";
      }
      if (svalue.Contains('\n')) {
        svalue = svalue.Replace("\r\n", " ");
        svalue = svalue.Replace("\n", " ");
      }
      //if the original value is string, enclose it in quotes
      if (value is string) 
        svalue = '"' + svalue + '"';
      return svalue;
    }//method
  }//class


}//namespace
