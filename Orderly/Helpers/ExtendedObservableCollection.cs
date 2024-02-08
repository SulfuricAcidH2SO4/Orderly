using Orderly.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Orderly.Helpers
{
    public class ExtendedObservableCollection<T> : ObservableCollection<T>, IDisposable
    {
        private readonly object _lockCollection = new();

        public ExtendedObservableCollection() : base() { }
        public ExtendedObservableCollection(IEnumerable<T> collection) : base(collection) { }

        #region 'Override' Methods
        public new void Add(T item)
        {
            lock (_lockCollection) {
                Application.Current.Dispatcher.Invoke(delegate
                {
                    base.Add(item);
                });
            }
        }
        public new void Remove(T item)
        {
            lock (_lockCollection) {
                Application.Current.Dispatcher.Invoke(delegate
                {
                    _ = base.Remove(item);
                });
            }
        }
        public new void RemoveAt(int index)
        {
            lock (_lockCollection) {
                Application.Current.Dispatcher.Invoke(delegate
                {
                    base.RemoveAt(index);
                });
            }
        }

        public new void Clear()
        {
            lock (_lockCollection) {
                Application.Current.Dispatcher.Invoke(delegate
                {
                    base.Clear();
                });
            }
        }
        public new void Insert(int index, T item)
        {
            lock (_lockCollection) {
                Application.Current.Dispatcher.Invoke(delegate
                {
                    base.Insert(index, item);
                });
            }
        }
        #endregion

        public void AddRange(IEnumerable<T> collection)
        {
            if (collection == null) {
                return; // Quit 
            }

            lock (_lockCollection) {
                Application.Current.Dispatcher.Invoke(delegate
                {
                    collection.ForEach(x => base.Add(x));
                });
            }
        }

        public void RemoveAll(Func<T, bool> match = null)
        {
            lock (_lockCollection) {
                Application.Current.Dispatcher.Invoke(delegate
                {
                    if (match == null)
                        base.Clear();

                    foreach (var item in Items.Where(match).ToList())
                        base.Remove(item);
                });
            }
        }

        public void Dispose()
        {
            lock (_lockCollection) {
                Application.Current.Dispatcher.Invoke(delegate
                {
                    Clear();
                });
            }

            GC.SuppressFinalize(this);
        }
    }
}
