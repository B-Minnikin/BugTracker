using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Extension_Methods
{
	public static class ActivityExtensions
	{
		public static bool HasProperty(this Activity activity, string propertyName)
		{
			if (string.IsNullOrEmpty(propertyName))
			{
				return false;
			}

			var type = activity.GetType();
			return type.GetProperty(propertyName) != null;
		}

		public static T GetDerivedProperty<T>(this Activity thisActivity, string targetProperty)
		{
			var derivedType = thisActivity.GetType();
			var propertyName = derivedType.GetProperty(targetProperty);
			T result = (T)propertyName.GetValue(thisActivity);

			return result;
		}
	}
}
