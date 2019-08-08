using System.Linq.Expressions;

namespace HotChocolate.Types.Filters.Expressions
{
    public sealed class ComparableEqualsOperationHandler
        : ComparableOperationHandlerBase
    {
        protected override bool TryCreateExpression(
            FilterOperation operation,
            MemberExpression property,
            object parsedValue,
            out Expression expression)
        {
            switch (operation.Kind)
            {
                case FilterOperationKind.Equals:
                    expression = FilterExpressionBuilder.Equals(
                        property, parsedValue);
                    return true;

                case FilterOperationKind.NotEquals:
                    expression = FilterExpressionBuilder.Not(
                        FilterExpressionBuilder.Equals(
                            property, parsedValue)
                     );
                    return true;

                default:
                    expression = null;
                    return false;
            }
        }
    }
}