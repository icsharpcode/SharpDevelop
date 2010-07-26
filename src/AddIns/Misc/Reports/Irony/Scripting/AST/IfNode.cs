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
using Irony.Scripting.Runtime;

namespace Irony.Scripting.Ast {
  public class IfNode : AstNode {
    public AstNode Test;
    public AstNode IfTrue;
    public AstNode IfFalse;

    public IfNode(NodeArgs args, AstNode test, AstNode ifTrue, AstNode ifFalse): base(args) {
      ChildNodes.Clear();

      Test = test;
      AddChild("Test", Test);
      
      IfTrue = ifTrue;
      if (IfTrue.IsEmpty()) 
        IfTrue = null;
      AddChild("IfTrue", IfTrue);

      IfFalse = ifFalse;
      if (IfFalse.IsEmpty()) IfFalse = null;
      AddChild("IfFalse", IfFalse);
    }

    public override void OnCodeAnalysis(CodeAnalysisArgs args) {
      switch (args.Phase) {
        case CodeAnalysisPhase.MarkTailCalls:
          if (IsSet(AstNodeFlags.IsTail)) {
            if (IfTrue != null)
              IfTrue.Flags |= AstNodeFlags.IsTail;
            if (IfFalse != null)
              IfFalse.Flags |= AstNodeFlags.IsTail;
          }
          break;
      }
      base.OnCodeAnalysis(args);
    }

    protected override void DoEvaluate(EvaluationContext context) {
      Test.Evaluate(context);
      if (context.Runtime.IsTrue(context.CurrentResult)) {
        if (IfTrue != null)    IfTrue.Evaluate(context);
      } else {
        if (IfFalse != null)   IfFalse.Evaluate(context);
      }
    }
  }//class

}//namespace
