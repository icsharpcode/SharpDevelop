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

namespace Irony.CompilerServices {

  #region Comments
  //  Token filter is a token preprocessor that operates on a token stream between scanner and parser:
  //    scanner -> (token filters)-> parser
  // Token stream from scanner output is fed into a chain of token filters that add/remove/modify tokens
  // in the stream before it gets to the parser. Some tasks that come up in scanning and parsing are best
  // handled by such an intermediate processor. Examples:
  //  1. Macro expansion
  //  2. Conditional compilation clauses
  //  3. Verifying matching braces/parenthesis. See more about this in comments to BraceMatchTokenFilter class.
  //  4. Handling commented-out blocks. Scheme allows commenting out entire blocks of code using "#;" prefix followed by
  //     well-formed datum. This type of comments cannot be handled by scanner as it requires parser-like processing
  //     of the stream to locate the end of the block. At the same time parser is not a good place to handle this either,
  //     as it would require defining optional "commented block" element everywhere in the grammar. 
  //     Token filter is an ideal place for implementing this task - after scanning but before parsing.  
  //  5. Assembling doc-comment blocks (XML doc lines in c#) from individual comment lines 
  //     and attaching it to the next content token, and later sticking it to the parsed node.
  //  6. Handling newlines, indents and unindents for languages like Python. 
  //     Tracking this information directly in the scanner makes things really messy, and it does not fit well
  //     into general-purpose scanner. Token filter can handle it easily. In this case the scanner 
  //     handles the new-line character and indentations as whitespace and simply ignores it. 
  //     The CodeOutlineFilter re-creates new-line and indent tokens by analyzing 
  //     the line/column properties of the incoming tokens, and inserts them into its output. 
  #endregion
  public class TokenFilter {
    protected GrammarData GrammarData;
    protected Grammar Grammar; 

    public virtual IEnumerable<Token> BeginFiltering(CompilerContext context, IEnumerable<Token> tokens) {
      yield break;
    }
    public virtual void Init(GrammarData grammarData) {
      GrammarData = grammarData;
      Grammar = GrammarData.Grammar;
    }
    public virtual void OnSetSourceLocation(SourceLocation location) {
    }

  }//class

  public class TokenFilterList : List<TokenFilter> { }

}//namespace
