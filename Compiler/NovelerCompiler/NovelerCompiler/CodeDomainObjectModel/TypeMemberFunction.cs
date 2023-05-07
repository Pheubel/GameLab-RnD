﻿using Noveler.Compiler.CodeDomainObjectModel.Statements;

namespace Noveler.Compiler.CodeDomainObjectModel
{
	/// <summary>
	/// 
	/// </summary>
	/// <param name="Name"></param>
	/// <param name="ReturnType"></param>
	internal sealed record TypeMemberFunction(string Name, TypeReference ReturnType) : TypeMember(Name)
	{
		public TypeMemberFunction(string Name, string ReturnTypeName) : this(Name, new TypeReference(ReturnTypeName)) { }

		public TypeParameterCollection GenericTypeParameters { get; } = new TypeParameterCollection();

		public ParameterDeclarationExpressionCollection Parameters { get; } = new ParameterDeclarationExpressionCollection();

		public StatementCollection Statements { get; } = new StatementCollection();
	};
}
