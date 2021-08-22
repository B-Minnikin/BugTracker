using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Extension_Methods
{
	public interface IActivityMethods
	{
		public bool HasProperty(Activity activity, string propertyName);
		public T GetDerivedProperty<T>(Activity thisActivity, string targetProperty);
	}

	public class ActivityMethods : IActivityMethods
	{
		public bool HasProperty(Activity activity, string propertyName)
		{
			if (string.IsNullOrEmpty(propertyName))
			{
				return false;
			}

			var type = activity.GetType();
			return type.GetProperty(propertyName) != null;
		}

		public T GetDerivedProperty<T>(Activity thisActivity, string targetProperty)
		{
			var derivedType = thisActivity.GetType();
			var propertyName = derivedType.GetProperty(targetProperty);
			T result = (T)propertyName.GetValue(thisActivity);

			return result;
		}
	}

	public static class ActivityExtensions
	{
		private static IActivityMethods defaultImplementation = new ActivityMethods();
		public static IActivityMethods Implementation { private get; set; } = defaultImplementation;

		public static void RevertToDefaultImplementation()
		{
			Implementation = defaultImplementation;
		}

		public static bool HasProperty(this Activity activity, string propertyName) {
			return Implementation.HasProperty(activity, propertyName);
		}

		public static T GetDerivedProperty<T>(this Activity activity, string targetProperty) {
			return Implementation.GetDerivedProperty<T>(activity, targetProperty);
		}
	}
}
