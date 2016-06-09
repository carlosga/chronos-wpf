using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Chronos.Presentation.ViewModel
{
    /// <summary>
    /// ObservableCollection transform extension methods
    /// </summary>
    public static class CollectionTransforms
    {
        #region · Methods ·

        /// <summary>
        /// Transforms the give collection to a <see cref="CollectionViewModel<TSource, TTarget>"/> collection
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="sourceCollection"></param>
        /// <param name="setup"></param>
        /// <param name="coerce"></param>
        /// <param name="teardown"></param>
        /// <returns></returns>
        public static CollectionViewModel<TSource, TTarget> Transform<TSource, TTarget>(
            this ObservableCollection<TSource> sourceCollection,
            Func<TSource, TTarget> setup,
            Func<TTarget, TSource> coerce,
            Action<TTarget> teardown = null) where TSource : INotifyPropertyChanged where TTarget : INotifyPropertyChanged
        {
            return new CollectionViewModel<TSource, TTarget>(sourceCollection, setup, coerce, teardown);
        }

        #endregion
    }
}
