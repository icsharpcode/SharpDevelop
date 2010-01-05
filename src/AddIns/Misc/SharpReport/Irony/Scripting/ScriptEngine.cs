using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.CompilerServices;
using Irony.Scripting.Ast;
using Irony.Scripting.Runtime;

namespace Irony.Scripting {
  public class ScriptEngine {
    public readonly LanguageData Language;
    public readonly Compiler Compiler;

    #region constructors
    public ScriptEngine(Compiler compiler) {
      Compiler = compiler;
      Language = Compiler.Language;
    }
    public ScriptEngine(LanguageData language) {
      Language = language;
      Compiler = new Compiler(Language); 
    }
    public ScriptEngine(Grammar grammar) {
      Compiler = new Compiler(grammar);
      Language = Compiler.Language;
    }
    #endregion

/*
    public void AnalyzeCode(ParseTree parseTree, CompilerContext context) {
      var astRoot = parseTree.Root.AstNode as AstNode;
      if (astRoot == null) return;
      RunAnalysisPhases(astRoot, context,
           CodeAnalysisPhase.Init, CodeAnalysisPhase.AssignScopes, CodeAnalysisPhase.Allocate,
           CodeAnalysisPhase.Binding, CodeAnalysisPhase.MarkTailCalls, CodeAnalysisPhase.Optimization);
      //sort errors if there are any
      if (context.CurrentParseTree.Errors.Count > 0)
        context.CurrentParseTree.Errors.Sort(SyntaxErrorList.ByLocation);
    }

    private void RunAnalysisPhases(AstNode astRoot, CompilerContext context, params CodeAnalysisPhase[] phases) {
      CodeAnalysisArgs args = new CodeAnalysisArgs(context);
      foreach (CodeAnalysisPhase phase in phases) {
        switch (phase) {
          case CodeAnalysisPhase.AssignScopes:
            astRoot.Scope = new Scope(astRoot, null);
            break;

          case CodeAnalysisPhase.MarkTailCalls:
            if (!Language.Grammar.FlagIsSet(LanguageFlags.TailRecursive)) continue;//foreach loop - don't run the phase
            astRoot.Flags |= AstNodeFlags.IsTail;
            break;
        }//switch
        args.Phase = phase;
        astRoot.OnCodeAnalysis(args);
      }//foreach phase
    }//method
  
 */

  
  }//class
}
