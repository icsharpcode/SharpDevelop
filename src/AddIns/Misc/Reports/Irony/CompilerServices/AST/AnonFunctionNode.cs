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

  public class AnonFunctionNode : AstNode  {
    public AstNode Parameters;
    public AstNode Body;
    protected Scope OwnerScope; // The base.Scope is for child nodes (except NameRef) - it is replaced with newly created scope; 
                                // OwnerScope saves the parent scope - it is used by NameRef node to refer to the slot in the parent scope
    public FunctionBindingInfo BindingInfo;

    public AnonFunctionNode(NodeArgs args, AstNode parameters, AstNode body) : base(args) {
      ChildNodes.Clear();
      Parameters = parameters;
      AddChild("Params", Parameters);
      Body = body;
      AddChild("Body", Body);
      foreach (VarRefNode prm in Parameters.ChildNodes)
        prm.Flags |= AstNodeFlags.AllocateSlot;
      BindingInfo = new FunctionBindingInfo(null, Parameters.ChildNodes.Count, this.Body.Evaluate, this, FunctionFlags.IsLocal);
    }

    public override void OnCodeAnalysis(CodeAnalysisArgs args) {
      switch (args.Phase) {
        case CodeAnalysisPhase.AssignScopes:
          // for all child nodes we define a new scope
          OwnerScope = base.Scope;
          base.Scope = new Scope(this, OwnerScope);
          Scope.ParamCount = Parameters.ChildNodes.Count;
          break;
        case CodeAnalysisPhase.MarkTailCalls:
          Body.Flags |= AstNodeFlags.IsTail; //unconditionally set body's tail flag
          break; 

      }//switch
      //process child nodes
      base.OnCodeAnalysis(args);
      //The following actions should be performed AFTER we process child nodes
      switch (args.Phase) {
        case CodeAnalysisPhase.Binding:
          BindingInfo.LocalCount = Scope.Slots.Count;
            //there may be two different evaluation cases: 
            // 1. Evaluation of function definition - this is when Evaluate function of this node is called. Normally this method should 
            //  set the variable associated with function name to point to the evaluation method. The value is a reference to the actual 
            //  evaluation method; it is either Evaluate method of new closure (if this function needs closure), or simply reference
            //  to static evaluation method; for definition, we need to do what is needed in this.Evaluate method.
          this.Evaluate = EvaluateOnDefine;
          // 2. Actual function call and evaluation of the body. 
          // All functions are closures, so the caller finds a closure in the function name's slot.
          // The caller invokes then closure.Evaluate, which in turn pushes a frame
          // into framestack and invokes BindingInfo.Evaluate. The BindingInfo.Evaluate points to 
          // Body.Evaluate - we provide this method as implementation to the BindingInfo constructor - 
          // (see the constructor of AnonFunctionNode(this) class).
          break; 
      }//switch
    }

    protected void EvaluateOnDefine(EvaluationContext context) {
      context.CurrentResult = new Closure(context.CurrentFrame, this, BindingInfo); // Body.Evaluate);
    }


    #region IBindingTarget Members

    public FunctionBindingInfo GetBindingInfo() {
      return BindingInfo;
    }

    #endregion
  }//class
}