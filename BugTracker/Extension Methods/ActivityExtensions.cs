using BugTracker.Models;

namespace BugTracker.Extension_Methods;

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
		var result = (T)propertyName?.GetValue(thisActivity);

		return result;
	}
}

public static class ActivityExtensions
{
	private static IActivityMethods _defaultImplementation = new ActivityMethods();
	public static IActivityMethods Implementation { private get; set; } = _defaultImplementation;

	public static void RevertToDefaultImplementation()
	{
		Implementation = _defaultImplementation;
	}

	public static bool HasProperty(this Activity activity, string propertyName) {
		return Implementation.HasProperty(activity, propertyName);
	}

	public static T GetDerivedProperty<T>(this Activity activity, string targetProperty) {
		return Implementation.GetDerivedProperty<T>(activity, targetProperty);
	}
}
