using System;
using System.Linq;
using System.Reactive.Linq;

namespace ReactiveOperators
{
    public static class ReactiveBooleanOperators
    {
        /// <summary>
        /// Projects a boolean obeservable into its opposite form.
        /// </summary>
        /// <param name="operand"></param>
        /// <returns></returns>
        public static IObservable<bool> Not(this IObservable<bool> operand)
        {
            return operand.Select(x => !x);
        }

        /// <summary>
        /// Combines the latest value from boolean observables with the And operator
        /// </summary>
        /// <param name="operand"></param>
        /// <param name="operands"></param>
        /// <returns></returns>
        public static IObservable<bool> And(this IObservable<bool> operand, params IObservable<bool>[] operands)
        {
            return PerformOperation(And, operand, operands);
        }

        /// <summary>
        /// Combines the latest value from boolean observables with the Nand operator
        /// </summary>
        /// <param name="operand"></param>
        /// <param name="operands"></param>
        /// <returns></returns>
        public static IObservable<bool> Nand(this IObservable<bool> operand, params IObservable<bool>[] operands)
        {
            return AntiPerformOperation(And, operand, operands);
        }

        private static bool And(bool x, bool y)
        {
            return x && y;
        }

        /// <summary>
        /// Combines the latest value from boolean observables with the Or operator
        /// </summary>
        /// <param name="operand"></param>
        /// <param name="operands"></param>
        /// <returns></returns>
        public static IObservable<bool> Or(this IObservable<bool> operand, params IObservable<bool>[] operands)
        {
            return PerformOperation(Or, operand, operands);
        }

        /// <summary>
        /// Combines the latest value from boolean observables with the Nor operator
        /// </summary>
        /// <param name="operand"></param>
        /// <param name="operands"></param>
        /// <returns></returns>
        public static IObservable<bool> Nor(this IObservable<bool> operand, params IObservable<bool>[] operands)
        {
            return AntiPerformOperation(Or, operand, operands);
        }

        private static bool Or(bool x, bool y)
        {
            return x || y;
        }

        /// <summary>
        /// Combines the latest value from boolean observables with the Xor operator
        /// </summary>
        /// <param name="operand"></param>
        /// <param name="operands"></param>
        /// <returns></returns>
        public static IObservable<bool> Xor(this IObservable<bool> operand, params IObservable<bool>[] operands)
        {
            return PerformOperation(Xor, operand, operands);
        }

        /// <summary>
        /// Combines the latest value from boolean observables with the Xnor operator
        /// </summary>
        /// <param name="operand"></param>
        /// <param name="operands"></param>
        /// <returns></returns>
        public static IObservable<bool> Xnor(this IObservable<bool> operand, params IObservable<bool>[] operands)
        {
            return AntiPerformOperation(Xor, operand, operands);
        }

        private static bool Xor(bool x, bool y)
        {
            return x ^ y;
        }

        private static IObservable<bool> PerformOperation(Func<bool, bool, bool> @operator, IObservable<bool> operand, params IObservable<bool>[] operands)
        {
            return operands.Aggregate(operand, (current, observable) => current.CombineLatest(observable, @operator));
        }

        private static IObservable<bool> AntiPerformOperation(Func<bool, bool, bool> @operator, IObservable<bool> operand, params IObservable<bool>[] operands)
        {
            return PerformOperation(@operator, operand, operands).Not();


        }
    }
}
