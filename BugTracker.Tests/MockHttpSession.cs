using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BugTracker.Tests
{
	internal class MockHttpSession : ISession
	{
		Dictionary<string, object> sessionStorage = new Dictionary<string, object>();

		public object this[string name]
		{
			get { return sessionStorage[name]; }
			set { sessionStorage[name] = value; }
		}

		public bool IsAvailable => throw new NotImplementedException();

		public string Id => throw new NotImplementedException();

		public IEnumerable<string> Keys => throw new NotImplementedException();

		public void Clear()
		{
			sessionStorage.Clear();
		}

		public Task CommitAsync(CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public Task LoadAsync(CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public void Remove(string key)
		{
			sessionStorage.Remove(key);
		}

		public void Set(string key, byte[] value)
		{
			sessionStorage[key] = value;
		}

		public bool TryGetValue(string key, out byte[] value)
		{
			if(sessionStorage[key] != null)
			{
				value = (byte[])sessionStorage[key];
				return true;
			}
			else
			{
				value = null;
				return false;
			}
		}
	}
}
