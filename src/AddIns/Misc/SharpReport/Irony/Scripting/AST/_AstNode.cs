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
using System.CodeDom;
using System.Xml;
using System.IO;
using Irony.CompilerServices;
using Irony.Scripting.Runtime;

namespace Irony.Scripting.Ast {

  #region NodeArgs
  // This class is a container for information used by the NodeCreator delegate and default node constructor
  // Using this struct simplifies signatures of custom AST nodes and it allows to easily add parameters in the future
  // if such need arises without breaking the existing code. 
  public class NodeArgs {
    public readonly CompilerContext Context;
    public readonly BnfTerm Term;
    public readonly SourceSpan Span;
    public readonly AstNodeList ChildNodes;

    public NodeArgs(CompilerContext context, BnfTerm term, SourceSpan span, AstNodeList childNodes) {
      Context = context;
      Term = term;
      Span = span; 
      ChildNodes = childNodes;
    }
  }//struct
  #endregion

  [Flags]
  public enum AstNodeFlags {
    None = 0x0,
    IsTail          = 0x01,     //the node is in tail position
    //Identifiers flags
    AllocateSlot    = 0x02,     //the identifier node should allocate slot for a variable
    SuppressNotDefined = 0x04,  //suppress "variable not defined" message during binding; this flag is set in Id node of function call - 
                                // to allow for external functions
    NotRValue        = 0x08,    // used in identifiers as indicators that this use is not RValue, so identifier does not mark the slot as 
    IsLValue         = 0x10,           // used in identifiers as indicators that this use is LValue assignment
    UsesOuterScope   = 0x020,   //the function uses values in outer(parent) scope(s), so it may need closure 

  }


  public class AstNodeList : List<AstNode> { }

  public class NodeAttributeDictionary : Dictionary<string, object> { }

  public interface IAstVisitor {
    void BeginVisit(AstNode node);
    void EndVisit(AstNode node);
  }
  public delegate void NodeEvaluate(EvaluationContext context);
  
  

  //Base AST node class
  public class AstNode : IAstNodeInit  {
    public AstNode() {
    }
    public AstNode(NodeArgs args) {
    }

    #region IAstNodeInit Members
    public void Init(CompilerContext context, ParseTreeNode treeNode) {
      this.Evaluate = DoEvaluate;
      this.Term = treeNode.Term;
      Span = treeNode.Span;
      foreach (ParseTreeNode childInfo in treeNode.ChildNodes)
        AddChild(null, childInfo.AstNode as AstNode);
    }

    public void SetParent(object parent) {
      Parent = (AstNode)parent;
    }
    #endregion

    #region properties Parent, Term, Span, Location, ChildNodes, Scope, Role, Flags, Attributes
    public AstNode Parent;
    public BnfTerm Term; 

    public SourceSpan Span; 

    public SourceLocation Location {
      get { return Span.Start; }
    }

    //List of child nodes
    public AstNodeList  ChildNodes = new AstNodeList();
    public void AddChild(string role, AstNode child) {
      if (child == null) return;
      child.Role = role;
      ChildNodes.Add(child);
    }

    //Most of the time, Scope is the scope that owns this node - the scope in which it is defined; this scope belongs to one of 
    // node parents. Only for AnonFunctionNode we have a scope that is defined by the node itself - the scope that contain function's local
    // variables
    public Scope Scope;
    
    // Role is a free-form string used as prefix in ToString() representation of the node. 
    // Node's parent can set it to "property name" or role of the child node in parent's node context. 
    public string Role;
    public AstNodeFlags Flags;

    public bool IsSet(AstNodeFlags flag) {
      return (Flags & flag) != 0;
    }

    //TODO: finish this
    public static string GetContent(AstNode node) {
      return string.Empty; 
    }

    public NodeAttributeDictionary Attributes {
      get {
        if (_attributes == null)
          _attributes = new NodeAttributeDictionary();
        return _attributes;
      }
    } NodeAttributeDictionary _attributes;

    #endregion

    
    public override string ToString() {
      string result = string.Empty; 
      if (!string.IsNullOrEmpty(Role))
        result = Role + ": ";
      if (Term != null)
        result += Term.Name;
      return result;

    }

    #region Visitors, Iterators
    //the first primitive Visitor facility
    public virtual void AcceptVisitor(IAstVisitor visitor) {
      visitor.BeginVisit(this);
      if (ChildNodes.Count > 0)
        foreach(AstNode node in ChildNodes)
          node.AcceptVisitor(visitor);
      visitor.EndVisit(this);
    }

    //Node traversal 
    public IEnumerable<AstNode> GetAll() {
      AstNodeList result = new AstNodeList();
      AddAll(result);
      return result; 
    }
    private void AddAll(AstNodeList list) {
      list.Add(this);
      foreach (AstNode child in this.ChildNodes)
        if (child != null) 
          child.AddAll(list);
    }
    #endregion

    #region CodeAnalysis
    public virtual void OnCodeAnalysis(CodeAnalysisArgs args) {
      switch (args.Phase) {
        case CodeAnalysisPhase.Init:
          foreach (AstNode child in ChildNodes) {
            child.Parent = this;
            child.Scope = null;
          }
          break;

        case CodeAnalysisPhase.AssignScopes:
          foreach (AstNode child in ChildNodes)
            if (child.Scope == null) //don't override if it already exists
              child.Scope = this.Scope;
          break;

        case CodeAnalysisPhase.MarkTailCalls:
          //if (!IsSet(AstNodeFlags.IsTail))       args.SkipChildren = true;
          break; 
      }//switch

      if (ChildNodes.Count > 0 && !args.SkipChildren)
        foreach (AstNode child in ChildNodes)
          child.OnCodeAnalysis(args);
      args.SkipChildren = false; 
    }//method
    #endregion

    #region evaluation: Evaluate, DoEvaluate
    // A reference to Evaluate implementation method called by Runtime during execution. By default it points to a virtual
    // method DoEvaluate that derived nodes can override. If there is some static logic to be executed during evaluation, 
    // the node implementation can optimize evaluation by defining several specific implementations (stripped from static logic) 
    // and setting this reference to the appropriate implementation during compile time. 
    public NodeEvaluate Evaluate;

    protected virtual void DoEvaluate(EvaluationContext context) {
      if (ChildNodes.Count >  0)
        foreach (AstNode child in ChildNodes)
          child.Evaluate(context);
    }
    #endregion

    #region ChildNodes manipulations
    public virtual bool IsEmpty() {
      return ChildNodes.Count == 0;
    }
    #endregion

    #region Xml Processing
    //TODO: Xml - this is just initial draft, needs more work.
    protected virtual XmlElement XmlAppendTo(XmlNode parentNode) {
      XmlElement thisElem = parentNode.OwnerDocument.CreateElement("Node");
      XmlSetAttributes(thisElem); 
      parentNode.AppendChild(thisElem);
      foreach (AstNode node in ChildNodes)
        node.XmlAppendTo(thisElem);
      return thisElem;
    }

    protected virtual void XmlSetAttributes(XmlElement element) {
      element.SetAttribute("Element", this.Term.Name);
      element.SetAttribute("NodeType", this.GetType().Name);
    }

    public XmlDocument XmlToDocument() {
      XmlDocument xdoc = new XmlDocument();
      XmlElement xRoot = xdoc.CreateElement("AST");
      xdoc.AppendChild(xRoot);
      this.XmlAppendTo(xRoot); 
      return xdoc;
    }
    public string XmlGetXmlString() {
      XmlDocument xdoc = XmlToDocument();
      StringWriter sw = new StringWriter();
      XmlTextWriter xw = new XmlTextWriter(sw);
      xw.Formatting = Formatting.Indented;
      xdoc.WriteTo(xw);
      xw.Flush();
      return sw.ToString(); 
    } 
    #endregion



  }//class

}//namespace
