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
  public class CondClauseNode : AstNode {
    public AstNode Test;
    public StatementListNode Expressions;

    public CondClauseNode(NodeArgs args, AstNode test, StatementListNode expressions) :base(args) {
      ChildNodes.Clear();
      this.Role = "Clause";
      Test = test;
      Test.Role = "Test";
      ChildNodes.Add(Test);
      Expressions = expressions;
      Expressions.Role = "Command";
      ChildNodes.Add(Expressions);
    }

    public override void OnCodeAnalysis(CodeAnalysisArgs args) {
      switch (args.Phase) {
        case CodeAnalysisPhase.MarkTailCalls:
          if (IsSet(AstNodeFlags.IsTail))
            Expressions.Flags |= AstNodeFlags.IsTail;
          break;
      }
      base.OnCodeAnalysis(args);
    }

    protected override void DoEvaluate(EvaluationContext context) {
      Expressions.Evaluate(context);
    }
  
  }//class

}//namespace
