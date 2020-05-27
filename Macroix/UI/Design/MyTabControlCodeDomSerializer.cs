using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Macroix.UI.Design
{
	using System.CodeDom;
	using System.Windows.Forms;
	using UI.Controls;
	internal class MyTabControlCodeDomSerializer : CodeDomSerializer
	{
		enum Phase
		{
			Declaration,
			Comment,
			Initialization,
			Collection
		}

		static CodeDomSerializer GetBaseSerializer(IDesignerSerializationManager manager)
		{
			return (CodeDomSerializer)manager.GetSerializer(typeof(MyTabControl).BaseType, typeof(CodeDomSerializer));
		}
		static string CodeToString(CodeObject @object)
		{
			switch (@object)
			{
				case CodeTypeReference ctr:
					return ctr.BaseType;
				case CodeComment cc:
					return cc.Text;
				default:
					return string.Format("Unknown code object type: {0}", @object.GetType());
			}
		}
		static string CodeToString(CodeExpression expression)
		{
			switch (expression)
			{
				case CodeVariableReferenceExpression cvre:
					return cvre.VariableName;
				case CodeObjectCreateExpression coce:
					return string.Format("new {0}(<...>)", CodeToString(coce.CreateType));
				case CodePropertyReferenceExpression cpre:
					return string.Format("{0}.{1}", CodeToString(cpre.TargetObject), cpre.PropertyName);
				case CodeTypeReferenceExpression ctre:
					return CodeToString(ctre.Type);
				case CodePrimitiveExpression cpe:
					return cpe.Value.ToString();
				case CodeMethodInvokeExpression cmie:
					return string.Format("{0}(<...>)", CodeToString(cmie.Method));
				case CodeMethodReferenceExpression cmre:
					return string.Format("{0}.{1}", CodeToString(cmre.TargetObject), cmre.MethodName);
				default:
					return string.Format("Unknown expression type: {0}", expression.GetType());
			}
		}
		static string CodeToString(CodeStatement statement)
		{
			switch (statement)
			{
				case CodeAssignStatement cas:
					return string.Format("{0} = {1}", CodeToString(cas.Left), CodeToString(cas.Right));
				case CodeVariableDeclarationStatement cvds:
					return string.Format("var {0} = {1};", cvds.Name, cvds.InitExpression);
				case CodeCommentStatement ccs:
					return string.Format("//{0}", CodeToString(ccs.Comment));
				case CodeExpressionStatement ces:
					return CodeToString(ces.Expression);
				default:
					return string.Format("Unknown statement type: {0}", statement.GetType());
			}
		}

		public override object Deserialize(IDesignerSerializationManager manager, object codeObject)
		{
			return GetBaseSerializer(manager).Deserialize(manager, codeObject);
		}

		static Phase GetPhase(CodeStatement statement)
		{
			switch (statement)
			{
				case CodeVariableDeclarationStatement _:
					return Phase.Declaration;
				case CodeCommentStatement _:
					return Phase.Comment;
				case CodeAssignStatement cas:
					switch (cas.Left)
					{
						case CodePropertyReferenceExpression _:
							return Phase.Initialization;
						case CodeFieldReferenceExpression _:
							return Phase.Initialization;
						case CodeVariableReferenceExpression _:
							return Phase.Declaration;
					}
					throw new ArgumentException(string.Format("Unknown CodeAssignStatement.Left type: {0}", cas.Left.GetType()));
				case CodeExpressionStatement ces:
					switch (ces.Expression)
					{
						case CodeMethodInvokeExpression cmie:
							switch (cmie.Method.TargetObject)
							{
								case CodePropertyReferenceExpression cpre:
									if (cpre.PropertyName == "TabPages" && (cmie.Method.MethodName == "Clear" || cmie.Method.MethodName == "Add"))
										return Phase.Collection;
									return Phase.Initialization;
							}
							return Phase.Initialization;
					}
					return Phase.Initialization;
			}
			//throw new ArgumentException(string.Format("Unknown CodeStatement type: {0}", statement.GetType()));
			return Phase.Initialization;
		}
		public override object Serialize(IDesignerSerializationManager manager, object value)
		{
			CodeDomSerializer serializer = GetBaseSerializer(manager);
			object codeObject = serializer.Serialize(manager, value);
			CodeStatementCollection codeStatementCollection = codeObject as CodeStatementCollection;
			CodeStatementCollection finalStatements;
			if (codeStatementCollection != null)
			{
				CodeStatementCollection declarationStatements = new CodeStatementCollection();
				CodeStatementCollection commentStatements = new CodeStatementCollection();
				CodeStatementCollection initializationStatements = new CodeStatementCollection();
				CodeStatementCollection collectionStatements = new CodeStatementCollection();
				Dictionary<Phase, CodeStatementCollection> mapping = new Dictionary<Phase, CodeStatementCollection>();
				mapping.Add(Phase.Declaration, declarationStatements);
				mapping.Add(Phase.Comment, commentStatements);
				mapping.Add(Phase.Initialization, initializationStatements);
				mapping.Add(Phase.Collection, collectionStatements);

				foreach (CodeStatement statement in codeStatementCollection)
				{
					mapping[GetPhase(statement)].Add(statement);
				}

				finalStatements = new CodeStatementCollection();
				finalStatements.AddRange(declarationStatements);
				finalStatements.AddRange(commentStatements);
				finalStatements.AddRange(collectionStatements);
				finalStatements.AddRange(initializationStatements);
			}
			else
				finalStatements = codeStatementCollection;
			return finalStatements;
		}
	}
}
