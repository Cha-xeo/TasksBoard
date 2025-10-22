using TaskBoard.Api.Models;

namespace TaskBoard.Api.Services.Helper
{
	public static class ColletionHelpers
	{
        /// <summary>
        /// Sync an existing collection from an incoming collection. It is assumed that each T has a unique key (like ID).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="existingItems"></param>
        /// <param name="incomingItems"></param>
        /// <param name="keySelector"></param>
        public static void SyncCollection<T>(ICollection<T> existingItems, List<T>? incomingItems, Func<T, object> keySelector)
			where T : class
		{
			if (incomingItems is null) return; // no change

			if (incomingItems.Count == 0) // clear all
			{
				existingItems.Clear();
				return;
			}

			// Add/Remove

			// Convert for easier comparison
			HashSet<Object>? existingKeys = existingItems.Select(keySelector).ToHashSet();
			HashSet<Object>? incomingKeys = incomingItems.Select(keySelector).ToHashSet();


            if (existingKeys.SetEquals(incomingKeys))
                return; // nothing changed

            // Remove
            List<T> toRemove = existingItems
				.Where(e => !incomingKeys.Contains(keySelector(e)))
				.ToList();
			if (toRemove.Any())
			{
				foreach (T? item in toRemove) 
				{
					existingItems.Remove(item);
				}
			}


			// Add
			List<T> toAdd = incomingItems
                .Where(i => !existingKeys.Contains(keySelector(i)))
				.ToList();
            if (toAdd.Any())
			{
				foreach (T? item in toAdd) 
				{
					existingItems.Add(item);
				}
			}

			return;
		}
	}
}