using MilpManager.Abstraction;

namespace OrToolsMilpManager.Implementation
{
	public class LpSaveFileSettings : SaveFileSettings
	{
		public LpSaveFileSettings()
		{
			Obfuscated = false;
		}

		public bool Obfuscated { get; set; }
	}

	public class MpsSaveFileSettings : SaveFileSettings
	{
		public MpsSaveFileSettings()
		{
			Obfuscated = false;
			Fixed = false;
		}

		public bool Obfuscated { get; set; }
		public bool Fixed { get; set; }
	}
}