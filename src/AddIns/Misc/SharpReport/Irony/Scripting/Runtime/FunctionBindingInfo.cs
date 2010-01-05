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
using Irony.Scripting.Ast;

namespace Irony.Scripting.Runtime {

  [Flags]
  public enum FunctionFlags {
    None    = 0, 

    IsLocal = 0x01,   // function defined in current module
    IsImported = 0x02,   // same language but imported from another module
    IsExternal = 0x04,   // is interop method

    NeedsClosure = 0x010,   // 
    IsVoid = 0x080,

    HasParamArray = 0x100,      // The last argument is a param array
    TypeBasedDispatch = 0x200,  //uses dynamic dispatch based on types
  }


  //Used for binding to runtime library functions
  public class FunctionBindingInfo {
    public string Name;
    public NodeEvaluate Evaluate;
    public int ParamCount;  //-1 means "any"
    public int LocalCount;
    public FunctionFlags Flags; 
    public AstNode TargetNode; 
    public FunctionBindingInfo(string name, int paramCount, NodeEvaluate evaluate, AstNode targetNode, FunctionFlags flags) {
      Name = name;
      Evaluate = evaluate;
      ParamCount = paramCount;
      LocalCount = ParamCount; //by default
      TargetNode = targetNode;
      Flags = flags; 
    }
    public bool IsSet(FunctionFlags flag) {
      return (Flags & flag) != 0;
    }
  }

  public class FunctionBindingList : List<FunctionBindingInfo> { }
  public class FunctionBindingTable : Dictionary<string, FunctionBindingList> {
    public void Add(string name, FunctionBindingInfo info) {
      FunctionBindingList list;
      if (!TryGetValue(name, out list)) 
        this[name] = list = new FunctionBindingList();
      list.Add(info);
    }
    public FunctionBindingInfo Find(string name, int paramCount) {
      FunctionBindingList list;
      if (!TryGetValue(name, out list))
        return null;
      foreach (FunctionBindingInfo info in list) {
        if (info.ParamCount == paramCount)
          return info;
        //Functions with variable-length parameter lists (like methods with "params" keyword in c#)
        if (info.ParamCount < paramCount && info.IsSet(FunctionFlags.HasParamArray))
          return info; 

      }
      return null; 
    }//method
  }//class


}//namespace
