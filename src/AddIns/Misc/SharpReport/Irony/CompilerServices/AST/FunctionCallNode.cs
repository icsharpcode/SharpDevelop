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
using Irony.Runtime;

namespace Irony.Compiler.AST {

  //Note: currently does not support dynamic binding to target. Function name is a constant symbol. 
  // TODO: extend to support dynamic binding - for ex in Scheme, the node in function position can be any expression
  // that evaluates to a function
  public class FunctionCallNode : AstNode {
    public VarRefNode NameRef;
    public AstNodeList Arguments;
    private bool _isTail;
    //If the target method is fixed and statically bound (runtime library function for ex),
    // then this field contains the binding info for the target method
    public FunctionBindingInfo FixedTargetInfo;

    public FunctionCallNode(NodeArgs args, VarRefNode name, AstNodeList arguments) : base(args) {
      ChildNodes.Clear();
      NameRef = name;
      NameRef.Flags |= AstNodeFlags.SuppressNotDefined;
      AddChild("Name", NameRef);
      Arguments = arguments;
      foreach (AstNode arg in Arguments) 
        AddChild("Arg", arg);
    }//constructor

    public override void OnCodeAnalysis(CodeAnalysisArgs args) {
      //process child nodes first, so that NameRef is processed
      base.OnCodeAnalysis(args);
      switch (args.Phase) {
        case CodeAnalysisPhase.Binding:
          if (NameRef.Slot != null) {
            NameRef.Slot.Flags |= SlotFlags.UsedAsCallTarget;
          } else {
            // Slot does not exist so it is not locally-defined method. Let's try global/library function
            // TODO: implement support for library references at scope level, so that all "import.." statements are checked 
            //  before checking global methods
            FixedTargetInfo = args.Context.Runtime.GetFunctionBindingInfo(NameRef.Name, Arguments);
            if (FixedTargetInfo == null) {
              args.Context.ReportError(this.Location, "Method not found: {0}", NameRef.Name);
              return;
            }
          }//else
          break;
        case CodeAnalysisPhase.MarkTailCalls:
          _isTail = IsSet(AstNodeFlags.IsTail) && this.Scope.Level > 0;
          break; 
        case CodeAnalysisPhase.Optimization:
          if (this.FixedTargetInfo != null)
            this.Evaluate = InvokeFixed;
          else
            this.Evaluate = InvokeDynamic;
          break;
      }//switch
    }//method

    protected void InvokeDynamic(EvaluationContext context) {
      NameRef.Evaluate(context);
      Closure target;
      try {
        target = (Closure)context.CurrentResult;
        if (target.MethodName == null)
          target.MethodName = NameRef.Name;
      } catch (InvalidCastException castExc) {
        throw new Irony.Runtime.RuntimeException("Method [" + NameRef.Name + "] not found or method reference is not set.", castExc, NameRef.Location);
      } catch (NullReferenceException) {
        throw new Irony.Runtime.RuntimeException("Method reference is not set in variable " + NameRef.Name, null, NameRef.Location);
      }

      EvaluateArgs(context, target.BindingInfo);
      if (_isTail) {
        context.Tail = target;
        return; 
      } 
      //execute non-tail call
      target.Evaluate(context);
      if (context.Tail == null) return;
      //check returning tail
      while (context.Tail != null) {
        Closure tail = context.Tail;
        context.Tail = null;
        tail.Evaluate(context);
      }
      context.CallArgs = null;
    }
    protected void InvokeFixed(EvaluationContext context) {
      EvaluateArgs(context, FixedTargetInfo);
      FixedTargetInfo.Evaluate(context);
      //check returning tail
      if (context.Tail == null) return;  
      while (context.Tail != null) {
        Closure tail = context.Tail;
        context.Tail = null;
        tail.Evaluate(context);
      }
      context.CallArgs = null;
    }
  

    private void EvaluateArgs(EvaluationContext context, FunctionBindingInfo targetInfo) {
      object[] values = context.CreateCallArgs(this.Arguments.Count);
      //Just for perfomance, we implement two cases separately
      if (targetInfo.IsSet(FunctionFlags.HasParamArray)) {
        //with params array
        for (int i = 0; i < targetInfo.ParamCount-1; i++)  {
          Arguments[i].Evaluate(context);
          values[i] = context.CurrentResult;
        }
        //Now combine all remaining arguments into one array and put it into the last element
        int startIndex = targetInfo.ParamCount - 1;
        int arrayLen = Arguments.Count - startIndex;
        object[] arrayArgs = new object[arrayLen];
        for (int i = 0; i < arrayLen; i++) {
          Arguments[startIndex + i].Evaluate(context);
          arrayArgs[i] = context.CurrentResult;
        }
        values[startIndex] = arrayArgs;

      } else {
        //No params array
        for (int i = 0; i < Arguments.Count; i++)  {
          Arguments[i].Evaluate(context);
          values[i] = context.CurrentResult;
        }
      }

      context.CallArgs = values;
    }//method

    public override string ToString() {
      string result = "call " + NameRef.Name;
      if (!string.IsNullOrEmpty(Role))
        result = Role + ": " + result; 
      return result; 
    }


  }//class
}
