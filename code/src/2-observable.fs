module AsyncReactive.Observables

open Fable.Import
open Fable.Import.Browser
open AsyncReactive.Async
open AsyncReactive.Gui

// ------------------------------------------------------------------------------------------------
// Observables - implementation
// ------------------------------------------------------------------------------------------------

/// An observable is very similar to events, but it returns a function
/// that can be used to cancel the subscription when we are no longer
/// interested in listening to the event.
type IObservable<'T> = 
  abstract AddHandler : ('T -> unit) -> (unit -> unit)

module Observable = 
  /// Creates an observable that calls handlers when an event happens
  let on (evt:string) (el:HTMLElement) = 
    let handlers = ResizeArray<_>()
    el.addEventListener(evt, EventListenerOrEventListenerObject.Case1(fun e -> 
      for h in handlers do h e ))
    { new IObservable<_> with
        member x.AddHandler(h) = 
          handlers.Add(h) 
          (fun () -> handlers.Remove(h) |> ignore) }

  /// Add handler to an observable
  let add f (e:IObservable<_>) = 
    e.AddHandler(fun e -> f e)

  /// Creates an event that is triggered whenever either
  /// of the two observables is triggered.
  let merge (e1:IObservable<_>) (e2:IObservable<_>) = 
    { new IObservable<_> with
        member x.AddHandler(h) = 
          // TASK #1: Merge should produce a new observable that calls the
          // handler whenever either of the two source observables is triggered.
          // It should return a disposable function that disposes of both 
          // of the subscriptions to 'e1' and 'e2'. (The 'map' function below
          // is a useful inspiration for how this might look.)          
          window.alert("Not implemented!")
          failwith "Not implemented!"
          fun () -> () }
  
  /// Creates a new observable that keeps a state. When the source
  /// observable happens, the state is updated using the specified 
  /// function and the new state is reported.
  let scan v f (e:IObservable<_>) = 
    { new IObservable<_> with
        member x.AddHandler(h) = 
          let mutable value = v
          e.AddHandler(fun x -> value <- f value x; h value) }

  /// Create a derived observable that triggers a handler whenever
  /// a source observable happens, but it transforms the value using
  /// a specified function.
  let map f (e:IObservable<_>) = 
    { new IObservable<_> with
        member x.AddHandler(h) = e.AddHandler(fun x -> h (f x))  }

  // TASK #3: The 'interval' function should return 'IObservable<unit>' that 
  // is triggered every 'ms' milliseconds. You can implement this by using
  // 'window.setInterval'. Note that you will need to store the returned 'id',
  // because you need to call 'window.clearInterval' when the subscribtion is
  // cancelled (in the function you return)
  let interval ms = 
    window.alert("Not implemented!")
    failwith "Not implemented!"


module Async =
  /// Creates an asynchronous workflow that waits for the
  /// first value emitted by a given observable.
  let awaitObservable (e:IObservable<_>) = 
    { new Async<_> with 
        member x.Start(f) = 
          let mutable d = ignore
          d <- e.AddHandler(fun v -> d (); f v) }

// ------------------------------------------------------------------------------------------------
// Observables - demos
// ------------------------------------------------------------------------------------------------

let setLabel text = 
  Section2.lbl.innerText <- text

// Show UP or DOWN message when one of the buttons is clicked
let demo1 () = 
  show "section2"
  Observable.on "click" Section2.up |> Observable.map (fun _ -> "UP") |> Observable.add setLabel |> ignore
  Observable.on "click" Section2.down |> Observable.map (fun _ -> "DOWN") |> Observable.add setLabel |> ignore

// Simple up/down counter that accumulates +1 or -1 values using scan
// formats the result nicely and shows it on the label.
let demo2 () = 
  show "section2"
  let e1 = Observable.on "click" Section2.up |> Observable.map (fun _ -> 1)
  let e2 = Observable.on "click" Section2.down |> Observable.map (fun _ -> -1)

  Observable.merge e1 e2
  |> Observable.scan 0 (+)
  |> Observable.map (sprintf "Count: %d")
  |> Observable.add setLabel
  |> ignore

// In addition to 'demo2', this now also uses the 'start' and 'stop' buttons that let you
// control the counter and use the returned 'dispose' function to clear the subscriptions
let demo3 () = 
  show "section2"
  let e1 = Observable.on "click" Section2.up |> Observable.map (fun _ -> 1)
  let e2 = Observable.on "click" Section2.down |> Observable.map (fun _ -> -1)

  let evt = 
    Observable.merge e1 e2
    |> Observable.scan 0 (+)
    |> Observable.map (sprintf "Count: %d")
    
  let mutable d = ignore
  Observable.on "click" Section2.start |> Observable.add (fun _ -> 
    d <- evt |> Observable.add (fun s -> Section2.lbl.innerText <- s)) |> ignore    
  Observable.on "click" Section2.stop |> Observable.add (fun _ -> 
    Section2.lbl.innerText <- "Count: 0"
    d () ) |> ignore

// TASK #3: Slot machine simulator. The simulator has three circles and the
// player wins when they get the same color in all of the lights. The lights
// change regularly with a fixed time interval. 

type SlotEvent = 
  | One of string
  | Two of string
  | Three of string
  | Stop

let demo4 () =
  show "section3"

  let rnd = System.Random()
  let color () = ["#ff9900"; "#993366"; "#0066cc"; "#86b300"].[rnd.Next(4)]
  
  // The following creates one event 'IObservable<SlotEvent>' which is triggered
  // whenever any of the lights change or when the 'Stop' button is pressed. 
  // The 'One', 'Two' and 'Three' cases defines what light  has changed and what
  // its new colour should be.
  let slots = 
    [ Observable.interval 300  |> Observable.map (fun _ -> One(color()))
      Observable.interval 500  |> Observable.map (fun _ -> Two(color()))
      Observable.interval 700  |> Observable.map (fun _ -> Three(color())) 
      Observable.on "click" Section3.stop |> Observable.map (fun _ -> Stop) ]
    |> List.reduce Observable.merge

  // TASK #3A: First, you need to implement 'Observable.interval' (see above!)

  // TASK #3B: Now, you need to start the 'slots' subscribtion. When the user
  // clicks on 'Section3.start' button, call 'Observable.add' on 'slots' and 
  // implement a function that will change the colours of the light objects
  // (those are available in the 'Section3' module). 

  // TASK #3C: Now, detect if the user has won! To do this, use 'Observable.scan'
  // aggregate the 'slots' events. You will need to remember the three colours
  // and when 'Stop' happens, see what the colours are. Then you need to cancel
  // the subscription to 'slots' which will stop all animations.
  
  window.alert("Not implemented!")
  failwith "Not implemented!"
