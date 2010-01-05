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

  public class StatementListNode : AstNode {
    
    public StatementListNode(NodeArgs args) : base(args) { }

    public StatementListNode(NodeArgs args, AstNodeList statements) : base(args) {
      ChildNodes.Clear();
      foreach (AstNode stmt in statements)
        AddChild(null, stmt);
    }

    public override void OnCodeAnalysis(CodeAnalysisArgs args) {
      switch (args.Phase) {
        case CodeAnalysisPhase.MarkTailCalls:
          if (IsSet(AstNodeFlags.IsTail) && ChildNodes.Count > 0)
            ChildNodes[ChildNodes.Count - 1].Flags |= AstNodeFlags.IsTail;
          break;
      }
      base.OnCodeAnalysis(args);
    }

    
    protected override void DoEvaluate(Irony.Runtime.EvaluationContext context) {
      foreach(AstNode node in ChildNodes) {
        node.Evaluate(context);
/*
        switch (context.Jump) {
          case JumpType.Goto:
            //TODO: implement GOTO
            break;
          case JumpType.Break:
          case JumpType.Continue:
          case JumpType.Return:
            return; 
          case JumpType.None:
            continue; //nothing to do, just continue
        }//switch
 */ 
      }//foreach

    }//method

  }//class

}//namespace
