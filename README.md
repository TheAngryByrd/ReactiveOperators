# ReactiveOperators

##Build status
Head (branch `master`) Build & Unit tests

Windows/.NET [![Build status](https://ci.appveyor.com/api/projects/status/1d75glc4hep7nqka?svg=true)](https://ci.appveyor.com/project/TheAngryByrd/reactiveoperators)

Mono 3.10 [![Build Status](https://travis-ci.org/TheAngryByrd/ReactiveOperators.svg?branch=master)](https://travis-ci.org/TheAngryByrd/ReactiveOperators)

Is a collection of useful operations outside of the Reactive Extensions standard toolset.

So far this binary has two operator sets:

- Boolean
- Generic

##Boolean Operators
This is set of fluent operations geared directly toward ```IObservable<bool>```. It has the standard binary operation you would expect:
- Not
- And
- Or
- Xor
- Nand
- Nor
- XNor

These operations except for the Not operator, use CombineLatest to combine event streams together.

```cs
Subject<bool> binaryStreamA = new Subject<bool>();
Subject<bool> binaryStreamB = new Subject<bool>();

var andedStreams = binaryStreamA.And(binaryStreamB);

andedStreams.Subscribe(x => Console.Writeline(x));

binaryStreamA.OnNext(false);
//No output yet as these are using combine latest.

binaryStreamB.OnNext(false);
//Output: false

binaryStreamA.OnNext(true);
//Output: false

binaryStreamA.OnNext(true);
//Output: true
```


It is particularly useful when used with [ReactiveUI](http://www.reactiveui.net)'s Commands. ReactiveCommand take a ```IObservable<bool>``` as a parameter in its constructor for its CanExecute. ([Read More](https://github.com/reactiveui/ReactiveUI/blob/docs/docs/basics/reactive-command.md#canexecute-via-observable))

```cs
var nameRequired = this.WhenAnyValue(x => x.Name)
        .Select(x => !String.IsNullOrWhitespace(x));
var ageConstraints = this.WhenAnyValue(x => x.Age)
        .Select(x => !String.IsNullOrWhitespace(x) && x >= 18);

var AllowAccess = new ReactiveCommand(nameRequired.And(ageConstraints));
```

It can also be useful when using it in conjunction with the IsExecuting property of a ReactiveCommand.

```cs
private IObservable<bool> AreAnyCommandsExecuting()
{
    return DownloadPhotoCommand.IsExecuting
        .Or(SetLockscreenCommand.IsExecuting)
        .Or(OnActivatedCommand.IsExecuting);
}


AreAnyCommandsExecuting().Subscribe(x => ProgressBarVisibility = x);


```

