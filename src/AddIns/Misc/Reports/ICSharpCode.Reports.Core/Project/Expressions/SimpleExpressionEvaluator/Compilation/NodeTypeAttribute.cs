using System;

namespace SimpleExpressionEvaluator.Compilation
{
    [AttributeUsage(AttributeTargets.Class,AllowMultiple=false,Inherited=true)]
    public class NodeTypeAttribute : Attribute
    {
        public NodeTypeAttribute(ExpressionNodeType nodeType)
        {
            NodeType = nodeType;
        }

        public ExpressionNodeType NodeType { get; set; }

        public static NodeTypeAttribute Find(Type target)
        {
            var attributes = GetCustomAttributes(target, typeof (NodeTypeAttribute),true);
            if (attributes != null && attributes.Length > 0)
                return attributes[attributes.Length - 1] as NodeTypeAttribute;
            return null;
        }
    }
}