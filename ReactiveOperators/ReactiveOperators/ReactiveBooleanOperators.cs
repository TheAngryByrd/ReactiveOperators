using System;
using System.Linq;
using System.Reactive.Linq;

namespace ReactiveOperators
{
    public static class ReactiveBooleanOperators
    {
        public static IObservable<bool> Not(this IObservable<bool> operand)
        {
            return operand.Select(x => !x);
        }

        public static IObservable<bool> And(this IObservable<bool> operand, params IObservable<bool>[] operands)
        {
            return operands.Aggregate(operand, (current, observable) => current.CombineLatest(observable, (x,y) => x && y));
        }
        
        public static IObservable<bool> Nand(this IObservable<bool> operand, params IObservable<bool>[] operands)
        {
            return operands.Aggregate(operand, (current, observable) => current.CombineLatest(observable, (x,y) => (x && y))).Not();
        }

        public static IObservable<bool> Or(this IObservable<bool> operand, params IObservable<bool>[] operands)
        {
            return operands.Aggregate(operand, (current, observable) => current.CombineLatest(observable, (x,y) => x || y));
        }

        public static IObservable<bool> Nor(this IObservable<bool> operand, params IObservable<bool>[] operands)
        {
            return operands.Aggregate(operand, (current, observable) => current.CombineLatest(observable, (x,y) => x || y)).Not();
        }

        public static IObservable<bool> Xor(this IObservable<bool> operand, params IObservable<bool>[] operands)
        {
            return operands.Aggregate(operand, (current, observable) => current.CombineLatest(observable, (x, y) => x ^ y));
        }
        
        public static IObservable<bool> Xnor(this IObservable<bool> operand, params IObservable<bool>[] operands)
        {
            return operands.Aggregate(operand, (current, observable) => current.CombineLatest(observable, (x, y) => x ^ y)).Not();
        }      



        
    }
}
