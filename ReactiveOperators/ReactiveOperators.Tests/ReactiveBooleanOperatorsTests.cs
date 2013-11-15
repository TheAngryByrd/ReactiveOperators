using System;
using System.Linq;
using System.Reactive.Subjects;
using Microsoft.Reactive.Testing;
using NUnit.Framework;
using ReactiveOperators;

namespace ReactiveOperators.Tests
{
    [TestFixture]
    public class ReactiveBooleanOperatorsTests : ReactiveTest
    {
        [TestCase(false, Result = true)]
        [TestCase(true, Result = false)]
        public bool Not(bool value)
        {
            var subject = new Subject<bool>();
            var notSubject = subject.Not();
            bool? result = null;
            notSubject.Subscribe(c => result = c);

            subject.OnNext(value);

            return result.Value;
        }

        [TestCase(false, false, Result = false)]
        [TestCase(true, false, Result = false)]
        [TestCase(false, true, Result = false)]
        [TestCase(true, true, Result = true)]
        [TestCase(false, false,false, Result = false)]
        [TestCase(false, false, true, Result = false)]
        [TestCase(false, true, false, Result = false)]
        [TestCase(false, true, true, Result = false)]
        [TestCase(true, false, false, Result = false)]
        [TestCase(true, false, true, Result = false)]
        [TestCase(true, true, false, Result = false)]
        [TestCase(true, true, true, Result = true)]
        public bool And(params bool[] values)
        {
            return PerformTest(ReactiveBooleanOperators.And, values);
        }

        [TestCase(false, false, Result = true)]
        [TestCase(true, false, Result = true)]
        [TestCase(false, true, Result = true)]
        [TestCase(true, true, Result = false)]
        [TestCase(false, false, false, Result = true)]
        [TestCase(false, false, true, Result = true)]
        [TestCase(false, true, false, Result = true)]
        [TestCase(false, true, true, Result = true)]
        [TestCase(true, false, false, Result = true)]
        [TestCase(true, false, true, Result = true)]
        [TestCase(true, true, false, Result = true)]
        [TestCase(true, true, true, Result = false)]
        public bool Nand(params bool[] values)
        {
            return PerformTest(ReactiveBooleanOperators.Nand, values);
        }

        [TestCase(false, false, Result = false)]
        [TestCase(true, false, Result = true)]
        [TestCase(false, true, Result = true)]
        [TestCase(true, true, Result = true)]
        [TestCase(false, false, false, Result = false)]
        [TestCase(false, false, true, Result = true)]
        [TestCase(false, true, false, Result = true)]
        [TestCase(false, true, true, Result = true)]
        [TestCase(true, false, false, Result = true)]
        [TestCase(true, false, true, Result = true)]
        [TestCase(true, true, false, Result = true)]
        [TestCase(true, true, true, Result = true)]
        public bool Or(params bool[] values)
        {
            return PerformTest(ReactiveBooleanOperators.Or, values);
        }

        [TestCase(false,false, Result = true)]
        [TestCase(true, false, Result = false)]
        [TestCase(false, true, Result = false)]
        [TestCase(true, true, Result = false)]
        [TestCase(false, false, false, Result = true)]
        [TestCase(false, false, true, Result = false)]
        [TestCase(false, true, false, Result = false)]
        [TestCase(false, true, true, Result = false)]
        [TestCase(true, false, false, Result = false)]
        [TestCase(true, false, true, Result = false)]
        [TestCase(true, true, false, Result = false)]
        [TestCase(true, true, true, Result = false)]
        public bool Nor(params bool[] values)
        {
            return PerformTest(ReactiveBooleanOperators.Nor, values);
        }

        [TestCase(false, false, Result = false)]
        [TestCase(true, false, Result = true)]
        [TestCase(false, true, Result = true)]
        [TestCase(true, true, Result = false)]
        [TestCase(false, false, false, Result = false)]
        [TestCase(false, false, true, Result = true)]
        [TestCase(false, true, false, Result = true)]
        [TestCase(false, true, true, Result = false)]
        [TestCase(true, false, false, Result = true)]
        [TestCase(true, false, true, Result = false)]
        [TestCase(true, true, false, Result = false)]
        [TestCase(true, true, true, Result = true)]
        public bool Xor(params bool[] values)
        {
            return PerformTest(ReactiveBooleanOperators.Xor, values);
        }
        
        [TestCase(false, false, Result = true)]
        [TestCase(true, false, Result = false)]
        [TestCase(false, true, Result = false)]
        [TestCase(true, true, Result = true)]
        [TestCase(false, false, false, Result = true)]
        [TestCase(false, false, true, Result = false)]
        [TestCase(false, true, false, Result = false)]
        [TestCase(false, true, true, Result = true)]
        [TestCase(true, false, false, Result = false)]
        [TestCase(true, false, true, Result = true)]
        [TestCase(true, true, false, Result = true)]
        [TestCase(true, true, true, Result = false)]
        public bool Xnor(params bool[] values)
        {
            return PerformTest(ReactiveBooleanOperators.Xnor, values);
        }

        private delegate IObservable<bool> LogicOperator(IObservable<bool> operand, params IObservable<bool>[] operands);

        private bool PerformTest(LogicOperator logicOperator, params bool[] values)
        {
            var subjects = values.Select(_ => new Subject<bool>()).ToArray();
       
            var combined = logicOperator(subjects.First(), subjects.Skip(1).ToArray());
            bool? result = null;
            combined.Subscribe(c =>
            {
                result = c;                
            });

            foreach (var keyValues in values.Zip(subjects, (value,subject) => new {value,subject}))
            {
               keyValues.subject.OnNext(keyValues.value);
            }
            return result.Value;
        }

        [TestCase(false,false,false, Result=false)]
        [TestCase(false,false,true, Result=true)]
        [TestCase(false,true,false, Result=false)]
        [TestCase(false,true,true, Result=true)]
        [TestCase(true, false, false, Result = false)]
        [TestCase(true, false, true, Result = true)]
        [TestCase(true, true, false, Result = true)]
        [TestCase(true, true, true, Result = true)]
        public bool XAndYOrZ(bool x, bool y, bool z)
        {
            bool? result = null;

            var xObservable = new Subject<bool>();
            var yObservable = new Subject<bool>();
            var zObservable = new Subject<bool>();

            var combined = xObservable.And(yObservable).Or(zObservable);
            combined.Subscribe(c => result = c);

            zObservable.OnNext(z);
            xObservable.OnNext(x);            
            yObservable.OnNext(y);        

            return result.Value;
        }
    }
}
