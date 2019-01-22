using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace RadyaLabs.Components.Mvc
{
    public static class ModelStateDictionaryExtensions
    {
        public static Dictionary<String, String> Errors(this ModelStateDictionary modelState)
        {
            return modelState
                .Where(state => state.Value.Errors.Count > 0)
                .ToDictionary(
                    pair => pair.Key,
                    pair => pair.Value.Errors
                        .Select(value => value.ErrorMessage)
                        .FirstOrDefault(error => !String.IsNullOrEmpty(error))
            );
        }

        public static void AddModelError<TModel>(this ModelStateDictionary modelState, Expression<Func<TModel, Object>> expression, String message)
        {
            modelState.AddModelError(GetExpressionText(expression), message);
        }
        public static void AddModelError<TModel>(this ModelStateDictionary modelState, Expression<Func<TModel, Object>> expression, String format, params Object[] args)
        {
            modelState.AddModelError(GetExpressionText(expression), String.Format(format, args));
        }

        private static String GetExpressionText(LambdaExpression expression)
        {
            UnaryExpression unary = expression.Body as UnaryExpression;
            if (unary?.Operand is MemberExpression)
                return ExpressionHelper.GetExpressionText(Expression.Lambda(unary.Operand, expression.Parameters[0]));

            return ExpressionHelper.GetExpressionText(expression);
        }
    }
}
