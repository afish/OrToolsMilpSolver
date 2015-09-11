using System;
using System.IO;
using Google.OrTools.LinearSolver;
using MilpManager.Abstraction;
using Solver = Google.OrTools.LinearSolver.Solver;

namespace OrToolsMilpManager.Implementation
{
    public class OrToolsMilpSolver : BaseMilpSolver
    {
        private readonly Solver _solver;
        private int _varId;
        private int _solutionStatus;

        public OrToolsMilpSolver(int integerWidth, string solverName = "CBC_MIXED_INTEGER_PROGRAMMING") : base(integerWidth)
        {
            _solver = Solver.CreateSolver("OrTools", solverName);
        }

        public override IVariable SumVariables(IVariable first, IVariable second, Domain domain)
        {
            var firstCasted = first as OrToolsVariable;
            var secondCasted = second as OrToolsVariable;

            var variable = CreateAnonymous(domain) as OrToolsVariable;
            var constraint = _solver.MakeConstraint(0, 0);
            constraint.SetCoefficient(firstCasted.Variable, 1);
            constraint.SetCoefficient(secondCasted.Variable, 1);
            constraint.SetCoefficient(variable.Variable, -1);
            variable.ConstantValue = firstCasted.ConstantValue + secondCasted.ConstantValue;

            return variable;
        }

        public override IVariable NegateVariable(IVariable variable, Domain domain)
        {
            var firstCasted = variable as OrToolsVariable;

            var result = CreateAnonymous(domain) as OrToolsVariable;
            var constraint = _solver.MakeConstraint(0, 0);
            constraint.SetCoefficient(firstCasted.Variable, -1);
            constraint.SetCoefficient(result.Variable, -1);
            result.ConstantValue = -firstCasted.ConstantValue;

            return result;
        }

        public override IVariable MultiplyVariableByConstant(IVariable variable, IVariable constant, Domain domain)
        {
            var firstCasted = variable as OrToolsVariable;
            var secondCasted = constant as OrToolsVariable;

            var result = CreateAnonymous(domain) as OrToolsVariable;
            var constraint = _solver.MakeConstraint(0, 0);
            constraint.SetCoefficient(firstCasted.Variable, secondCasted.ConstantValue.Value);
            constraint.SetCoefficient(result.Variable, -1);
            result.ConstantValue = firstCasted.ConstantValue*secondCasted.ConstantValue;

            return result;
        }

        public override IVariable DivideVariableByConstant(IVariable variable, IVariable constant, Domain domain)
        {
            var firstCasted = variable as OrToolsVariable;
            var secondCasted = constant as OrToolsVariable;

            var result = CreateAnonymous(domain) as OrToolsVariable;
            var constraint = _solver.MakeConstraint(0, 0);
            constraint.SetCoefficient(firstCasted.Variable, 1/secondCasted.ConstantValue.Value);
            constraint.SetCoefficient(result.Variable, -1);
            result.ConstantValue = firstCasted.ConstantValue/secondCasted.ConstantValue;

            return result;
        }

        public override void SetLessOrEqual(IVariable variable, IVariable bound)
        {
            var firstCasted = variable as OrToolsVariable;
            var secondCasted = bound as OrToolsVariable;
            var constraint = _solver.MakeConstraint(double.NegativeInfinity, 0);
            constraint.SetCoefficient(firstCasted.Variable, 1);
            constraint.SetCoefficient(secondCasted.Variable, -1);
        }

        public override IVariable FromConstant(int value, Domain domain)
        {
            return FromConstant((double)value, domain);
        }

        public override IVariable FromConstant(double value, Domain domain)
        {
            var variable = _solver.MakeIntVar(value, value, $"x_{_varId++}");
            return new OrToolsVariable
            {
                Domain = domain,
                MilpManager = this,
                Variable = variable,
                ConstantValue = value
            };
        }

        public override IVariable Create(string name, Domain domain)
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
                    variable.Variable = _solver.MakeIntVar(-Int32.MaxValue, Int32.MaxValue, name);
                    break;
                case Domain.AnyReal:
                case Domain.AnyConstantReal:
                    variable.Variable = _solver.MakeIntVar(-Int32.MaxValue, Int32.MaxValue, name);
                    break;
                case Domain.PositiveOrZeroInteger:
                case Domain.PositiveOrZeroConstantInteger:
                    variable.Variable = _solver.MakeIntVar(0, Int32.MaxValue, name);
                    break;
                case Domain.PositiveOrZeroReal:
                case Domain.PositiveOrZeroConstantReal:
                    variable.Variable = _solver.MakeIntVar(0, Int32.MaxValue, name);
                    break;
                case Domain.BinaryInteger:
                case Domain.BinaryConstantInteger:
                    variable.Variable = _solver.MakeIntVar(0, 1, name);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(domain), domain, null);
            }
            
            return variable;
        }

        public override IVariable CreateAnonymous(Domain domain)
        {
            return Create($"var_{_varId++}", domain);
        }

        public override void AddGoal(string name, IVariable operation)
        {
            Objective objective = _solver.Objective();
            objective.SetMaximization();
            var cost = (operation as OrToolsVariable).Variable;
            objective.SetCoefficient(cost, 1);
        }

        public override string GetGoalExpression(string name)
        {
            throw new System.NotImplementedException();
        }

        public override void SaveModelToFile(string modelPath)
        {
            if (Path.GetExtension(modelPath) == "lp")
            {
                File.WriteAllText(modelPath, _solver.ExportModelAsLpFormat(false));
            }
            else
            {
                File.WriteAllText(modelPath, _solver.ExportModelAsMpsFormat(true, false));
            }
        }

        public override void LoadModelFromFile(string modelPath, string solverDataPath)
        {
            throw new System.NotImplementedException();
        }

        public override void SaveSolverDataToFile(string solverOutput)
        {
            throw new System.NotImplementedException();
        }

        public override IVariable GetByName(string name)
        {
            throw new System.NotImplementedException();
        }

        public override IVariable TryGetByName(string name)
        {
            throw new System.NotImplementedException();
        }

        public override void Solve()
        {
            _solutionStatus = _solver.Solve();
        }

        public override double GetValue(IVariable variable)
        {
            return (variable as OrToolsVariable).Variable.SolutionValue();
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