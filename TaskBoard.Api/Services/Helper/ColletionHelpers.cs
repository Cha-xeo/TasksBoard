namespace TaskBoard.Api.Services.Helper
{
	public static class ColletionHelpers
	{
		public static void SyncCollection<T>(ICollection<T> existingItems, List<T>? incomingItems)
			where T : class
		{
			if (incomingItems == null) return; // no change
			existingItems.Clear();
			foreach (var item in incomingItems)
			{
				existingItems.Add(item);
			}
		}
	}
}