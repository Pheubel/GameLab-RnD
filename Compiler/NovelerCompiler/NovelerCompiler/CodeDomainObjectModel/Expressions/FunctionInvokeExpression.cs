namespace Noveler.Compiler.CodeDomainObjectModel.Expressions
{
	internal sealed record FunctionInvokeExpression(FunctionReferenceExpression FunctionReference, Expression[] Parameters) : Expression
	{
		public FunctionInvokeExpression(Expression TargetObject, string FunctionName, Expression[] Parameters) : this(new FunctionReferenceExpression(TargetObject, FunctionName), Parameters) { }
		public FunctionInvokeExpression(Expression TargetObject, string FunctionName, Expression[] Parameters, TypeReferenceCollection GenericTypeArguments) : this(new FunctionReferenceExpression(TargetObject, FunctionName, GenericTypeArguments), Parameters) { }
	};
}
