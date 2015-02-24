using System.Linq;



namespace WindowsService.Example.Repositories
{
	internal class CityRepository : ICityRepository
	{
		private int _lastCityId;

		public CityRepository()
		{
			_lastCityId = 1;
		}

		public int[] GetCityIds(int count)
		{
			var ids = Enumerable.Range(_lastCityId, count).ToArray();
			
			_lastCityId += count;

			return ids;
		}
	}
}
