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

namespace Irony.Compiler.Nlalr {
  public enum StackElementType {
    Node,
    Marker,
  }
  public struct ParserStackElement {
    public readonly StackElementType Type;
    public readonly AstNode Node;
    public readonly ParserState State;
    public ParserStackElement(StackElementType type, AstNode node,  ParserState state) {
      Type = type;
      Node = node;
      State = state;
    }
    public override string ToString() {
      switch (Type) {
        case StackElementType.Marker: return "!Marker, state " + State.Name;
        default: return State.Name + " " + Node;
      }
    }//method
  }


  public class ParserStack  {
    private ParserStackElement[] _data = new ParserStackElement[InitialSize];

    public int Count {
      [System.Diagnostics.DebuggerStepThrough]
      get { return _count; }
    } int  _count; //actual count of elements currently in stack

    public ParserStackElement this[int index] {
      [System.Diagnostics.DebuggerStepThrough]
      get { return _data[index]; }
    }
    public ParserStackElement Top {
      [System.Diagnostics.DebuggerStepThrough]
      get { return this[Count - 1]; }
    }
    public void Push(AstNode node, ParserState state) {
      if (_count == _data.Length) 
        ExtendData();
      _data[_count] = new ParserStackElement(StackElementType.Node, node, state);
      _count++;
    }
    public void Pop(int popCount) {
      _count -= popCount;
    }
    public void Reset() {
      //replace data array with empty array, to clear all references
      _data = new ParserStackElement[InitialSize];
      _count = 0;
    }
    private void ExtendData() {
      ParserStackElement[] newData = new ParserStackElement[_data.Length + SizeIncrement];
      Array.Copy(_data, newData, _data.Length);
      _data = newData;
    }

    private const int InitialSize = 100;
    private const int SizeIncrement = 100;
  }//class


}
