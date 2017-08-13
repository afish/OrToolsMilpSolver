using System;
using System.IO;
using System.Linq;
using Google.OrTools.LinearSolver;
using MilpManager.Abstraction;
using Solver = Google.OrTools.LinearSolver.Solver;

namespace OrToolsMilpManager.Implementation
{
	public class OrToolsMilpSolver : BaseMilpSolver, IModelSaver<MpsSaveFileSettings>, IModelSaver<LpSaveFileSettings>
	{
		public Solver Solver => Settings.Solver;

		private int _solutionStatus;

		public new readonly OrToolsMilpSolverSettings Settings;

		public OrToolsMilpSolver(OrToolsMilpSolverSettings settings) : base(settings)
		{
			Settings = settings;
		}

		protected override IVariable InternalSumVariables(IVariable first, IVariable second, Domain domain)
		{
			var firstCasted = first as IOrToolsVariable;
			var secondCasted = second as IOrToolsVariable;

			var variable = CreateAnonymous(domain) as IOrToolsVariable;
			var constraint = Solver.MakeConstraint(0, 0);
			constraint.SetCoefficient(firstCasted.Variable, 1);
			constraint.SetCoefficient(secondCasted.Variable, 1);
			constraint.SetCoefficient(variable.Variable, -1);

			return variable;
		}

		protected override IVariable InternalNegateVariable(IVariable variable, Domain domain)
		{
			var firstCasted = variable as IOrToolsVariable;

			var result = CreateAnonymous(domain) as IOrToolsVariable;
			var constraint = Solver.MakeConstraint(0, 0);
			constraint.SetCoefficient(firstCasted.Variable, -1);
			constraint.SetCoefficient(result.Variable, -1);

			return result;
		}

		protected override IVariable InternalMultiplyVariableByConstant(IVariable variable, IVariable constant, Domain domain)
		{
			var firstCasted = variable as IOrToolsVariable;

			var result = CreateAnonymous(domain) as IOrToolsVariable;
			var constraint = Solver.MakeConstraint(0, 0);
			constraint.SetCoefficient(firstCasted.Variable, constant.ConstantValue.Value);
			constraint.SetCoefficient(result.Variable, -1);

			return result;
		}

		protected override IVariable InternalDivideVariableByConstant(IVariable variable, IVariable constant,
			Domain domain)
		{
			var firstCasted = variable as IOrToolsVariable;

			var result = CreateAnonymous(domain) as IOrToolsVariable;
			var constraint = Solver.MakeConstraint(0, 0);
			constraint.SetCoefficient(firstCasted.Variable, 1/constant.ConstantValue.Value);
			constraint.SetCoefficient(result.Variable, -1);

			return result;
		}

		private void Set(IVariable variable, IVariable bound, double lowerBound, double upperBound)
		{
			var firstCasted = variable as IOrToolsVariable;
			var secondCasted = bound as IOrToolsVariable;
			var constraint = Solver.MakeConstraint(lowerBound, upperBound);
			constraint.SetCoefficient(firstCasted.Variable, 1);
			constraint.SetCoefficient(secondCasted.Variable, -1);
		}

		public override void SetLessOrEqual(IVariable variable, IVariable bound)
		{
			Set(variable, bound, double.NegativeInfinity, 0);
		}

		public override void SetGreaterOrEqual(IVariable variable, IVariable bound)
		{
			Set(variable, bound, 0, double.PositiveInfinity);
		}

		public override void SetEqual(IVariable variable, IVariable bound)
		{
			if (variable != bound)
			{
				Set(variable, bound, 0, 0);
			}
		}

		protected override IVariable InternalFromConstant(string name, int value, Domain domain)
		{
			return FromConstant((double)value, domain);
		}

		protected override IVariable InternalFromConstant(string name, double value, Domain domain)
		{
			var variable = Solver.MakeIntVar(value, value, name);
			return new OrToolsVariable
			{
				Domain = domain,
				MilpManager = this,
				Variable = variable,
				ConstantValue = value,
				Name = name
			};
		}

		protected override IVariable InternalCreate(string name, Domain domain)
		{
			var variable = new OrToolsVariable
			{
				Name = name,
				Domain = domain,
				MilpManager = this
			};
			switch (domain)
			{
				case Domain.AnyInteger:
				case Domain.AnyConstantInteger:
					variable.Variable = Solver.MakeIntVar(double.MinValue, double.MaxValue, name);
					break;
				case Domain.AnyReal:
				case Domain.AnyConstantReal:
					variable.Variable = Solver.MakeVar(double.MinValue, double.MaxValue, false, name);
					break;
				case Domain.PositiveOrZeroInteger:
				case Domain.PositiveOrZeroConstantInteger:
					variable.Variable = Solver.MakeIntVar(0, double.MaxValue, name);
					break;
				case Domain.PositiveOrZeroReal:
				case Domain.PositiveOrZeroConstantReal:
					variable.Variable = Solver.MakeVar(0, double.MaxValue, false, name);
					break;
				case Domain.BinaryInteger:
				case Domain.BinaryConstantInteger:
					variable.Variable = Solver.MakeIntVar(0, 1, name);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(domain), domain, null);
			}
			
			return variable;
		}

		protected override void InternalAddGoal(string name, IVariable operation)
		{
			Objective objective = Solver.Objective();
			objective.SetMaximization();
			var cost = (operation as IOrToolsVariable).Variable;
			objective.SetCoefficient(cost, 1);
		}

		public override void SaveModel(SaveFileSettings settings)
		{
			File.WriteAllText(settings.Path, Solver.ExportModelAsMpsFormat(true, false));
		}

		public void SaveModel(MpsSaveFileSettings settings)
		{
			File.WriteAllText(settings.Path, Solver.ExportModelAsMpsFormat(settings.Fixed, settings.Obfuscated));
		}

		public void SaveModel(LpSaveFileSettings settings)
		{
			File.WriteAllText(settings.Path, Solver.ExportModelAsLpFormat(settings.Obfuscated));
		}

		protected override object GetObjectsToSerialize()
		{
			return null;
		}

		protected override void InternalDeserialize(object data)
		{
			var variablesCopy = Variables.Values.OfType<IOrToolsVariable>().ToArray();
			foreach (var variable in variablesCopy)
			{
				var solverVariable = Solver.LookupVariableOrNull(variable.Name);
				if (solverVariable != null)
				{
					variable.Variable = solverVariable;
				}
				else
				{
					Variables.Remove(variable.Name);
				}
			}
			foreach (var goal in Goals.ToArray())
			{
				AddGoal(goal.Key, goal.Value);
			}
		}

		protected override void InternalLoadModelFromFile(string modelPath)
		{
			Solver.ReadModelFromFile(modelPath, true);
		}
		public override void Solve()
		{
			_solutionStatus = Solver.Solve();
		}

		public override double GetValue(IVariable variable)
		{
			return (variable as IOrToolsVariable).Variable.SolutionValue();
		}

		public override SolutionStatus GetStatus()
		{
			if (_solutionStatus == Solver.OPTIMAL) return SolutionStatus.Optimal;
			if (_solutionStatus == Solver.FEASIBLE) return SolutionStatus.Feasible;
			if (_solutionStatus == Solver.INFEASIBLE) return SolutionStatus.Infeasible;
			return SolutionStatus.Unknown;
		}
	}
}