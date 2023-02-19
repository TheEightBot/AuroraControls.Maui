using System;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Maui.Animations;
using Microsoft.Maui.Controls.Compatibility;
using Animation = Microsoft.Maui.Controls.Animation;

namespace AuroraControls;

/// <summary>
/// Visual element extensions.
/// </summary>
public static class VisualElementExtensions
{
    /// <summary>
    /// Extends VisualElement with a new ColorTo method which provides a higher level approach for animating an elements color.
    /// </summary>
    /// <returns>A task containing the animation result boolean.</returns>
    /// <param name="element">VisualElement to process.</param>
    /// <param name="start">Expression.</param>
    /// <param name="end">end color.</param>
    /// <param name="rate">The time, in milliseconds, between frames.</param>
    /// <param name="length">The number of milliseconds over which to interpolate the animation.</param>
    /// <param name="easing">The easing function to use to transision in, out, or in and out of the animation.</param>
    /// <typeparam name="TElement">The 1st type parameter.</typeparam>
    public static Task<bool> ColorTo<TElement>(this TElement element, Expression<Func<TElement, Color>> start, Color end, uint rate = 16, uint length = 250, Easing easing = null)
        where TElement : IAnimatable
    {
        if (element is null)
        {
            return Task.FromResult(false);
        }

        if (easing is null)
        {
            easing = Easing.Linear;
        }

        var member = (MemberExpression)start.Body;
        var property = member.Member as PropertyInfo;

        var animationName = $"color_to_{property.Name}_{element.GetHashCode()}";

        var tcs = new TaskCompletionSource<bool>();

        var elementStartingColor = (Color)property.GetValue(element);

        var transitionAnimation = new Animation(d => property.SetValue(element, elementStartingColor.Lerp(end, d)), 0d, 1d, easing);

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

    /// <summary>
    /// Extends VisualElement with a new SizeTo method which provides a higher level approach for animating transitions.
    /// </summary>
    /// <returns>A task containing the animation result boolean.</returns>
    /// <param name="element">The VisualElement to perform animation on.</param>
    /// <param name="start">The animation starting point.</param>
    /// <param name="end">The animation ending point.</param>
    /// <param name="rate">The time, in milliseconds, between frames.</param>
    /// <param name="length">The number of milliseconds over which to interpolate the animation.</param>
    /// <param name="easing">The easing function to use to transision in, out, or in and out of the animation.</param>
    /// <typeparam name="TElement">The 1st type parameter.</typeparam>
    public static Task<bool> TransitionTo<TElement>(this TElement element, Expression<Func<TElement, double>> start, double end, uint rate = 16, uint length = 250, Easing easing = null)
        where TElement : IAnimatable
    {
        if (element is null)
        {
            return Task.FromResult(false);
        }

        if (easing is null)
        {
            easing = Easing.Linear;
        }

        var member = (MemberExpression)start.Body;
        var property = member.Member as PropertyInfo;

        var animationName = $"transition_to_{property.Name}_{element.GetHashCode()}";

        var tcs = new TaskCompletionSource<bool>();

        var elementStartingPosition = (double)property.GetValue(element);

        var transitionAnimation = new Animation(d => property.SetValue(element, d), elementStartingPosition, end, easing);

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

    /// <summary>
    /// Extends VisualElement with a new SizeTo method which provides a higher level approach for animating transitions.
    /// </summary>
    /// <returns>A task containing the animation result boolean.</returns>
    /// <param name="element">The VisualElement to perform animation on.</param>
    /// <param name="start">The animation starting point.</param>
    /// <param name="end">The animation ending point.</param>
    /// <param name="rate">The time, in milliseconds, between frames.</param>
    /// <param name="length">The number of milliseconds over which to interpolate the animation.</param>
    /// <param name="easing">The easing function to use to transision in, out, or in and out of the animation.</param>
    /// <typeparam name="TElement">The 1st type parameter.</typeparam>
    public static Task<bool> TransitionTo<TElement>(this TElement element, Expression<Func<TElement, int>> start, int end, uint rate = 16, uint length = 250, Easing easing = null)
        where TElement : IAnimatable
    {
        if (element is null)
        {
            return Task.FromResult(false);
        }

        if (easing is null)
        {
            easing = Easing.Linear;
        }

        var member = (MemberExpression)start.Body;
        var property = member.Member as PropertyInfo;

        var animationName = $"transition_to_{property.Name}_{element.GetHashCode()}";

        var tcs = new TaskCompletionSource<bool>();

        var elementStartingPosition = (int)property.GetValue(element);

        var transitionAnimation = new Animation(d => property.SetValue(element, (int)d), elementStartingPosition, end, easing);

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

    public static Task<bool> TransitionTo<TElement>(this TElement element, Expression<Func<TElement, uint>> start, uint end, uint rate = 16, uint length = 250, Easing easing = null)
        where TElement : IAnimatable
    {
        if (element is null)
        {
            return Task.FromResult(false);
        }

        if (easing is null)
        {
            easing = Easing.Linear;
        }

        var member = (MemberExpression)start.Body;
        var property = member.Member as PropertyInfo;

        var animationName = $"transition_to_{property.Name}_{element.GetHashCode()}";

        var tcs = new TaskCompletionSource<bool>();

        var elementStartingPosition = (uint)property.GetValue(element);

        var transitionAnimation = new Animation(d => property.SetValue(element, (uint)d), elementStartingPosition, end, easing);

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

    public static Task<bool> VisualTransitionTo<TElement>(this TElement element, string animationName, Action<double> callback, Func<double> start, double end, uint rate = 16, uint length = 250, Easing easing = null)
        where TElement : IAnimatable
    {
        if (element is null)
        {
            return Task.FromResult(false);
        }

        if (easing is null)
        {
            easing = Easing.Linear;
        }

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

    public static Task<bool> TransitionTo<TElement>(this TElement element, string animationName, Action<double> callback, double start, double end, uint rate = 16, uint length = 250, Easing easing = null)
        where TElement : IAnimatable
    {
        if (element is null)
        {
            return Task.FromResult(false);
        }

        if (easing is null)
        {
            easing = Easing.Linear;
        }

        var tcs = new TaskCompletionSource<bool>();

        var transitionAnimation = new Animation(callback, start, end, easing);

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

    public static Task<bool> TransitionTo<TElement>(this TElement element, string animationName, Action<int> callback, Func<int> start, int end, uint rate = 16, uint length = 250, Easing easing = null)
        where TElement : IAnimatable
    {
        if (element is null)
        {
            return Task.FromResult(false);
        }

        if (easing is null)
        {
            easing = Easing.Linear;
        }

        var tcs = new TaskCompletionSource<bool>();

        var transitionAnimation = new Animation(x => callback((int)x), start?.Invoke() ?? default(int), end, easing);

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

    /// <summary>
    /// Extends VisualElement with a new PointTo method which provides a higher level method for animating Points.
    /// </summary>
    /// <returns>A task containing the animation result boolean.</returns>
    /// <param name="element">The VisualElement to perform animation on.</param>
    /// <param name="start">Start in point.</param>
    /// <param name="end">Ending point.</param>
    /// <param name="rate">The time, in milliseconds, between frames.</param>
    /// <param name="length">The number of milliseconds over which to interpolate the animation.</param>
    /// <param name="easing">The easing function to use to transision in, out, or in and out of the animation.</param>
    /// <typeparam name="TElement">The 1st type parameter.</typeparam>
    public static Task<bool> PointTo<TElement>(this TElement element, Expression<Func<TElement, Point>> start, Point end, uint rate = 16, uint length = 250, Easing easing = null)
        where TElement : IAnimatable
    {
        if (element is null)
        {
            return Task.FromResult(false);
        }

        if (easing is null)
        {
            easing = Easing.Linear;
        }

        var member = (MemberExpression)start.Body;
        var property = member.Member as PropertyInfo;

        var animationName = $"point_to_{property.Name}_{element.GetHashCode()}";

        var tcs = new TaskCompletionSource<bool>();

        var elementStartingPoint = (Point)property.GetValue(element);

        var transitionAnimation = new Animation(
            d =>
            {
                var newX = elementStartingPoint.X.Lerp(end.X, d);
                var newY = elementStartingPoint.Y.Lerp(end.Y, d);
                var newPoint = new Point(newX, newY);
                property.SetValue(element, newPoint);
            }, 0d, 1d, easing);

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

    /// <summary>
    /// Extends VisualElement with a new ThicknessTo method which provides a higher level approach for animating an elements Thickness.
    /// </summary>
    /// <returns>A task containing the animation result boolean.</returns>
    /// <param name="element">The VisualElement to perform animation on.</param>
    /// <param name="start">Starting thickness.</param>
    /// <param name="end">Ending thickness.</param>
    /// <param name="rate">The time, in milliseconds, between frames.</param>
    /// <param name="length">The number of milliseconds over which to interpolate the animation.</param>
    /// <param name="easing">The easing function to use to transision in, out, or in and out of the animation.</param>
    /// <typeparam name="TElement">The 1st type parameter.</typeparam>
    public static Task<bool> ThicknessTo<TElement>(this TElement element, Expression<Func<TElement, Thickness>> start, Thickness end, uint rate = 16, uint length = 250, Easing easing = null)
        where TElement : IAnimatable
    {
        if (element is null)
        {
            return Task.FromResult(false);
        }

        if (easing is null)
        {
            easing = Easing.Linear;
        }

        var member = (MemberExpression)start.Body;
        var property = member.Member as PropertyInfo;

        var animationName = $"point_to_{property.Name}_{element.GetHashCode()}";

        var tcs = new TaskCompletionSource<bool>();

        var elementStartingThickness = (Thickness)property.GetValue(element);

        var transitionAnimation = new Animation(
            d =>
            {
                var newLeft = elementStartingThickness.Left.Lerp(end.Left, d);
                var newTop = elementStartingThickness.Top.Lerp(end.Top, d);
                var newRight = elementStartingThickness.Right.Lerp(end.Right, d);
                var newBottom = elementStartingThickness.Bottom.Lerp(end.Bottom, d);
                var newThickness = new Thickness(newLeft, newTop, newRight, newBottom);
                property.SetValue(element, newThickness);
            }, 0d, 1d, easing);

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

    /// <summary>
    /// Extends VisualElement with a new SizeTo method which provides a higher level approach for animating an elements Size.
    /// </summary>
    /// <returns>A task containing the animation result boolean.</returns>
    /// <param name="element">The VisualElement to perform animation on.</param>
    /// <param name="start">The start size.</param>
    /// <param name="end">The end size.</param>
    /// <param name="rate">The time, in milliseconds, between frames.</param>
    /// <param name="length">The number of milliseconds over which to interpolate the animation.</param>
    /// <param name="easing">The easing function to use to transision in, out, or in and out of the animation.</param>
    /// <typeparam name="TElement">The 1st type parameter.</typeparam>
    public static Task<bool> SizeTo<TElement>(this TElement element, Expression<Func<TElement, Size>> start, Size end, uint rate = 16, uint length = 250, Easing easing = null)
        where TElement : IAnimatable
    {
        if (element is null)
        {
            return Task.FromResult(false);
        }

        if (easing is null)
        {
            easing = Easing.Linear;
        }

        var member = (MemberExpression)start.Body;
        var property = member.Member as PropertyInfo;

        var animationName = $"point_to_{property.Name}_{element.GetHashCode()}";

        var tcs = new TaskCompletionSource<bool>();

        var elementStartingSize = (Size)property.GetValue(element);

        var transitionAnimation = new Animation(
            d =>
            {
                var newWidth = elementStartingSize.Width.Lerp(end.Width, d);
                var newHeight = elementStartingSize.Height.Lerp(end.Height, d);
                var newSize = new Size(newWidth, newHeight);
                property.SetValue(element, newSize);
            }, 0d, 1d, easing);

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

    public static Page ParentPage(this VisualElement element)
    {
        if (element is null || element.Parent is null)
        {
            return null;
        }

        var hasParent = false;

        var parentElement = element.Parent;

        while (hasParent == false)
        {
            if (parentElement.Parent is null)
            {
                break;
            }

            hasParent = parentElement is Page;

            if (!hasParent)
            {
                parentElement = parentElement.Parent;
            }
        }

        return hasParent && parentElement is not null ? parentElement as Page : null;
    }

    public static VisualElement FindViewByAutomationId(this VisualElement element, string automationId)
    {
        if (element is null)
        {
            return null;
        }

        if (automationId.Equals(element.AutomationId))
        {
            return element;
        }

        if (element is ContentPage page)
        {
            return FindViewByAutomationId(page.Content, automationId);
        }
        else if (element is Layout<View> layout)
        {
            foreach (var child in layout.Children)
            {
                var foundChild = child.FindViewByAutomationId(automationId);

                if (foundChild is not null)
                {
                    return foundChild;
                }
            }
        }

        return null;
    }

    public static Thickness Scale(this Thickness thickness, double scale)
    {
        return new Thickness(
            thickness.Left * scale, thickness.Top * scale,
            thickness.Right * scale, thickness.Bottom * scale);
    }
}