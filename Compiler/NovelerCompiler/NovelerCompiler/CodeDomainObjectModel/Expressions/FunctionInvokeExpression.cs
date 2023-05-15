namespace Noveler.Compiler.CodeDomainObjectModel.Expressions
{
	internal sealed record FunctionInvokeExpression(FunctionReferenceExpression FunctionReference, ExpressionCollection Parameters) : Expression
	{
		public FunctionInvokeExpression(Expression TargetObject, string FunctionName, ExpressionCollection Parameters) : this(new FunctionReferenceExpression(TargetObject, FunctionName), Parameters) { }
		public FunctionInvokeExpression(Expression TargetObject, string FunctionName, ExpressionCollection Parameters, TypeReferenceCollection GenericTypeArguments) : this(new FunctionReferenceExpression(TargetObject, FunctionName, GenericTypeArguments), Parameters) { }

		public override IReadOnlyList<DomainObject> GetChildren()
		{
			var children = new List<DomainObject>
			{
				FunctionReference,
				Parameters
			};

			return children;
		}
	}
}
