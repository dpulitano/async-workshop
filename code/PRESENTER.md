# Presenter notes

## Async

Define `Async`:

    type Async<'T> = 
      abstract Start : ('T -> unit) -> unit

Copy `sleep`:

    let sleep n =
      { new Async<unit> with
          member x.Start(f) = 
            window.setTimeout(f, n) |> ignore }

Implement `unit` and `bind`:

    let bind (f:'a -> Async<'b>) (a:Async<'a>) : Async<'b> = 
      { new Async<'b> with
          member x.Start(g) =
            a.Start(fun a ->
              let ab = f a
              ab.Start(g) ) }

    let unit v = 
      { new Async<_> with
          member x.Start(f) = f v }

Copy `AsyncBuilder`:

    type AsyncBuilder() = 
      member x.Return(v) = unit v
      member x.Bind(a, f) = bind f a
          
    let async = Async.AsyncBuilder()

Run a mini version of the demo:

    let work = async {
      do! Async.sleep 1000      
      Section1.light.style.backgroundColor <- "green"
      do! Async.sleep 1000
      Section1.light.style.backgroundColor <- "orange"
      do! Async.sleep 1000
      Section1.light.style.backgroundColor <- "red" 
      return () } 
      
    work.Start(ignore)

## Observables

Define `IObservable`:

    type IObservable<'T> = 
      abstract AddHandler : ('T -> unit) -> (unit -> unit)

Copy `on`:

    let on (evt:string) (el:HTMLElement) = 
      let handlers = ResizeArray<_>()
      el.addEventListener(evt, EventListenerOrEventListenerObject.Case1(fun e -> 
        for h in handlers do h e ))
      { new IObservable<_> with
          member x.AddHandler(h) = 
            handlers.Add(h) 
            (fun () -> handlers.Remove(h) |> ignore) }

Implement `add` and `merge`:

    let add f (e:IObservable<_>) = 
      e.AddHandler(fun e -> f e)

Implement `map`:

    let map f (e:IObservable<_>) = 
      { new IObservable<_> with
          member x.AddHandler(h) = e.AddHandler(fun x -> h (f x))  }

Run `demo1`:

    let demo1 () = 
      show "section2"
      Observable.on "click" Section2.up |> Observable.map (fun _ -> "UP") |> Observable.add setLabel |> ignore
      Observable.on "click" Section2.down |> Observable.map (fun _ -> "DOWN") |> Observable.add setLabel |> ignore
