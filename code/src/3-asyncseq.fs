module AsyncReactive.AsyncSeq

open Fable.Core
open Fable.Import
open Fable.Import.Browser
open AsyncReactive.Async
open AsyncReactive.Observables
open AsyncReactive.Gui

// ------------------------------------------------------------------------------------------------
// Async sequences - implementation
// ------------------------------------------------------------------------------------------------

/// An asynchronous sequence is a computation that asynchronously produces 
/// a next cell of a linked list - the next cell can be either empty (Nil)
/// or it can be a value, followed by another asynchronous sequence.
type AsyncSeq<'T> = Async<AsyncSeqRes<'T>>
and AsyncSeqRes<'T> = 
  | Nil
  | Cons of 'T * AsyncSeq<'T>

module AsyncSeq = 
  /// A simple async function to read JSON from a given URL
  let readJson fn =
    { new Async<_> with
        member x.Start(f) =
          let xh = XMLHttpRequest.Create()
          xh.addEventListener_readystatechange(fun p -> 
            if xh.readyState > 3. && xh.status = 200. then
              f xh.responseText
            null)
          xh.``open``("GET", fn, true)
          xh.send("") }

  /// Generates an asynchronous sequence that reads the 
  /// specified files one by one and returns them.
  let rec readFiles files : AsyncSeq<_> = async {
    match files with
    | [] -> return Nil
    | f::files ->
        let! d = readJson f
        return Cons(d, readFiles files) }

  /// Iterate over the whole asynchronous sequence as fast as
  /// possible and run the specified function `f` for each value.
  let rec run f (aseq:AsyncSeq<_>) = async { 
    let! next = aseq
    match next with
    | Nil -> return ()
    | Cons(v, vs) -> 
        f v 
        return! run f vs }

  /// A function that iterates over an asynchronous sequence 
  /// and returns a new async sequence with transformed values
  let rec map f (aseq:AsyncSeq<'T>) : AsyncSeq<'R> = async { 
    let! next = aseq
    match next with
    | Nil -> return Nil
    | Cons(v, vs) -> return Cons(f v, map f vs) }


  /// TODO: Implement a function that iterates over the specified
  /// async sequence and generates a new async sequence that 
  /// returns the same values, but sleeps using `op` before advancing.
  let rec delay (op:Async<unit>) (aseq:AsyncSeq<'T>) : AsyncSeq<'T> = 
    window.alert("Not implemented!")
    failwith "Not implemented!"

/// Helper async computation that calls another 
/// computation and ignores whatever value it returns
let asyncIgnore a = async {
  let! _ = a
  return () }

// ------------------------------------------------------------------------------------------------
// Async sequences - demo
// ------------------------------------------------------------------------------------------------

[<Emit("JSON.parse($0)")>]
let jsonParse<'R> (str:string) : 'R = failwith "JS Only"

type Rate =
  { code : string
    value : float }

type Prices = 
  { rates : Rate[] }

let demo1 () = 
  show "section4"
  [ for i in 0 .. 364 -> sprintf "/data/%d.json" i ]
  |> AsyncSeq.readFiles
  |> AsyncSeq.run (fun f ->
      let p = jsonParse<Prices> f
      let gbp = p.rates |> Array.find (fun r -> r.code = "GBP")
      Section4.current.innerText <- sprintf "GBP: %A" gbp.value )
  |> Async.start


let demo2 () = 
  show "section4"

  [ for i in 0 .. 364 -> sprintf "/data/%d.json" i ]
  |> AsyncSeq.readFiles
  // TODO: Choose one of the following to either wait for a click, or wait for 250ms
  // |> AsyncSeq.delay (asyncIgnore (Async.awaitObservable (Observable.on "click" Section4.next)))
  // |> AsyncSeq.delay (Async.sleep 250)
  |> AsyncSeq.map jsonParse<Prices>
  |> AsyncSeq.map (fun p -> p.rates |> Array.find (fun r -> r.code = "GBP"))
  |> AsyncSeq.run (fun gbp -> Section4.current.innerText <- sprintf "GBP: %A" gbp.value)
  |> Async.start
