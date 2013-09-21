using System;
using System.Reactive.Subjects;
using Microsoft.Reactive.Testing;
using NUnit.Framework;

namespace ReactiveOperators.Tests
{
    [TestFixture]
    public class ReactiveBooleanOperators : ReactiveTest
    {

        [TestCase(false, Result = true)]
        [TestCase(true, Result = false)]
        public bool Not(bool x)
        {
            var one = new Subject<bool>();
            var not = one.Not();
            bool result = false;
            not.Subscribe(c => result = c);

            one.OnNext(x);

            return result;
        }

        [TestCase(false, false, Result = false)]
        [TestCase(true, false, Result = false)]
        [TestCase(false, true, Result = false)]
        [TestCase(true, true, Result = true)]
        public bool And(bool x, bool y)
        {
            return PerformTest(x, y, (a, b) => a.And(b));
        }
        [TestCase(false, false, Result = false)]
        [TestCase(true, false, Result = true)]
        [TestCase(false, true, Result = true)]
        [TestCase(true, true, Result = true)]
        public bool Or(bool x, bool y)
        {
            return PerformTest(x, y, (a, b) => a.Or(b));
        }

        [TestCase(false,false, Result = true)]
        [TestCase(true,false, Result = true)]
        [TestCase(false,true, Result = true)]
        [TestCase(true,true, Result = false)]
        public bool Nand(bool x, bool y)
        {
            return PerformTest(x, y, (a, b) => a.Nand(b));
        }        
        [TestCase(false,false, Result = true)]
        [TestCase(true, false, Result = false)]
        [TestCase(false, true, Result = false)]
        [TestCase(true, true, Result = false)]
        public bool Nor(bool x, bool y)
        {
            return PerformTest(x, y, (a, b) => a.Nor(b));
        }

        [TestCase(false, false, Result = false)]
        [TestCase(true, false, Result = true)]
        [TestCase(false, true, Result = true)]
        [TestCase(true, true, Result = false)]
        public bool Xor(bool x, bool y)
        {
            return PerformTest(x, y, (a, b) => a.Xor(b));
        }  
        
        [TestCase(false, false, Result = true)]
        [TestCase(true, false, Result = false)]
        [TestCase(false, true, Result = false)]
        [TestCase(true, true, Result = true)]
        public bool Xnor(bool x, bool y)
        {
            return PerformTest(x, y, (a,b) => a.Xnor(b));
        }  


        private bool PerformTest(bool x, bool y, Func<IObservable<bool>,IObservable<bool>,IObservable<bool>> logicOperator)
        {
            var one = new Subject<bool>();  
            var two = new Subject<bool>();            
            var combined = logicOperator(one, two);
            var result = false;
            combined.Subscribe(c => result = c);

            one.OnNext(x);
            two.OnNext(y);

            return result;
        }
        




    }
}
