﻿using System.Threading;
using System.Threading.Tasks;



namespace WindowsService.Host.Workers
{
	public interface IAsyncWorker<T>
	{
		Task<T> DoWork(CancellationToken token);
	}
}