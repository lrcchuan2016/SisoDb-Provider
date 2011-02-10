﻿using System.Collections.Generic;
using System.Linq.Expressions;
using SisoDb.Lambdas.Nodes;
using SisoDb.Resources;

namespace SisoDb.Lambdas
{
    internal class SortingParser : ISortingParser
    {
        public IParsedLambda Parse(IEnumerable<LambdaExpression> sortingExpressions)
        {
            sortingExpressions.AssertHasItems("sortingExpressions");

            var nodesContainer = new NodesContainer();
            foreach (var lambda in sortingExpressions)
            {
                var memberExpression = ExpressionTreeUtils.GetRightMostMember(lambda.Body);

                var sortDirection = SortDirections.Asc;
                var callExpression = (lambda.Body is UnaryExpression)
                                         ? ((UnaryExpression)lambda.Body).Operand as MethodCallExpression
                                         : lambda.Body as MethodCallExpression;

                if (callExpression != null)
                {
                    switch (callExpression.Method.Name)
                    {
                        case "Asc":
                            sortDirection = SortDirections.Asc;
                            break;
                        case "Desc":
                            sortDirection = SortDirections.Desc;
                            break;
                        default:
                            throw new SisoDbException(ExceptionMessages.SortingParser_UnsupportedMethodForSortingDirection);
                    }
                }

                var sortingNode = new SortingNode(memberExpression.Path(), sortDirection);
                nodesContainer.AddNode(sortingNode);
            }

            return new ParsedLambda(nodesContainer);
        }
    }
}