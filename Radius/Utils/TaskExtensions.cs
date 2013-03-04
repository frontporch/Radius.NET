using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FP.Radius
{
	public static class TaskExtensions
	{
		public static Task<T> WithTimeout<T>(this Task<T> task, TimeSpan timeout, string remoteServer)
		{
			var timeoutTask = Task.Delay(timeout);
			var tcs = new TaskCompletionSource<T>();

			Task.WhenAny(task, timeoutTask).ContinueWith(_ =>
			{
				if (task.IsCompleted)
				{
					tcs.SetResult(task.Result);
					return;
				}
				if (task.IsCanceled)
				{
					tcs.SetCanceled();
					return;
				}
				if (task.IsFaulted)
				{
					tcs.SetException(task.Exception.Flatten());
					return;
				}

				//We have to do something with the original task if it faults and we have timed out
				task.ContinueWith((originalTask) => { var swallowMe = originalTask.Exception; }, TaskContinuationOptions.OnlyOnFaulted);

				tcs.SetException(new ConnectionException(remoteServer));
			});

			return tcs.Task;
		}
	}

	public class ConnectionException : Exception
	{
		public ConnectionException(string remoteServer, Exception innerException = null)
			: base(String.Format("Timeout connecting to {0}", remoteServer), innerException) { }
	}
}
