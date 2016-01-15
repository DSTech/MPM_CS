using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MPM.CLI;

namespace MPM.Extensions {

    public static class StackX {

        public static bool RemoveFirst<T>(this Stack<T> stack, Predicate<T> qualifier) {
            if (stack == null) {
                throw new ArgumentNullException(nameof(stack));
            }
            if (qualifier == null) {
                throw new ArgumentNullException(nameof(qualifier));
            }
            var tempStorage = new Stack<T>(stack.Count);
            var found = false;
            while (stack.Count > 0) {
                var item = stack.Pop();
                if (qualifier.Invoke(item)) {
                    found = true;
                    break;
                }
                tempStorage.Push(item);
            }
            foreach (var value in tempStorage) {
                stack.Push(value);
            }
            return found;
        }

        public static int RemoveAll<T>(this Stack<T> stack, Predicate<T> qualifier) {
            if (stack == null) {
                throw new ArgumentNullException(nameof(stack));
            }
            if (qualifier == null) {
                throw new ArgumentNullException(nameof(qualifier));
            }
            var tempStorage = new Stack<T>(stack.Count);
            var removed = 0;
            while (stack.Count > 0) {
                var item = stack.Pop();
                if (!qualifier.Invoke(item)) {
                    tempStorage.Push(item);
                } else {
                    ++removed;
                }
            }
            foreach (var value in tempStorage) {
                stack.Push(value);
            }
            return removed;
        }
    }
}
