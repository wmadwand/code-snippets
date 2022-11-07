using System;

namespace ARSystem
{
	public interface IARController
	{
		bool IsInitiated { get; }
		bool IsActive { get; }

		void Run(Action callback = null);
		void Stop(Action callback = null);
	}
}