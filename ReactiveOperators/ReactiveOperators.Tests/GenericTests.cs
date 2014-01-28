using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Reactive.Testing;
using NUnit.Framework;
using System.Reactive;
using ReactiveUI.Testing;

namespace ReactiveOperators.Tests
{
    [TestFixture]
    public class GenericTests
    {
        // Licensed under the MIT license with <3 by GitHub

        static readonly Func<int, TimeSpan> LinearMsStrategy = n => TimeSpan.FromMilliseconds(1 * n);

        [Test]
        public void DoesNotRetryInCaseOfSuccess()
        {
            (new TestScheduler()).With(sched =>
            {
                int tryCount = 0;

                var source = Observable.Defer(() =>
                {
                    tryCount++;
                    return Observable.Return("yolo");
                });

                source.RetryWithBackoffStrategy(
                    retryCount: 3,
                    strategy: LinearMsStrategy,
                    scheduler: sched
                    );

                source.Subscribe();
                
                Assert.AreEqual(1, tryCount);

                sched.AdvanceByMs(1);
                Assert.AreEqual(1, tryCount);
            });
        }

        [Test]
        public void PropagatesLastObservedExceptionIfAllTriesFail()
        {
            new TestScheduler().With(sched =>
            {
                int tryCount = 0;

                var source = Observable.Defer(() =>
                {
                    tryCount++;
                    return Observable.Throw<string>(new InvalidOperationException(tryCount.ToString()));
                });

                var observable = source.RetryWithBackoffStrategy(
                    retryCount: 3,
                    strategy: LinearMsStrategy,
                    scheduler: sched
                    );

                Exception lastError = null;
                observable.Subscribe(_ => { }, e => { lastError = e; });

                Assert.AreEqual(1, tryCount);

                sched.AdvanceByMs(1);
                Assert.AreEqual(2, tryCount);

                sched.AdvanceByMs(2);
                Assert.AreEqual(3, tryCount);

                Assert.Null(lastError);
                Assert.AreEqual("3", lastError.Message);
            });
        }

        [Test]
        public void RetriesOnceIfSuccessBeforeRetriesRunOut()
        {
            new TestScheduler().With(sched =>
            {
                int tryCount = 0;

                var source = Observable.Defer(() =>
                {
                    if (tryCount++ < 1) return Observable.Throw<string>(new InvalidOperationException());
                    return Observable.Return("yolo " + tryCount);
                });

                var observable = source.RetryWithBackoffStrategy(
                    retryCount: 5,
                    strategy: LinearMsStrategy,
                    scheduler: sched
                    );

                string lastValue = null;
                observable.Subscribe(n => { lastValue = n; });

                Assert.AreEqual(1, tryCount);
                Assert.Null(lastValue);

                sched.AdvanceByMs(1);
                Assert.AreEqual(2, tryCount);
                Assert.AreEqual("yolo 2", lastValue);
            });
        }

        [Test]
        public void UnsubscribingDisposesSource()
        {
            new TestScheduler().With(sched =>
            {
                int c = -1;

                var neverEndingSource = Observable.Defer(() =>
                {
                    return Observable.Timer(TimeSpan.FromMilliseconds(1), TimeSpan.FromMilliseconds(1), sched)
                        .Do(_ => c++)
                        .Select(_ => Unit.Default);
                });

                var observable = neverEndingSource.RetryWithBackoffStrategy(scheduler: sched);

                // Cold
                Assert.AreEqual(-1, c);

                var disp = observable
                    .Take(2)
                    .Subscribe();

                sched.AdvanceByMs(1);
                Assert.AreEqual(0, c);

                sched.AdvanceByMs(1);
                Assert.AreEqual(1, c);

                sched.AdvanceByMs(10);
                Assert.AreEqual(1, c);
            });
        }
    }
}
