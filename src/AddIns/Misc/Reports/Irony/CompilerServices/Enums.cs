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

namespace Irony.Compiler {

  public enum TokenCategory {
    Literal, 
    Content,
    Outline, //newLine, indent, dedent
    Comment,
    Error,
  }

  public enum TokenMatchMode {
    ByValue = 1,
    ByType = 2,
    ByValueThenByType = ByValue | ByType,
  }

  //Operator associativity types
  public enum Associativity {
    Left,
    Right,
    Neutral  //don't know what that means 
  }

  public enum TermOptions {
    None = 0,
    IsOperator = 0x01,
    IsGrammarSymbol = 0x02,
    IsOpenBrace  = 0x04,
    IsCloseBrace = 0x08,
    IsBrace = IsOpenBrace | IsCloseBrace,
    IsConstant    = 0x10,
    IsPunctuation = 0x20,
    IsDelimiter   = 0x40, 
    IsList        = 0x80,
    IsStarList   = 0x100,   //Special case of list with 0 or more elements separated by delimiters. Produced by MakeStarRule method
    IsNonGrammar= 0x0200,  // if set, parser would eliminate the token from the input stream; terms in Grammar.NonGrammarTerminals have this flag set
    IsTransient = 0x0400,  // Transient non-terminal - should be removed from the AST tree. 
    
  }


}
