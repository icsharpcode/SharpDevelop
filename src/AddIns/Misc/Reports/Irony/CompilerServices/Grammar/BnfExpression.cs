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
using System.Diagnostics;

namespace Irony.CompilerServices {

  //BNF expressions are represented as OR-list of Plus-lists of BNF terms
  internal class BnfExpressionData : List<BnfTermList> { }

  public class BnfExpression : BnfTerm {

    public BnfExpression(BnfTerm element): this() {
      Data[0].Add(element);
    }
    public BnfExpression() : base(null) {
      Data = new BnfExpressionData();
      Data.Add(new BnfTermList());
    }

    #region properties: Data
    internal BnfExpressionData Data;
    #endregion

    #region overrides: ToString()
    private string _toString;
    public override string ToString() {
      if (_toString != null) return _toString;
      try {
        string[] pipeArr = new string[Data.Count];
        for (int i = 0; i < Data.Count; i++) {
          BnfTermList seq = Data[i];
          string[] seqArr = new string[seq.Count];
          for (int j = 0; j < seq.Count; j++)
            seqArr[j] = seq[j].ToString();
          pipeArr[i] = String.Join("+", seqArr);
        }
        _toString = String.Join("|", pipeArr);
        return _toString; 
      } catch(Exception e) {
        return "(error: " + e.Message + ")";
      }
    } 
    #endregion

    #region Implicit cast operators
    public static implicit operator BnfExpression(string symbol) {
      return new BnfExpression(Grammar.CurrentGrammar.Symbol(symbol));
    }
    //It seems better to define one method instead of the following two, with parameter of type BnfElement -
    // but that's not possible - it would be a conversion from base type of BnfExpression itself, which
    // is not allowed in c#
    public static implicit operator BnfExpression(Terminal term) {
      return new BnfExpression(term);
    }
    public static implicit operator BnfExpression(NonTerminal nonTerminal) {
      return new BnfExpression(nonTerminal);
    }
    #endregion


  }//class

}//namespace
