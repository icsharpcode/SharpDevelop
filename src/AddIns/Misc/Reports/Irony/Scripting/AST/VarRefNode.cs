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
using System.Xml;
using Irony.Scripting.Runtime;

namespace Irony.Scripting.Ast {
  public enum AccessType {
    None,
    Read,
    Write
  }
  public class VarRefNode : AstNode {
    public string Name;
    public SlotInfo Slot;
    public AccessType Access = AccessType.Read;

    public VarRefNode(NodeArgs args, AstNode idNode) : base(args) {
      ChildNodes.Clear();
      Name = GetContent(idNode);
    }
    public VarRefNode(NodeArgs args, string name) : base(args) {
      ChildNodes.Clear();
      Name = name;
    }
    public VarRefNode(NodeArgs args) : this(args, args.ChildNodes[0]) {
    }

    public void SetupEvaluateMethod() {
      Evaluate = EvaluateDoNothing;
      if (Slot == null) return;
      int levelDiff = this.Scope.Level - Slot.Scope.Level;
      switch (Access) {
        case AccessType.None: Evaluate = EvaluateDoNothing; break; 
        case AccessType.Read :
          switch (levelDiff) {
            case 0: Evaluate = EvaluateReadLocal; break;
            case 1: Evaluate = EvaluateReadParent; break;
            default: Evaluate = EvaluateRead; break; 
          }
          break; //AccessType.Read
        case AccessType.Write:
          switch (levelDiff) {
            case 0: Evaluate = EvaluateWriteLocal; break;
            case 1: Evaluate = EvaluateWriteParent; break;
            default: Evaluate = EvaluateWrite; break; 
          }
          break; //AccessType.Write
      }//switch Access
    }

    protected void EvaluateDoNothing(EvaluationContext context) {
    }

    protected void EvaluateRead(EvaluationContext context) {
      try {
        context.CurrentResult = context.CurrentFrame.GetValue(Slot);
      } catch (RuntimeException rex) {
        rex.Location = this.Location;
        throw;
      }
    }
    protected void EvaluateReadLocal(EvaluationContext context) {
      try {
        context.CurrentResult = context.CurrentFrame.Locals[Slot.Index];
      } catch (RuntimeException rex) {
        rex.Location = this.Location;
        throw;
      }
    }
    protected void EvaluateReadParent(EvaluationContext context) {
      try {
        context.CurrentResult = context.CurrentFrame.Parent.Locals[Slot.Index];
      } catch (RuntimeException rex) {
        rex.Location = this.Location;
        throw;
      }
    }

    protected void EvaluateWrite(EvaluationContext context) {
      context.CurrentFrame.SetValue(Slot, context.CurrentResult);
    }
    protected void EvaluateWriteLocal(EvaluationContext context) {
      context.CurrentFrame.Locals[Slot.Index] = context.CurrentResult;
    }
    protected void EvaluateWriteParent(EvaluationContext context) {
      context.CurrentFrame.Parent.Locals[Slot.Index] = context.CurrentResult;
    }



    public override string ToString() {
      return Name;
    }


    protected override void XmlSetAttributes(XmlElement thisElement) {
      base.XmlSetAttributes(thisElement);
      thisElement.SetAttribute("Name", Name); 
    }

/*
    public override void OnCodeAnalysis(CodeAnalysisArgs args) {
      switch (args.Phase) {
        case CodeAnalysisPhase.Allocate:
          if (IsSet(AstNodeFlags.AllocateSlot) && !Scope.SlotDefined(Name))
            Slot = Scope.CreateSlot(Name);
          break;
        case CodeAnalysisPhase.Binding:
          Slot = Scope.FindSlot(Name);
          if (Slot == null && !IsSet(AstNodeFlags.SuppressNotDefined))
            args.Context.ReportError(this.Location, "Variable " + Name + " is not declared");
          if (Slot != null) {
            //unless suppressed, mark this ID use as RValue
            if (!IsSet(AstNodeFlags.NotRValue))
              Slot.Flags |= SlotFlags.UsedAsRValue;
            if (IsSet(AstNodeFlags.IsLValue))
              Slot.Flags |= SlotFlags.ExplicitlyAssigned;
          }
          SetupEvaluateMethod();
          break;
      }//switch
      base.OnCodeAnalysis(args);
    }
*/    


  }//class

}
