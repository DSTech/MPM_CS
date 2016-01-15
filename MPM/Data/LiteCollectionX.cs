using System;
using LiteDB;

namespace MPM.Data {
    public static class LiteCollectionX {
        /// <summary>
        /// Upserts the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection within which to upsert the <paramref name="value"/>.</param>
        /// <param name="value">The value to upsert.</param>
        public static void Upsert<T>(this LiteCollection<T> collection, T value) where T : new() {
            if (!collection.Update(value)) {
                collection.Insert(value);
            }
        }
        public static void Upsert<T>(this LiteCollection<T> collection, T value, Func<T, BsonValue> keySelector) where T : new() {
            if (!collection.Update(keySelector(value), value)) {
                collection.Insert(value);
            }
        }
    }
}
