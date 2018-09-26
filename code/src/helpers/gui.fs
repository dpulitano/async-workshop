module AsyncReactive.Gui
open Fable.Import.Browser
  
// ------------------------------------------------------------------------------------------------
// Infrastructure
// ------------------------------------------------------------------------------------------------

module Section1 = 
  let light = document.getElementById("light") :?> HTMLDivElement

module Section2 = 
  let up = document.getElementById("up") :?> HTMLButtonElement
  let down = document.getElementById("down") :?> HTMLButtonElement
  let lbl = document.getElementById("out") :?> HTMLParagraphElement
  let stop = document.getElementById("stop") :?> HTMLButtonElement
  let start = document.getElementById("start") :?> HTMLButtonElement

module Section3 = 
  let light1 = document.getElementById("light1") :?> HTMLDivElement
  let light2 = document.getElementById("light2") :?> HTMLDivElement
  let light3 = document.getElementById("light3") :?> HTMLDivElement
  let start = document.getElementById("slotstart") :?> HTMLButtonElement
  let stop = document.getElementById("slotstop") :?> HTMLButtonElement

module Section4 = 
  let current = document.getElementById("current") :?> HTMLParagraphElement
  let next = document.getElementById("next") :?> HTMLButtonElement

let show sec = 
  let secs = document.getElementsByTagName("section")
  for i in 0 .. int secs.length - 1 do (secs.[i] :?> HTMLTableSectionElement).style.display <- "none"
  document.getElementById(sec).style.display <- ""

