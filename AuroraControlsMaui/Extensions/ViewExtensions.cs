using System;
namespace AuroraControls
{
    public static class ViewExtensions
    {
        public static Task<bool> TransitionToAsync<TElement>(this TElement element, string animationName, Action<double> callback, Func<double> start, double end, uint rate = 16, uint length = 250, Easing easing = null)
            where TElement : View
        {
            if (element == null)
                return Task.FromResult(false);

            if (easing == null)
                easing = Easing.Linear;

            var tcs = new TaskCompletionSource<bool>();

            var transitionAnimation = new Animation(callback, start?.Invoke() ?? default(double), end, easing);

            try
            {
                element.AbortAnimation(animationName);

                transitionAnimation.Commit(element, animationName, rate, length, finished: (f, a) => tcs.SetResult(a));
            }
            catch (InvalidOperationException)
            {

            }

            return tcs.Task;
        }

        public static void TransitionTo<TElement>(this TElement element, string animationName, Action<double> callback, Func<double> start, double end, uint rate = 16, uint length = 250, Easing easing = null)
            where TElement : View
        {
            TransitionTo(element, animationName, callback, start?.Invoke() ?? default(double), end, rate, length, easing);
        }

        public static void TransitionTo<TElement>(this TElement element, string animationName, Action<double> callback, double start, double end, uint rate = 16, uint length = 250, Easing easing = null)
            where TElement : View
        {
            if (element == null)
                return;

            if (easing == null)
                easing = Easing.Linear;

            var transitionAnimation = new Animation(callback, start, end, easing);

            try
            {
                element.AbortAnimation(animationName);

                transitionAnimation.Commit(element, animationName, rate, length);
            }
            catch (InvalidOperationException)
            {

            }
        }
    }
}

