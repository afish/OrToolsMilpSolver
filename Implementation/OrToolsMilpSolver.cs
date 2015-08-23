using MilpManager.Abstraction;

namespace OrToolsMilpManager.Implementation
{
    public class OrToolsMilpSolver : BaseMilpSolver
    {
        public override IVariable SumVariables(IVariable first, IVariable second, Domain domain)
        {
            throw new System.NotImplementedException();
        }

        public override IVariable NegateVariable(IVariable variable, Domain domain)
        {
            throw new System.NotImplementedException();
        }

        public override IVariable MultiplyVariableByConstant(IVariable variable, IVariable constant, Domain domain)
        {
            throw new System.NotImplementedException();
        }

        public override IVariable DivideVariableByConstant(IVariable variable, IVariable constant, Domain domain)
        {
            throw new System.NotImplementedException();
        }

        public override void SetLessOrEqual(IVariable variable, IVariable bound)
        {
            throw new System.NotImplementedException();
        }

        public override IVariable FromConstant(int value, Domain domain)
        {
            throw new System.NotImplementedException();
        }

        public override IVariable FromConstant(double value, Domain domain)
        {
            throw new System.NotImplementedException();
        }

        public override IVariable Create(string name, Domain domain)
        {
            throw new System.NotImplementedException();
        }

        public override IVariable CreateAnonymous(Domain domain)
        {
            throw new System.NotImplementedException();
        }

        public override void AddGoal(string name, IVariable operation)
        {
            throw new System.NotImplementedException();
        }

        public override string GetGoalExpression(string name)
        {
            throw new System.NotImplementedException();
        }

        public override void SaveModelToFile(string modelPath)
        {
            throw new System.NotImplementedException();
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
            throw new System.NotImplementedException();
        }

        public override double GetValue(IVariable variable)
        {
            throw new System.NotImplementedException();
        }

        public override SolutionStatus GetStatus()
        {
            throw new System.NotImplementedException();
        }
    }
}