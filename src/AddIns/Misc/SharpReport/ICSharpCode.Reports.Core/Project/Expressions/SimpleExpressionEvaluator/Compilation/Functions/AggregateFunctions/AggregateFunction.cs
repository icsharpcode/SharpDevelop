using ICSharpCode.Reports.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using SimpleExpressionEvaluator.Evaluation;
using System.Globalization;

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
        	
        	if (context.ContextObject is SinglePage) {
        		return EvaluateSinglePage (context);
        	} else {
        	
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
        
        
        private List<object> SetupDataSource (object data,IDataNavigator navigator)
        {
        	navigator.Reset();
        	List<object> list = new List<object>();
        	while ( navigator.MoveNext()) {
        		CurrentItemsCollection row = navigator.GetDataRow();
				CurrentItem ci = null;	
        		if (data != null) {
        			ci = row.Find(data.ToString());
        		} else {
					ci = row.Find(row[0].ColumnName);
        		}
        		
        		// s1 = Convert.ToString(row.Find(data.ToString()).Value.ToString(),CultureInfo.CurrentCulture);
        		if (ci != null) {
        			
        			object s1 = Convert.ToString(ci.Value.ToString(),CultureInfo.CurrentCulture);
        			
        			if (IsNumeric(s1)) {
        				list.Add(Convert.ToDouble(s1,System.Globalization.CultureInfo.CurrentCulture));
        			} else {
        				list.Add(true);
        			}
        		} else {
        			string str = String.Format ("<{0}> not found in AggregateFunction",data.ToString());
        			throw new FieldNotFoundException(str);
        		}
        		
        		//        		s1 = Convert.ToString(ci.Value.ToString(),CultureInfo.CurrentCulture);
//
//
        		//        		if (IsNumeric(s1)) {
        		//        			list.Add(Convert.ToDouble(s1,System.Globalization.CultureInfo.CurrentCulture));
        		//        		} else {
        		//        			list.Add(true);
        		//        		}
        		
        	}
        	return list;
        }
        
        /*
        private List<object> SetupDataSource (object data,IDataNavigator navigator)
        {
        	navigator.Reset();
        	List<object> list = new List<object>();
        	while ( navigator.MoveNext()) {
        		DataRow row = navigator.Current as DataRow;
				//CurrentItemsCollection row = navigator.GetDataRow();


        		object s1 = null;
        		if (data == null) {
        			s1 = Convert.ToString(row.ItemArray[0],System.Globalization.CultureInfo.InvariantCulture);
        		} else {
        			s1 = Convert.ToString(row[data.ToString()].ToString(),CultureInfo.CurrentCulture);
        		}
        		
        		if (IsNumeric(s1)) {
        			list.Add(Convert.ToDouble(s1,System.Globalization.CultureInfo.CurrentCulture));
        		} else {
        			list.Add(true);
        		}
        	}
        	return list;
        }
        */
        
        private IDataNavigator NavigatorFromContext (IExpressionContext context)
        {
        	SinglePage p = context.ContextObject as SinglePage;
        	if (p != null) {
        		return p.IDataNavigator;
        	}
        	return null;
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
