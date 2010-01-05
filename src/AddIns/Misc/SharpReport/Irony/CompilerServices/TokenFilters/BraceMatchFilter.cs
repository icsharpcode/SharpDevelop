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

  #region Description
  // This filter controls matching of left/right symbol pairs in the source.
  // "Braces" are not necessarily curly braces "{' - it can be [], (), <> or any 
  // begin/end block symbol or keyword pair defined in particular language. 
  // Why do we need this filter?
  // Some languages allow different pairs of symbols to be used interchangabely. 
  // For example, Scheme allows using either () or [] symbol pairs. Ruby allows either do...end or {} 
  // as block enclosing symbols. However in both languages the Matching Rule still applies - 
  // an opening symbol must be matched by the closing symbol from the pair, not by a symbol from another pair.
  // If we try to express this in grammar, we'll have to write every production 
  // involving such symbols several times, once for each pair. This can be a real hassle. 
  // The alternative is to declare two nonTerminals, one for all opening symbols and one 
  // for all closing ones, and then use them in productions. 
  // In this case the resulting grammar (and parser) ignores the matching rule, 
  // so we must provide the match checking outside the parser - and that's what this filter is doing. 
  // The other use is to match open/closing braces for editor support (VS integration)
  #endregion


  public class BraceMatchFilter : TokenFilter {
    private Stack<Token> _braces = new Stack<Token>();

    public override IEnumerable<Token> BeginFiltering(CompilerContext context, IEnumerable<Token> tokens) {
      foreach (Token token in tokens) {
        if (!token.Terminal.IsSet(TermOptions.IsBrace)) {
          yield return token;
          continue;
        }
        //open brace symbol
        if (token.Terminal.IsSet(TermOptions.IsOpenBrace)) {
          _braces.Push(token);
          yield return token;
          continue;
        }
        //We have closing brace
        if (_braces.Count == 0) {
          yield return context.CreateErrorTokenAndReportError( token.Location, token.Text, "Unmatched closing brace '{0}'", token.Text);
          continue;
        }
        //check match
        Token last = _braces.Pop();
        if (last.AsSymbol.IsPairFor != token.AsSymbol) {
          yield return context.CreateErrorTokenAndReportError(token.Location, token.Text,
              "Unmatched closing brace '{0}' - expected '{1}'", last.AsSymbol.IsPairFor.Name);
          continue;
        }
        //everything is ok, there's matching brace on top of the stack
        Token.LinkMatchingBraces(last, token);
        context.CurrentParseTree.OpenBraces.Add(last);
        yield return token; //return this token
      }//foreach token
      yield break;
    }//method

    public override void OnSetSourceLocation(SourceLocation location) {
      while (_braces.Count > 0 && _braces.Peek().Location.Position >= location.Position)
        _braces.Pop(); 
    }
  }//class
}
