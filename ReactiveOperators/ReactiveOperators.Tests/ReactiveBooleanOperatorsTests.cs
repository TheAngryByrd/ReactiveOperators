using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using Microsoft.Reactive.Testing;
using NUnit.Framework;

namespace ReactiveOperators.Tests
{
    [TestFixture]
    public class ReactiveBooleanOperatorsTests : ReactiveTest
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
        public bool And(params bool[] values)
        {
            return PerformTest((a, b) => a.And(b), values);
        }

        [TestCase(false, false, Result = false)]
        [TestCase(true, false, Result = true)]
        [TestCase(false, true, Result = true)]
        [TestCase(true, true, Result = true)]
        public bool Or(params bool[] values)
        {
            return PerformTest((a, b) => a.Or(b), values);
        }

        [TestCase(false,false, Result = true)]
        [TestCase(true,false, Result = true)]
        [TestCase(false,true, Result = true)]
        [TestCase(true,true, Result = false)]
        public bool Nand(params bool[] values)
        {
            return PerformTest((a, b) => a.Nand(b), values);
        }

        [TestCase(false,false, Result = true)]
        [TestCase(true, false, Result = false)]
        [TestCase(false, true, Result = false)]
        [TestCase(true, true, Result = false)]
        public bool Nor(params bool[] values)
        {
            return PerformTest((a, b) => a.Nor(b), values);
        }

        [TestCase(false, false, Result = false)]
        [TestCase(true, false, Result = true)]
        [TestCase(false, true, Result = true)]
        [TestCase(true, true, Result = false)]
        public bool Xor(params bool[] values)
        {
            return PerformTest((a, b) => a.Xor(b), values);
        }
        
        [TestCase(false, false, Result = true)]
        [TestCase(true, false, Result = false)]
        [TestCase(false, true, Result = false)]
        [TestCase(true, true, Result = true)]
        public bool Xnor(params bool[] values)
        {
            return PerformTest((a,b) => a.Xnor(b), values);
        }

        private delegate IObservable<bool> LogicOperator(IObservable<bool> operand, params IObservable<bool>[] operands);

        private bool PerformTest(LogicOperator logicOperator, params bool[] values)
        {
            var subjects = values.Select(_ => new Subject<bool>()).ToArray();
       
            var combined = logicOperator(subjects.First(), subjects.Skip(1).ToArray());
            var result = false;
            combined.Subscribe(c =>
            {
                result = c;                
            });

            foreach (var keyValues in values.Zip(subjects, (value,subject) => new {value,subject}))
            {
                keyValues.subject.OnNext(keyValues.value);
            }
            return result;
        }
        




    }
}
