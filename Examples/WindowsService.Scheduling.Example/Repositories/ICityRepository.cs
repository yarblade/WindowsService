namespace WindowsService.Scheduling.Example.Repositories
{
	internal interface ICityRepository
	{
		int[] GetCityIds(int count);
	}
}