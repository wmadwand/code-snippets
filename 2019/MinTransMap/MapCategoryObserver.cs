namespace MinTrans
{
	public interface IObserver
	{
		void UpdateState(Category category);
	}

	public interface IObservable
	{
		void Subscribe(IObserver observer, string category);
		void Unsubscribe(IObserver observer, string category);
		void Notify(Category category);
	}

	public struct Category
	{
		public string name;
		public bool isActive;

		public Category(string category, bool isActive)
		{
			this.name = category;
			this.isActive = isActive;
		}
	}

	//public enum MapObjectCategory
	//{
	//	City,
	//	Tc
	//}	
}