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

namespace Irony.Scripting {

  [Flags]
  public enum SlotFlags {
    None = 0,
    UsedAsRValue = 0x01,     //the value is used in expression  or passed as a parameter to a call
    UsedAsCallTarget = 0x02,     //the value is used as a call target
    ExplicitlyAssigned = 0x04,   // the value is explicitly set by assignment operator - used for functions' slots
    IsLocalFunction = 0x08,     //the value referenced by slot is a function defined in this module

    IsStatic = 0x020,         //the function call target is a fixed method, either defined in current module or is a global/external method
    IsExternal = 0x040,       //the function call target is an external function
  }


  public class SlotInfo  {
    public string Name;
    public Scope Scope;
    public int Index;
    public SlotFlags Flags;
    public int AssignmentCount;
    public object ConstValue;   //the constant value of the variable, in case it is a constant. 
    public Type Type; //for use in typed languages or for type inference
    //we allow slot creation only through Scope.CreateSlot method
    internal SlotInfo(Scope scope, string name, int index) {
      Scope = scope;
      Name = name;
      Index = index;
    }
    public bool IsSet(SlotFlags flag) {
      return (Flags & flag) != 0;
    }
  }

  public class SlotInfoTable : Dictionary<string, SlotInfo> { }

  public class Scope {
    public AstNode Node;
    public Scope Parent; //lexical parent, scope container
    public int Level;
    public int ParamCount; 
    public readonly SlotInfoTable Slots = new SlotInfoTable();
    public Scope(AstNode node, Scope parent) {
      Node = node;
      Parent = parent;
      Level = (short)(parent == null ? 0 : parent.Level + 1);
    }
    public SlotInfo FindSlot(string name) {
      SlotInfo slot;
      if (Slots.TryGetValue(name, out slot))
        return slot;
      else if (Parent != null) {
        //search parent scopes
        slot = Parent.FindSlot(name);
        //if succeeded, and slot is not in global scope, set the flag indicating that "owner" function uses variables outside its scope
        if (slot != null && slot.Scope.Level > 0)
          this.Node.Flags |= AstNodeFlags.UsesOuterScope;
        return slot; 
      } else
        return null;
    }//method

    // inefficient, but it is used only by StackTrace for diagnostics, so performance is not an issue
    public SlotInfo GetSlotByIndex(int index) {
      foreach (SlotInfo slot in Slots.Values)
        if (slot.Index == index) return slot;
      return null; 
    }
    
    public SlotInfo CreateSlot(string name) {
      SlotInfo slot = new SlotInfo(this, name, Slots.Count);
      Slots.Add(name, slot);
      return slot;
    }
    public bool SlotDefined(string name) {
      return Slots.ContainsKey(name); 
    }
  }//class

}//namespace
