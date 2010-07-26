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
  public class AssigmentNode : AstNode {
    public VarRefNode Identifier;
    public AstNode Expression;

    public AssigmentNode(NodeArgs args)  : this(args, args.ChildNodes[0], args.ChildNodes[2]) {  }

    public AssigmentNode(NodeArgs args, AstNode id, AstNode expression) : base(args) {
      ChildNodes.Clear();
      Identifier = id as VarRefNode;
      if (Identifier == null) //id might be a simple token
        Identifier = new VarRefNode(args, id);
      if (Identifier == null && id is Token) {
        args.Context.ReportError(id.Location, "Expected identifier.");
      }
      Identifier.Flags |= AstNodeFlags.AllocateSlot | AstNodeFlags.NotRValue;
      Identifier.Access = AccessType.Write;
      AddChild("Name", Identifier);
      Expression = expression;
      AddChild("Expr", Expression);
    }

    protected override void DoEvaluate(EvaluationContext context) {
      Expression.Evaluate(context);
      Identifier.Evaluate(context); //writes the value into the slot
    }

  }
}
