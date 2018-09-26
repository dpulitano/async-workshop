module AsyncReactive.Main

open System
open Fable.Core
open Fable.Import
open Fable.Import.Browser
open System.Collections.Generic

// ==================================================================
// This file just calls demos in other files. Go through the 
// tasks one by one and uncomment the calls to demos!
// ==================================================================


// ------------------------------------------------------------------
// PART #1: Asynchronous computations
// ------------------------------------------------------------------

// TASK #1: Look at `1-async.fs` to see how asynchronous workflows work!

// TASK #2: Implement 'Async.sleep'. Once you do this, you should get
// a traffic light that cycles through green, orange and red lights.

// TASK #3: Modify the demo so that it iterates over the three colours
// of the traffic light using a `for` loop rather than by explicitly
// waiting and then setting different colour three times.

// TASK #4: Implement 'Async.awaitEvent' and modify the traffic light
// so that it advances on click, rather than automatically.

// UNCOMMENT: AsyncReactive.Async.demo ()

// ------------------------------------------------------------------
// PART #2: Reactive programming with Observables
// ------------------------------------------------------------------

// TASK #1: Run the first demo and look at how observables work. Next, run 
// the second demo - to make this work, you will need to implement
// the `merge` combinator for observables (see comments in `2-observables.fs`)

// UNCOMMENT:  AsyncReactive.Observables.demo1 ()
// UNCOMMENT:  AsyncReactive.Observables.demo2 ()

// TASK #2: When you create an observable and subscribe to it, you get
// back a function that can be used to cancel the subscription. Have a
// look at 'demo3' to see how this can be used!

// UNCOMMENT:  AsyncReactive.Observables.demo3 ()

// TASK #3: Implement a slot machine simulator! Have a look at the 'demo4'
// function in '2-observables.fs' and follow the instructions there..

// UNCOMMENT:  AsyncReactive.Observables.demo4 ()

// ------------------------------------------------------------------
// PART #3: Asynchronous sequences for async pull streams
// ------------------------------------------------------------------

// TASK #1: Run the first demo to see what async sequences can do.
// Then, fix the second demo, which waits before advancing to the next
// item of the asynchronous sequence. To do this, you will need to implement
// the `delay` combinator (see `3-asyncseq.fs`) and you will need to uncomment
// one of the two `delay` calls in `demo2`.

// UNCOMMENT:  AsyncReactive.AsyncSeq.demo1 ()
// UNCOMMENT:  AsyncReactive.AsyncSeq.demo2 ()

// TASK #2: As a bonus, try to implement `AsyncSeq.take` function that
// accepts a number and returns an asynchronous sequence with at most
// that number of elements (like `List.take`) and use this function to 
// only iterate over the first 10 elements in `demo2`.