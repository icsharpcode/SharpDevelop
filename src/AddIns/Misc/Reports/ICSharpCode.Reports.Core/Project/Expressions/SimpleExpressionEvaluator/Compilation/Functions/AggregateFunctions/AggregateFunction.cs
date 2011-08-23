using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.Interfaces;
using SimpleExpressionEvaluator.Evaluation;

namespace SimpleExpressionEvaluator.Compilation.Functions.AggregateFunctions
{
    [NodeType(ExpressionNodeType.Function)]
    
    public abstract class AggregateFunction<T> : FunctionBase<T>
    {
        protected AggregateFunction()
        {
        }

        protected AggregateFunction(ExpressionNodeType nodeType) : base(nodeType)
        {
        }
        
        public override T Evaluate(IExpressionContext context)
        {
        	ISinglePage singlePage = context.ContextObject as ISinglePage;
        	if (singlePage != null) {
        		return EvaluateSinglePage (context);
        	} else {
        		return EvaluateFromContext (context);
        	}
        }
        
        
        private  T EvaluateSinglePage(IExpressionContext context)
        {
        	var state = new AggregationState();
        	IDataNavigator navigator = NavigatorFromContext(context);
        	if (navigator != null) {
        		
        		if (Arguments.Count > 0)
        		{
        			object data = Arguments[0] != null ? Arguments[0].Evaluate(context) : null;      			
        			state.DataSet = SetupDataSource (data,navigator);        	        		
        		}

        		if (state.DataSet == null)
        		{
        			state.DataSet = SetupDataSource (null,navigator);
        		}

        		if (Arguments.Count > 1)
        		{
        			state.ValueExpression = Arguments[1];
        		}

        		var args = new object[0];
        		if (Arguments.Count > 2)
        		{
        			args = new object[Arguments.Count - 2];
        			for (int i = 2; i < Arguments.Count; i++)
        				args[i - 2] = Arguments[i].Evaluate(context);
        		}
        		return EvaluateFunction(context,state,args);
        	}
        	if (context.ContextObject is IEnumerable)
        		state.DataSet = context.ContextObject as IEnumerable;
        	else
        		return default(T);
        	return EvaluateFunction(context,state,null);
        }
        
        
        private T EvaluateFromContext(IExpressionContext context)
        {
        	var state = new AggregationState();

            if (Arguments.Count > 0)
            {
                object data = Arguments[0] != null ? Arguments[0].Evaluate(context) : null;
                state.DataSet = data as IEnumerable;
            }

            if (state.DataSet == null)
            {
                if (context.ContextObject is IEnumerable)
                    state.DataSet = context.ContextObject as IEnumerable;
                else
                    return default(T);
            }

            if (Arguments.Count > 1)
            {
                state.ValueExpression = Arguments[1];
            }

            var args = new object[0];
            if (Arguments.Count > 2)
            {
                args = new object[Arguments.Count - 2];
                for (int i = 2; i < Arguments.Count; i++)
                    args[i - 2] = Arguments[i].Evaluate(context);
            }
			
            return EvaluateFunction(context,state,args);
        }
        
        
        private List<object> SetupDataSource (object data,IDataNavigator navigator)
        {
            navigator.Reset();
        	List<object> list = new List<object>();
        	while ( navigator.MoveNext())
            {
        		CurrentItemsCollection row = navigator.GetDataRow;
        		CurrentItem currentItem = ExtractItemFromDataSet (row,data);
        		
        		if (currentItem != null) {
        			
        			object s1 = Convert.ToString(currentItem.Value.ToString(),CultureInfo.CurrentCulture);
        			if (IsNumeric(s1)) {
        				list.Add(Convert.ToDouble(s1,System.Globalization.CultureInfo.CurrentCulture));
        			} else {
        				list.Add(true);
        			}
        		} 
        		else {
        			string str = String.Format ("<{0}> not found in AggregateFunction.SetupDataSource",data.ToString());
        			throw new FieldNotFoundException(str);
        		}
        	}
        	return list;
        }
        
        
        
        private static CurrentItem  ExtractItemFromDataSet (CurrentItemsCollection row,object data)
        {
        	CurrentItem currentItem = null;
        	if (data != null)
        	{
        		currentItem = row.Find(data.ToString());
        	} else {
        		currentItem = row.Find(row[0].ColumnName);
        	}
        	return currentItem;
        }
        
        
        
        private IDataNavigator NavigatorFromContext (IExpressionContext context)
        {
        	ISinglePage p = context.ContextObject as ISinglePage;
        	return p.IDataNavigator;
        }
        
        
        private static bool IsNumeric(object Expression)
        {
        	bool isNum;
        	double retNum;
        	isNum = Double.TryParse(Convert.ToString(Expression,CultureInfo.CurrentCulture),
        	                        System.Globalization.NumberStyles.Any,
        	                        System.Globalization.NumberFormatInfo.InvariantInfo,
        	                        out retNum);
        	
        	return isNum;
        }
        
       
        
        protected virtual T EvaluateFunction(IExpressionContext context,AggregationState state,params object[] args)
        {

            state.CanReturn = false;
            foreach (object dataItem in state.DataSet)
            {
                var childContext = new ChildExpressionContext(context, dataItem);
                object childVal = state.ValueExpression != null ? state.ValueExpression.Evaluate(childContext) : dataItem;

                state.DataItem = dataItem;

                AggregateValue(childVal, state, args);
                
                //allow function to short circuit if it already
                //knows the answer.
                if (state.CanReturn)
                    break;

                state.CurrentIndex++;
            }

            return ExtractAggregateValue(state);
        }

        protected abstract void AggregateValue(object value, AggregationState aggregationState, params object[] args);

        protected abstract T ExtractAggregateValue(AggregationState aggregationState);
    }

    public class AggregationState : Dictionary<string,object>
    {
        public AggregationState() : base(StringComparer.InvariantCultureIgnoreCase)
        {
            CurrentIndex = 0;
        }

        public object DataItem { get; set; }
        public IEnumerable DataSet { get; set; }
        public IExpression ValueExpression { get; set; }
        public bool CanReturn { get; set; }
        public int CurrentIndex { get; set; }

    }
}
