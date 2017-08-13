using System;
using Google.OrTools.LinearSolver;
using MilpManager.Abstraction;

namespace OrToolsMilpManager.Implementation
{
	[Serializable]
	public class OrToolsVariable : IOrToolsVariable
	{
		[NonSerialized]
		private IMilpManager _milpManager;

		[NonSerialized]
		private Variable _variable;

		public IMilpManager MilpManager
		{
			get { return _milpManager; }
			set { _milpManager = value; }
		}

		public Domain Domain { get; set; }
		public string Name { get; set; }

		public Variable Variable
		{
			get { return _variable; }
			set { _variable = value; }
		}

		public double? ConstantValue { get; set; }
		public string Expression { get; set; }

		public override string ToString()
		{
			return $"[Name = {Name}, Domain = {Domain}, ConstantValue = {ConstantValue}, Variable = {Variable}";
		}
	}
}