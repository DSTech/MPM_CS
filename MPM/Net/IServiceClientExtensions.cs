using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NServiceKit.Service;

namespace MPM.Net {

	public static class IServiceClientExtensions {

		public static Task<T> GetAsync<T>(this IServiceClient serviceClient, string url) {
			var tcs = new TaskCompletionSource<T>();
			serviceClient.GetAsync<T>(
				url,
				(res) => {
					tcs.SetResult(res);
				},
				(res, ex) => {
					tcs.SetException(ex);
				}
			);
			return tcs.Task;
		}

		public static Task<T> PostAsync<T>(this IServiceClient serviceClient, string url, object request) {
			var tcs = new TaskCompletionSource<T>();
			serviceClient.PostAsync<T>(
				url,
				request,
				(res) => {
					tcs.SetResult(res);
				},
				(res, ex) => {
					tcs.SetException(ex);
				}
			);
			return tcs.Task;
		}
	}
}
