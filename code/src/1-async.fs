module AsyncReactive.Async

open Fable.Import.Browser
open AsyncReactive.Gui

// ------------------------------------------------------------------------------------------------
// Async - implementation
// ------------------------------------------------------------------------------------------------

/// An async computation is an object that can be started.
/// It eventually calls the given continuation with a result 'T.
type Async<'T> = 
  abstract Start : ('T -> unit) -> unit

module Async = 
  /// Creates an async workflow that calls the 
  /// continuation after the specified number of milliseconds
  let sleep n =
    { new Async<unit> with
        member x.Start(f) = window.setTimeout(f, n) |> ignore
          // TASK #1: Implement the 'Start' operation! It should use 
          // 'window.setTimeout' to sleep for 'n' milliseconds and then 
          // call 'f'. Note that 'window.setTimeout' in JavaScript 
          // returns ID of the created timer - you need to 'ignore' that!
    }

  
  // TASK #4: Add 'awaitEvent' function that returns an 'Async<unit>'
  // like the 'sleep' function, but waits until the specified event of
  // a given HTMLElement happens.
  let awaitEvent (el:HTMLElement) (evt:string) = 
    let listener f = EventListenerOrEventListenerObject.Case1(fun _ -> f ())
    { new Async<unit> with
        member x.Start(f) = 
          el.removeEventListener(evt, listener f)
          el.addEventListener(evt, listener f)
    }

  /// Creates a computation that composes 'a' with the result of 'f'
  let bind (f:'a -> Async<'b>) (a:Async<'a>) : Async<'b> = 
    { new Async<'b> with
        member x.Start(g) =
          a.Start(fun a ->
            let ab = f a
            ab.Start(g) ) }

  /// A computation that immediately returns the given value
  let unit v = 
    { new Async<_> with
        member x.Start(f) = f v }

  /// Start the computation and do nothing when it finishes.
  let start (a:Async<_>) =
    a.Start(fun () -> ())

  /// Defines a computation builed for asyncs.
  /// For and While are defined in terms of bind and unit.
  type AsyncBuilder() = 
    member x.Return(v) = unit v
    member x.Bind(a, f) = bind f a
    member x.Zero() = unit ()

    member x.For(vals, f) =
      match vals with
      | [] -> unit ()
      | v::vs -> f v |> bind (fun () -> x.For(vs, f))

    member x.Delay(f:unit -> Async<_>) =
      { new Async<_> with
          member x.Start(h) = f().Start(h) }

    member x.While(c, f) = 
      if not (c ()) then unit ()
      else f |> bind (fun () -> x.While(c, f))

    member x.ReturnFrom(a) = a
          
let async = Async.AsyncBuilder()

// ------------------------------------------------------------------------------------------------
// Async - demo
// ------------------------------------------------------------------------------------------------

// Simple traffic light that iterates through green, orange, red indifinitely
let demo () =
  show "section1"

  // TASK #4: Implement 'Async.awaitEvent' (above) and modify the code below to use 'awaitEvent' 
  // rather than using 'sleep'. The 'awaitEvent' function should take 'HTMLElement' and the name
  // of the event. It can use something like the following code to register event handler:
  // 
  //    Section1.light.addEventListener
  //      ("click", EventListenerOrEventListenerObject.Case1(fun _ -> window.alert("Yo!")))
  //
  async {
    while true do
      for color in ["green"; "orange"; "red"] do
        do! Async.awaitEvent Section1.light "click" // note this keeps adding new event listeners
        window.alert("yo!")
        Section1.light.style.backgroundColor <- color
  } 
  |> Async.start
